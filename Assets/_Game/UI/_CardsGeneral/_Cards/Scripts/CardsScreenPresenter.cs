using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core._UpgradesChecker;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._Cards;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._Currencies;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.Common.Scripts;
using Assets._Game.Core._UpgradesChecker;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardsScreenPresenter : IDisposable, ICardsScreenPresenter, IUpgradeAvailabilityProvider
    {
        public IEnumerable<GameScreen> AffectedScreens
        {
            get
            {
                yield return GameScreen.GeneralCards;
            }
        }
        public bool IsAvailable => _buttonModels.Any(x => x.State == ButtonState.Active);
        
        public event Action<TransactionButtonModel[]> ButtonModelsChanged;
        public event Action<int> CardBought;
        public ICardsSummoningPresenter CardsSummoningPresenter { get; private set; }

        public string CardsCountInfo => $"{CardsState.Cards.Count}/{_cardsConfigRepository.GetAllCardsCount()}";

        private readonly IUserContainer _userContainer;
        private readonly ICardsConfigRepository _cardsConfigRepository;
        private readonly IGameInitializer _gameInitializer;
        private readonly IUpgradesAvailabilityChecker _upgradesChecker;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private IUserCurrenciesStateReadonly Currencies => _userContainer.State.Currencies;
        private ICardsCollectionStateReadonly CardsState => _userContainer.State.CardsCollectionState;


        private readonly TransactionButtonModel[] _buttonModels 
            = {new TransactionButtonModel(), new TransactionButtonModel()};

        public TransactionButtonModel[] ButtonModels => _buttonModels;


        public CardsScreenPresenter(
            IUserContainer userContainer,
            IConfigRepositoryFacade facade,
            IGameInitializer gameInitializer,
            IMyLogger logger,
            IUpgradesAvailabilityChecker upgradesChecker,
            IFeatureUnlockSystem featureUnlockSystem)
        {
            _gameInitializer = gameInitializer;
            _userContainer = userContainer;
            _cardsConfigRepository = facade.CardsConfigRepository;

            CardsSummoningPresenter = new CardsSummoningPresenter(userContainer, facade.CardsConfigRepository);
            _upgradesChecker = upgradesChecker;

            _featureUnlockSystem = featureUnlockSystem;

            gameInitializer.OnMainInitialization += Init;
        }

        void ICardsScreenPresenter.TryToBuyX1Card()
        {
            int amount = 1;
            var price = _cardsConfigRepository.GetX1CardPrice();
            if(price > Currencies.Gems) return;
            _userContainer.CurrenciesHandler.SpendGems(price, CurrenciesSource.Cards);
            CardBought?.Invoke(amount);
        }

        void ICardsScreenPresenter.TryToBuyX10Card()
        {
            int amount = 10;
            var price = _cardsConfigRepository.GetX10CardPrice();
            if(price > Currencies.Gems) return;
            _userContainer.CurrenciesHandler.SpendGems(price, CurrenciesSource.Cards);
            CardBought?.Invoke(amount);
        }

        void ICardsScreenPresenter.OnCardsScreenOpened() => 
            UpdateButtonModels();

        void Init()
        {
            _upgradesChecker.Register(this);
            CardsSummoningPresenter.Init();
            Currencies.CurrenciesChanged += OnCurrenciesChanged;
            _featureUnlockSystem.FeatureUnlocked += OnFeatureUnlocked;
            UpdateButtonModels();
        }

        void IDisposable.Dispose()
        {
            _upgradesChecker.UnRegister(this);
            _gameInitializer.OnMainInitialization -= Init;
            Currencies.CurrenciesChanged -= OnCurrenciesChanged;
            _featureUnlockSystem.FeatureUnlocked -= OnFeatureUnlocked;
        }

        private void OnFeatureUnlocked(Feature feature)
        {
            if(feature == Feature.GemsShopping)
                UpdateButtonModels();
        }

        private void OnCurrenciesChanged(CurrencyType currencyType, double delta, CurrenciesSource source)
        {
            UpdateButtonModels();
        }

        private void UpdateButtonModels()
        {
            _buttonModels[0].Price = _cardsConfigRepository.GetX1CardPrice().ToString();
            _buttonModels[0].State = _cardsConfigRepository.GetX1CardPrice() 
                                     < Currencies.Gems
                                     && _featureUnlockSystem.IsFeatureUnlocked(Feature.GemsShopping)
                ? ButtonState.Active
                : ButtonState.Inactive;
            
            _buttonModels[1].Price = _cardsConfigRepository.GetX10CardPrice().ToString();
            _buttonModels[1].State = _cardsConfigRepository.GetX10CardPrice() < Currencies.Gems
                ? ButtonState.Active
                : ButtonState.Inactive;
            
            ButtonModelsChanged?.Invoke(ButtonModels);
        }
    }
}