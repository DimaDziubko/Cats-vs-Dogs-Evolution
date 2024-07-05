using Assets._Game.Core.Services.Audio;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.UI.Shop.Scripts
{
    [RequireComponent(typeof(Canvas))]
    public class ShopPopup : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;

        [SerializeField] private Button _buyButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _rightButton;
        [SerializeField] private Button _leftButton;
        
        private UniTaskCompletionSource<bool> _taskCompletion;
        
        private IAudioService _audioService;

        public void Construct(
            Camera uICamera,
            IAudioService audioService)
        {
            _canvas.worldCamera = uICamera;
            _audioService = audioService;
        }

        public async UniTask<bool> AwaitForExit()
        {
            _canvas.enabled = true;
            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            _canvas.enabled = false;
            return result;
        }
        
        private void OnCloseBtnClick()
        {
            _audioService.PlayButtonSound();
            _taskCompletion.TrySetResult(true);
        }
        
    }
}