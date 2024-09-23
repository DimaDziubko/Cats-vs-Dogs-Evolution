using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core._UpgradesChecker;
using _Game.Core.Configs.Models._Cards;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._Cards;
using _Game.Core.Configs.Repositories.Common;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._Boosts.Scripts;
using _Game.UI._MainMenu.Scripts;
using _Game.Utils;
using _Game.Utils.Extensions;
using Assets._Game.Core._UpgradesChecker;
using UnityEngine;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardsPresenter : ICardsPresenter, IDisposable, IUpgradeAvailabilityProvider
    {
        public IEnumerable<GameScreen> AffectedScreens
        {
            get
            {
                yield return GameScreen.Cards;
                yield return GameScreen.GeneralCards;
            }
        }

        public bool IsAvailable =>
            _cardModels.Any(x => x.Value.ProgressValue - 1 > Constants.ComparisonThreshold.MONEY_EPSILON);
        public event Action<int, CardModel> CardModelUpdated;

        public SortedDictionary<int, CardModel>  CardModels => _cardModels;

        private readonly SortedDictionary<int, CardModel> _cardModels = new SortedDictionary<int, CardModel> ();

        private readonly IUserContainer _userContainer;
        private readonly ICardsConfigRepository _cardsConfigRepository;
        private readonly IMyLogger _logger;
        private readonly ICommonItemsConfigRepository _commonConfig;
        private readonly IUpgradesAvailabilityChecker _upgradesChecker;
        private readonly IGameInitializer _gameInitializer;
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IBoostDataPresenter _boostDataPresenter;

        private ICardsCollectionStateReadonly CardsState => _userContainer.State.CardsCollectionState;

        private CardType _greatestAvailableType = CardType.Rare;


        public CardsPresenter(
            IUserContainer userContainer,
            IConfigRepositoryFacade configRepositoryFacade,
            IMyLogger logger,
            IUpgradesAvailabilityChecker checker,
            IGameInitializer gameInitializer,
            IWorldCameraService cameraService,
            IAudioService audioService,
            IBoostDataPresenter boostDataPresenter)
        {
            _userContainer = userContainer;
            _cardsConfigRepository = configRepositoryFacade.CardsConfigRepository;
            _logger = logger;
            _commonConfig = configRepositoryFacade.CommonItemsConfigRepository;
            _upgradesChecker = checker;
            _gameInitializer = gameInitializer;
            _cameraService = cameraService;
            _audioService = audioService;
            _boostDataPresenter = boostDataPresenter;
            _gameInitializer.OnMainInitialization += Init;
        }
        
        private void Init()
        {
            InitCardModels();
            CardsState.CardsCollected += OnCardsCollected;
            CardsState.CardUpgraded += OnCardUpgraded;
            _upgradesChecker.Register(this);
        }

        void IDisposable.Dispose()
        {
            CardsState.CardsCollected -= OnCardsCollected;
            CardsState.CardUpgraded -= OnCardUpgraded;
            _gameInitializer.OnMainInitialization -= Init;
            _upgradesChecker.UnRegister(this);
        }

        private async void OnCardsCollected(IEnumerable<int> newCardIds)
        {
            ICardAppearancePopupProvider cardAppearancePopupProvider = new CardAppearancePopupProvider(_cameraService, _audioService);
            var popup = await cardAppearancePopupProvider.Load();
            
            UpdateCardModels(newCardIds);
            
            List<CardModel> cardModelsForAnimation = new List<CardModel>();
            
            foreach (var cardId in newCardIds)
            {
                var refModel = _cardModels[cardId];
                
                var model = new CardModel()
                {
                    Config = refModel.Config,
                    Progress = refModel.Progress,
                    IsNew = refModel.IsNew && !refModel.IsNewShown,
                    ProgressValue = refModel.ProgressValue,
                    IsNewShown = refModel.IsNewShown,
                    IsGreatestType = refModel.Config.Type >= _greatestAvailableType,
                    Level = refModel.Level
                };

                refModel.IsNewShown = true;
                
                cardModelsForAnimation.Add(model);
            }
            
            var isConfirmed = await popup.Value.ShowAnimationAndAwaitForExit(cardModelsForAnimation);
            if (isConfirmed)
            {
                popup.Value.Cleanup();
                popup.Dispose();
            }
        }

        private void UpdateCardModels(IEnumerable<int> newCards)
        {
            foreach (var cardId in newCards)
            {
                CardConfig config = _cardsConfigRepository.ForCard(cardId);
                
                if (_cardModels.ContainsKey(cardId))
                {
                    var card = CardsState.Cards.FirstOrDefault(x => x.Id == cardId);
                    var model = UpdateExistingCardModel(card, config);
                    CardModelUpdated?.Invoke(cardId, model);
                }
                else
                {
                    var card = CardsState.Cards.FirstOrDefault(x => x.Id == cardId);
                    var model =  CreateNewCardModel(card, config, true, false);
                    _cardModels.Add(cardId, model);
                    CardModelUpdated?.Invoke(cardId, model);
                }
            }
        }

        private void OnCardUpgraded(int cardId)
        {
            var card = CardsState.Cards.FirstOrDefault(x => x.Id == cardId);
            var model = _cardModels[cardId];
            CardConfig config = model.Config;
            UpdateExistingCardModel(card, config);
            CardModelUpdated?.Invoke(cardId, model);
        }

        private CardModel UpdateExistingCardModel(Card card, CardConfig config)
        {
            var model = _cardModels[card.Id];
            model.Level = $"Level {card.Level}";
            model.Progress = $"{card.Count}/{config.GetUpgradeCount(card.Level)}";
            model.ProgressValue = Mathf.Min((float)card.Count / config.GetUpgradeCount(card.Level), 1);
            model.UpgradeCount = config.GetUpgradeCount(card.Level);
            UpdateBoostModels(model.BoostItemModels, card.Level, config);
            return model;
        }

        private CardModel CreateNewCardModel(Card card, CardConfig config, bool isNew, bool isNewShown)
        {
            var model = new CardModel()
            {
                Config = config,
                Level =  $"Level {card.Level}",
                Progress = $"{card.Count}/{config.GetUpgradeCount(card.Level)}",
                ProgressValue = Mathf.Min((float)card.Count / config.GetUpgradeCount(card.Level), 1),
                IsNew = isNew,
                IsNewShown = isNewShown,
                BoostItemModels = CreateBoostModels(card.Level, config),
                UpgradeCount = config.GetUpgradeCount(card.Level)
            };
            
            if (model.Config.Type > _greatestAvailableType)
                _greatestAvailableType = model.Config.Type;
            
            return model;
        }

        private List<BoostItemModel> CreateBoostModels(int cardLevel, CardConfig config)
        {
            if (config.Boosts == null) return new List<BoostItemModel>();
            List<BoostItemModel> boosts = new List<BoostItemModel>(config.Boosts.Length);
            foreach (var boost in config.Boosts)
            {
                BoostItemModel model = new BoostItemModel()
                {
                    BoostIcon = _commonConfig.ForBoostIcon(boost.Type),
                    CurrentValue = $"x{boost.Exponential.GetValue(cardLevel).ToFormattedString()}",
                    NextValue = $"x{boost.Exponential.GetValue(cardLevel + 1).ToFormattedString()}",
                    BoostName = boost.Name
                };
                
                boosts.Add(model);
            }
            
            return boosts;
        }

        private void UpdateBoostModels(List<BoostItemModel> modelBoostItemModels, int cardLevel, CardConfig config)
        {
            if(modelBoostItemModels == null) return;
            if(modelBoostItemModels.Count == 0) return;
            
            int boostIndex = 0;
            foreach (var boost in config.Boosts)
            {
                modelBoostItemModels[boostIndex].CurrentValue =
                    $"x{boost.Exponential.GetValue(cardLevel).ToFormattedString()}";
                modelBoostItemModels[boostIndex].NextValue =
                    $"x{boost.Exponential.GetValue(cardLevel + 1).ToFormattedString()}";
                boostIndex++;
            }
        }


        public void UpgradeCard(int id)
        {
            int needForUpgrade = _cardModels[id].UpgradeCount;
            _userContainer.UpgradeStateHandler.UpgradeCard(id, needForUpgrade);
        }
        
        async void ICardsPresenter.OnCardClicked(int id)
        {
            ICardPopupProvider provider = new CardPopupProvider(_cameraService, this, _audioService, _boostDataPresenter);
            var popup = await provider.Load();
            var isConfirmed = await popup.Value.ShowDetailsAndAwaitForExit(id);
            if (isConfirmed)
            {
                provider.Unload();
            }
        }

        private void InitCardModels()
        {
            List<Card> boughtCards = CardsState.Cards;
            foreach (var card in boughtCards)
            {
                CardConfig config = _cardsConfigRepository.ForCard(card.Id);

                var model = CreateNewCardModel(card, config, false, true);

                _logger.Log($"Card Name: {model.Config.Name} Progress Value: {model.ProgressValue}");
                    
                _cardModels.Add(card.Id, model);
            }
        }
    }
}