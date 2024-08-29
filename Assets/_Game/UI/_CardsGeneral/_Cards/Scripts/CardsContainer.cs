using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.UI.Factory;
using Assets._Game.Core.Services.Audio;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardsContainer : MonoBehaviour
    {
        [SerializeField] private Transform _parent;

        private readonly Dictionary<int, CardView> _cards = new  Dictionary<int, CardView>();
        
        private IUIFactory _uiFactory;
        private ICardsScreenPresenter _cardsScreenPresenter;
        private IAudioService _audioService;
        private IMyLogger _logger;

        public void Construct(
            ICardsScreenPresenter cardsScreenPresenter, 
            IUIFactory uiFactory,
            IAudioService audioService,
            IMyLogger logger)
        {
            _uiFactory = uiFactory;
            _cardsScreenPresenter = cardsScreenPresenter;
            _audioService = audioService;
            _logger = logger;
        }
        
        public void Init()
        {
            foreach (var model in _cardsScreenPresenter.CardsPresenter.CardModels)
            {
                CreateNewCard(model.Key, model.Value);
            }

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
            }
        }

        private void CreateNewCard(int id, CardModel model)
        {
            CardView instance = _uiFactory.GetCard(_parent);
            instance.Construct(_cardsScreenPresenter.CardsPresenter, _audioService);
            instance.UpdateView(model);
            instance.Init();
            _cards[id] = instance;
        }

        [Button]
        private void UpdateCards()
        {
            foreach (var pair in _cards)
            {
                pair.Value.UpdateView(_cardsScreenPresenter.CardsPresenter.CardModels[pair.Key]);
                break;
            }
        }
        
        private void Subscribe()
        {
            _cardsScreenPresenter.CardsPresenter.CardModelUpdated += UpdateCard;
        }

        private void Unsubscribe()
        {
            _cardsScreenPresenter.CardsPresenter.CardModelUpdated -= UpdateCard;
        }
    }
}