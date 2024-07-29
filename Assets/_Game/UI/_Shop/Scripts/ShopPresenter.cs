using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._GameInitializer;
using _Game.Core._UpgradesChecker;
using _Game.Core.Data;
using _Game.Core.Services._FreeGemsPackService;
using _Game.Core.Services.IAP;
using _Game.Core.Services.IGPService;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState;
using _Game.UI._Currencies;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.Common.Scripts;
using _Game.Utils;
using Assets._Game.Core._UpgradesChecker;

namespace _Game.UI._Shop.Scripts
{
    public class ShopPresenter : IShopPresenter, IDisposable, IUpgradeAvailabilityProvider
    {
        public event Action<List<ShopItemModel>> ShopItemsUpdated;

        public IEnumerable<Screen> AffectedScreens
        {
            get
            {
                yield return Screen.Shop;
            }
        }

        public bool IsAvailable 
            => _models.Any(x => x.ButtonState == ButtonState.Active);

        private readonly IGeneralDataPool _generalDataPool;
        private readonly IIAPService _iapService;
        private readonly IIGPService _igpService;
        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly IFreeGemsPackService _freeGemsPackService;
        private readonly IUpgradesAvailabilityChecker _checker;

        private IUserCurrenciesStateReadonly Currencies => _userContainer.State.Currencies;
        private IPurchaseDataStateReadonly Purchases => _userContainer.State.PurchaseDataState;
        private IFreeGemsPackStateReadonly FreeGemsPackState => _userContainer.State.FreeGemsPackState;
        
        private List<ShopItemModel> _models;

        public ShopPresenter(
            IGeneralDataPool generalDataPool,
            IIAPService iapService,
            IIGPService igpService,
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IFreeGemsPackService freeGemsPackService,
            IUpgradesAvailabilityChecker checker)
        {
            _generalDataPool = generalDataPool;
            _iapService = iapService;
            _igpService = igpService;
            _freeGemsPackService = freeGemsPackService;
            _userContainer = userContainer;
            _checker = checker;
            gameInitializer.OnMainInitialization += Init;
            _gameInitializer = gameInitializer;
        }

        private void Init()
        {
            _checker.Register(this);
            _freeGemsPackService.FreeGemsPackUpdated += UpdateItems;
            FreeGemsPackState.FreeGemsPackCountChanged += UpdateItems;
            Currencies.CurrenciesChanged += OnCurrenciesChanged;
            _iapService.Initialized += UpdateItems;
            Purchases.Changed += OnPurchasesChanged;
            UpdateItems();
        }


        public void Dispose()
        {
            _checker.UnRegister(this);
            _freeGemsPackService.FreeGemsPackUpdated -= UpdateItems;
            FreeGemsPackState.FreeGemsPackCountChanged -= UpdateItems;
            _iapService.Initialized -= UpdateItems;
            Currencies.CurrenciesChanged -= OnCurrenciesChanged;
            Purchases.Changed -= OnPurchasesChanged;
            _gameInitializer.OnMainInitialization -= Init;
        }

        private void OnPurchasesChanged() => 
            UpdateItems();

        private void OnCurrenciesChanged(Currencies _, bool __) => 
            UpdateItems();

        public void OnShopOpened()
        {
            _freeGemsPackService.UpdateFreeGemsPack();
            UpdateItems();
        }

        public void TryToBuy(ProductDescription productDescription)
        {
            if (productDescription.Id == Constants.ConfigKeys.PLACEMENT)
            {
                _freeGemsPackService.OnFreeGemsPackBtnClicked();
                return;
            }
            if (productDescription.Id != Constants.ConfigKeys.MISSING_KEY)
            {
                _iapService.StartPurchase(productDescription.Id);
                return;
            }
            _igpService.StartPurchase(productDescription.Config);
        }

        private void UpdateItems()
        {
            _models = new List<ShopItemModel>();
            UpdateIAPs(_models);
            UpdateIGPs(_models);
            UpdateAdPlacement(_models);
            ShopItemsUpdated?.Invoke(_models);
        }

        private void UpdateIGPs(List<ShopItemModel> models)
        {
            List<ProductDescription> products = _igpService.Products();
            foreach (var product in products)
            {
                ShopItemModel model = new ShopItemModel
                {
                    Description = product,
                    ButtonState = product.Config.Price <= Currencies.Gems 
                        ? ButtonState.Active 
                        : ButtonState.Inactive,
                    ItemStaticData = _generalDataPool.ShopItemStaticDataPool.ForType(product.Config.Id)
                };
                
                models.Add(model);
            }
        }

        private void UpdateIAPs(List<ShopItemModel> models )
        {
            if(!_iapService.IsInitialized) return;
            List<ProductDescription> products = _iapService.Products();
            foreach (var product in products)
            {
                ShopItemModel model = new ShopItemModel
                {
                    Description = product,
                    ButtonState = ButtonState.Active,
                    ItemStaticData = _generalDataPool.ShopItemStaticDataPool.ForType(product.Config.Id)
                };
                
                models.Add(model);
            }
        }

        private void UpdateAdPlacement(List<ShopItemModel> models)
        {
            ProductDescription product = _freeGemsPackService.FreeGemsPack;
            
            ButtonState state;
            if (!product.IsReady)
                state = ButtonState.Loading;
            else if (product.AvailablePurchasesLeft > 0)
                state = ButtonState.Active;
            else
                state = ButtonState.Inactive;

            ShopItemModel model = new ShopItemModel
            {
                Description = product,
                ButtonState = state,
                ItemStaticData = _generalDataPool.ShopItemStaticDataPool.ForType(product.Config.Id)
            };
            models.Add(model);
        }
    }
}