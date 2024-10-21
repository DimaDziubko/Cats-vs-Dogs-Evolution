using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Services.Audio;
using _Game.UI._Shop.Scripts._DecorAndUtils;
using _Game.UI.Factory;
using UnityEngine;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardsContainer : MonoBehaviour
    {
        [SerializeField] private Transform _parent;
        [SerializeField] private DynamicGridLayout _dynamicGridLayout;

        private readonly Dictionary<int, CardItemView> _cards = new  Dictionary<int, CardItemView>();
        
        private IUIFactory _uiFactory;
        private ICardsPresenter _cardsPresenter;
        private IAudioService _audioService;
        private IMyLogger _logger;

        public void Construct(
            ICardsPresenter cardsPresenter, 
            IUIFactory uiFactory,
            IAudioService audioService,
            IMyLogger logger)
        {
            _uiFactory = uiFactory;
            _cardsPresenter = cardsPresenter;
            _audioService = audioService;
            _logger = logger;
        }
        
        public void Init()
        {
            foreach (var model in _cardsPresenter.CardModels)
            {
                CreateNewCard(model.Key, model.Value);
            }
            SortCardsById();
            AdjustViewport();
            Subscribe();
        }

        public void Cleanup()
        {
            Unsubscribe();
            foreach (var card in _cards) card.Value.Release();
            _cards.Clear();
        }

        private void UpdateCard(int id, CardModel model)
        {
            if(_cards.ContainsKey(id))
                _cards[id].UpdateView(model);
            else
            {
                CreateNewCard(id, model);
                SortCardsById();
            }
            AdjustViewport();
        }

        private void AdjustViewport()
        {
            _dynamicGridLayout.AdjustViewport(_cards.Count);
        }

        private void CreateNewCard(int id, CardModel model)
        {
            CardItemView instance = _uiFactory.GetCard(_parent);
            instance.Construct(_cardsPresenter, _audioService);
            instance.UpdateView(model);
            instance.Init(id);
            _cards[id] = instance;
        }
        
        private void Subscribe()
        {
            _cardsPresenter.CardModelUpdated += UpdateCard;
        }

        private void Unsubscribe()
        {
            _cardsPresenter.CardModelUpdated -= UpdateCard;
        }
        
        private void SortCardsById()
        {
            int index = 0;
            foreach (var cardId in _cards.Keys.OrderBy(k => k))
            {
                _cards[cardId].transform.SetSiblingIndex(index);
                index++;
            }
        }
    }
}