using System.Collections.Generic;
using _Game.Core.Services.Audio;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardAppearancePopup : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CardView _singleView;
        [SerializeField] private CardView[] _10Views;
        [SerializeField] private Button _exitButton;
        [SerializeField] private int _appearanceDelay = 1000;
        [SerializeField] private int _collectionAppearanceDelay = 200;
        [SerializeField] private int _illuminationAnimationDelay = 200;
        
        private IAudioService _audioService;
        
        private UniTaskCompletionSource<bool> _taskCompletion;

        public void Construct(
            Camera cameraServiceUICameraOverlay,
            IAudioService audioService)
        {
            _canvas.worldCamera = cameraServiceUICameraOverlay;
            _audioService = audioService;
            _singleView.Disable();
            foreach (var view in _10Views)
            {
                view.Disable();
            }

            Init();
        }

        private void Init()
        {
            _exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        public void Cleanup()
        {
            _exitButton.onClick.RemoveAllListeners();
        }

        private void OnExitButtonClicked()
        {
            _taskCompletion.TrySetResult(true);
            PlayButtonSound();
        }

        public async UniTask<bool> ShowAnimationAndAwaitForExit(
            List<CardModel> cardModelsForAnimation)
        {
            _exitButton.interactable = false;
            _canvas.enabled = true;
            _taskCompletion = new UniTaskCompletionSource<bool>();
            
            await UniTask.Delay(_appearanceDelay);
            if (cardModelsForAnimation.Count == 1)
            {
                await PlaySingleAnimation(_singleView, cardModelsForAnimation[0]);
            }
            else
            {
                await PlayCollectionAnimation(cardModelsForAnimation);
            }
            
            _exitButton.interactable = true;
            var result = await _taskCompletion.Task;
            _canvas.enabled = false;
            return result;
        }

        private async UniTask PlaySingleAnimation(CardView view, CardModel cardModel)
        {
            bool needIlluminationAnimation = cardModel.IsNew || cardModel.IsGreatestType; 
            view.UpdateView(cardModel);
            view.Enable();
            view.PlayAppearanceAnimation(
                _audioService, 
                cardModel.Config.ColorIdentifier,
                needIlluminationAnimation,
                cardModel.IsNew);
            if (needIlluminationAnimation)
            {
                await UniTask.Delay(_illuminationAnimationDelay);
            }
            await UniTask.Yield();
        }

        private async UniTask PlayCollectionAnimation(List<CardModel> cardModelsForAnimation)
        {
            for (int i = 0; i < cardModelsForAnimation.Count; i++)
            {
                await PlaySingleAnimation(_10Views[i], cardModelsForAnimation[i]);
                await UniTask.Delay(_collectionAppearanceDelay);
            }
        }

        private void PlayButtonSound() => _audioService.PlayButtonSound();
    }
}

    