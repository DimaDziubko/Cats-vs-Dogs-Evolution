using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._GameInitializer;
using _Game.Core.Configs.Models._Cards;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._Cards;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._Currencies;
using _Game.UI.Common.Scripts;
using UnityEngine;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardsScreenPresenter : IDisposable, ICardsScreenPresenter
    {
        public event Action<TransactionButtonModel[]> ButtonModelsChanged;
        public event Action<int> CardBought;
        public ICardsSummoningPresenter CardsSummoningPresenter { get; private set; }
        public ICardsPresenter CardsPresenter { get; private set; }

        private readonly IUserContainer _userContainer;
        private readonly ICardsConfigRepository _cardsConfigRepository;
        private readonly IGameInitializer _gameInitializer;
        private IUserCurrenciesStateReadonly Currencies => _userContainer.State.Currencies;
        private ICardsCollectionStateReadonly CardsState => _userContainer.State.CardsCollectionState;
        
        private readonly TransactionButtonModel[] _buttonModels 
            = {new TransactionButtonModel(), new TransactionButtonModel()};
        public TransactionButtonModel[] ButtonModels => _buttonModels;

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

        public CardsScreenPresenter(
            IUserContainer userContainer,
            IConfigRepositoryFacade facade,
            IGameInitializer gameInitializer)
        {
            _gameInitializer = gameInitializer;
            _userContainer = userContainer;
            _cardsConfigRepository = facade.CardsConfigRepository;
            
            CardsSummoningPresenter = new CardsSummoningPresenter(userContainer, facade.CardsConfigRepository);
            CardsPresenter = new CardsPresenter(userContainer, facade.CardsConfigRepository);
            
            gameInitializer.OnMainInitialization += Init;
        }

        void Init()
        {
            CardsSummoningPresenter.Init();
            CardsPresenter.Init();
            Currencies.CurrenciesChanged += OnCurrenciesChanged;
            UpdateButtonModels();
        }
        
        private void OnCurrenciesChanged(Currencies currencies, double delta, CurrenciesSource source)
        {
            UpdateButtonModels();
        }

        private void UpdateButtonModels()
        {
            _buttonModels[0].Price = _cardsConfigRepository.GetX1CardPrice().ToString();
            _buttonModels[0].State = _cardsConfigRepository.GetX1CardPrice() < Currencies.Gems
                ? ButtonState.Active
                : ButtonState.Inactive;
            
            _buttonModels[1].Price = _cardsConfigRepository.GetX10CardPrice().ToString();
            _buttonModels[1].State = _cardsConfigRepository.GetX10CardPrice() < Currencies.Gems
                ? ButtonState.Active
                : ButtonState.Inactive;
            
            ButtonModelsChanged?.Invoke(ButtonModels);
        }

        void IDisposable.Dispose()
        {
            _gameInitializer.OnMainInitialization -= Init;
            Currencies.CurrenciesChanged -= OnCurrenciesChanged;
        }
    }
}