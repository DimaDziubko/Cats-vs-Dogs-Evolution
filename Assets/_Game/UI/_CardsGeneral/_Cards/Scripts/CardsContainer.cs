using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.UI.Factory;
using Assets._Game.Core.Services.Audio;
using UnityEngine;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardsContainer : MonoBehaviour
    {
        [SerializeField] private Transform _parent;

        private readonly List<CardView> _cards = new List<CardView>();
        
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

        public void UpdateCards(List<CardModel> models)
        {
            Cleanup();
            
            foreach (var model in models)
            {
                //TODO Implement later
                //CardView instance = _uiFactory.GetCard();
                //instance.Construct(_shopPresenter, model, _audioService);
                //_cards.Add(instance);
            }

            _logger.Log("Cards updated");
            Init();
        }

        private void Init()
        {
            
            foreach (var card in _cards)
            {
                card.Init();
            }
        }
        
        public void Cleanup()
        {
            foreach (var card in _cards) card.Release();
            _cards.Clear();
        }
    }
}