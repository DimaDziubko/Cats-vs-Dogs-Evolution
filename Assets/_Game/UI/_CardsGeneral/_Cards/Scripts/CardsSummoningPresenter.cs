using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Configs.Models._Cards;
using _Game.Core.Configs.Repositories._Cards;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using UnityEngine;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardsSummoningPresenter : ICardsSummoningPresenter
    {
        public event Action<CardsSummoningModel> CardsSummoningModelChanged;
        
        private readonly IUserContainer _userContainer;
        private readonly ICardsConfigRepository _cardsConfigRepository;

        private IUserCurrenciesStateReadonly Currencies => _userContainer.State.Currencies;
        private ICardsCollectionStateReadonly CardsState => _userContainer.State.CardsCollectionState;

        private readonly CardsSummoningModel _cardsSummoningModel = new CardsSummoningModel();
        public CardsSummoningModel CardsSummoningModel => _cardsSummoningModel;
        
        public CardsSummoningPresenter(
            IUserContainer userContainer,
            ICardsConfigRepository cardsConfigRepository)
        {
            _userContainer = userContainer;
            _cardsConfigRepository = cardsConfigRepository;
        }
        
        public void Init()
        {
            CardsState.CardsSummoningProgressChanged += OnSummoningProgressChanged;
            CardsState.CardsSummoningLevelChanged += OnLevelChanged;
            CheckSummoningLevel();
            UpdateSummoningModel();
        }

        private void OnLevelChanged(int _)
        {
            UpdateSummoningModel();
        }

        private void OnSummoningProgressChanged(int obj)
        {
            if (CheckSummoningLevel()) return;
            UpdateSummoningModel();
        }

        private bool CheckSummoningLevel()
        {
            if (CardsState.CardsSummoningLevel < _cardsConfigRepository.MaxSummoningLevel)
            {
                if (NeedLevelUp(
                    CardsState.CardsSummoningLevel,
                    _cardsConfigRepository.MaxSummoningLevel,
                    CardsState.CardsSummoningProgressCount,
                    _cardsConfigRepository.GetAllSummonings(), out int level))
                {
                    LevelUp(level);
                    return true;
                }
            }

            return false;
        }

        private void UpdateSummoningModel()
        {
            Dictionary<int, CardsSummoning> allSummonings = _cardsSummoningModel.AllCardSummonings 
                                                            ?? _cardsConfigRepository.GetAllSummonings();
            int collectedCardsInCurrentLevel = Mathf.Max(0,
                CardsState.CardsSummoningProgressCount -
                allSummonings[CardsState.CardsSummoningLevel].AccumulatedCardsRequiredForLevel);
            
            _cardsSummoningModel.CurrentLevel = CardsState.CardsSummoningLevel;

            if (CardsState.CardsSummoningLevel < _cardsConfigRepository.MaxSummoningLevel)
            {
                int cardsRequiredForNextLevel = allSummonings[CardsState.CardsSummoningLevel + 1].CardsRequiredForLevel;
                _cardsSummoningModel.Progress = $"{collectedCardsInCurrentLevel}/{cardsRequiredForNextLevel}";
                _cardsSummoningModel.ProgressValue = (float)collectedCardsInCurrentLevel / cardsRequiredForNextLevel;
            }
            else
            {
                _cardsSummoningModel.Progress = "max.";
                _cardsSummoningModel.ProgressValue = 1;
            }

            _cardsSummoningModel.AllCardSummonings = allSummonings;
            _cardsSummoningModel.MinSummoningLevel = _cardsConfigRepository.MinSummoningLevel;
            _cardsSummoningModel.MaxSummoningLevel = _cardsConfigRepository.MaxSummoningLevel;
            
            CardsSummoningModelChanged?.Invoke(CardsSummoningModel);
        }

        private void LevelUp(int level) => 
            _userContainer.UpgradeStateHandler.ChangeCardSummoningLevel(level);

        private bool NeedLevelUp(
            int currentLevel,
            int maxLevel,
            int progress,
            Dictionary<int, CardsSummoning> allSummonings,
            out int level)
        {
            if (progress >= allSummonings[maxLevel].AccumulatedCardsRequiredForLevel)
            {
                level = maxLevel;
                return true;
            }
            
            var achievedLevel 
                = allSummonings.LastOrDefault(x => x.Value.AccumulatedCardsRequiredForLevel < progress + 1).Key;
            
            if (achievedLevel > currentLevel)
            {
                level = achievedLevel;
                return true;
            }

            level = currentLevel;
            return false;
        }


        public void Cleanup()
        {
            CardsState.CardsSummoningProgressChanged -= OnSummoningProgressChanged;
            CardsState.CardsSummoningLevelChanged -= OnLevelChanged;
        }
    }
}