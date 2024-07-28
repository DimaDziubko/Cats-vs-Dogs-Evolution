using _Game.Core.Services.IAP;
using _Game.UI.Common.Scripts;
using _Game.UI.Factory;
using _Game.Utils;
using Assets._Game.Core.Services.Audio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Shop.Scripts
{
    public class ShopItem : MonoBehaviour
    {
        [SerializeField] private Image _majorProductIconHolder;
        [SerializeField] private Image _minorProductIconHolder;
        [SerializeField] private TMP_Text _quantity;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private TMP_Text _valueLabel;

        [SerializeField] private TransactionButton _button;
        public IUIFactory OriginFactory { get; set; }

        private ProductDescription _productDescription;
        private IShopPresenter _shopPresenter;
        private IAudioService _audioService;

        public void Construct(
            IShopPresenter shopPresenter, 
            ShopItemModel model, 
            IAudioService audioService)
        {
            _shopPresenter = shopPresenter;
            _productDescription = model.Description;
            _audioService = audioService;

            if (_valueLabel != null)
            {
                switch (model.Description.Config.ItemType)
                {
                    case Core.Services.IAP.ItemType.x1_5:
                        _valueLabel.text = "x1.5";
                        break;
                    case Core.Services.IAP.ItemType.x2:
                        _valueLabel.text = "x2";
                        break;
                    case Core.Services.IAP.ItemType.Gems:
                        break;
                    case Core.Services.IAP.ItemType.Coins:
                        break;
                }
            }
            
            if(_majorProductIconHolder != null)
                _majorProductIconHolder.sprite = model.ItemStaticData.MajorProductIcon;
            if (_minorProductIconHolder != null)
                _minorProductIconHolder.sprite = model.ItemStaticData.MinorProductIcon;
            if (_quantity != null)
            {
                _quantity.text = model.Description.Config.Quantity.ToString();
            }
            if (_description != null)
            {
                _description.text = model.Description.Config.Description;
            }

            if (_button != null)
            {
                var price = model.Description.Id != Constants.ConfigKeys.MISSING_KEY 
                    ? model.Description.Product.metadata.localizedPriceString 
                    : model.Description.Config.Price.ToString();
                _button.Construct(model.ItemStaticData.CurrencyIcon);
                _button.Init();
                _button.UpdateButtonState(model.CanAfford, price);
            }
        }

        public void Init()
        {
            _button.Init();
            _button.Click += OnTransactionButtonClicked;
        }

        public void Release()
        {
            _button.Click -= OnTransactionButtonClicked;
            _button.Cleanup();
            OriginFactory.Reclaim(this);
        }

        private void OnTransactionButtonClicked()
        {
            _shopPresenter.TryToBuy(_productDescription);
            _audioService.PlayButtonSound();
        }
    }
}