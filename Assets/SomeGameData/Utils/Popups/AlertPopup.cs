using Assets._Game.Core.Services.Audio;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Utils.Popups
{
    [RequireComponent(typeof(Canvas))]
    public class AlertPopup : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
    
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Button _okButton;
        [SerializeField] private Button _cancelButton;

        private UniTaskCompletionSource<bool> _taskCompletion;
        
        private IAudioService _audioService;

        public void Construct(
            Camera uICamera,
            IAudioService audioService)
        {
            _canvas.worldCamera = uICamera;
            _canvas.enabled = false;
            _okButton.onClick.AddListener(OnAccept);
            _cancelButton.onClick.AddListener(OnCancelled);
            _audioService = audioService;
        }

        public async UniTask<bool> AwaitForDecision(string text)
        {
            _text.text = text;
            _canvas.enabled = true;
            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            _canvas.enabled = false;
            return result;
        }

        private void OnAccept()
        {
            _audioService.PlayButtonSound();
            _taskCompletion.TrySetResult(true);
        }

        private void OnCancelled()
        {
            _audioService.PlayButtonSound();
            _taskCompletion.TrySetResult(false);
        }
    }
}