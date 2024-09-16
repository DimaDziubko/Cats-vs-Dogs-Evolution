using _Game.Core.Services.Audio;
using _Game.Gameplay._Boosts.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._BoostPopup
{
    public class BoostPopup : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;

        [SerializeField] private BoostInfoPanel _specificBoostInfoPanel;
        [SerializeField] private BoostInfoPanel _totalBoostInfoPanel;

        [SerializeField] private Button[] _cancelButtons;
        
        private UniTaskCompletionSource<bool> _taskCompletion;
        
        private IAudioService _audioService;

        public void Construct(
            Camera uICameraOverlay, 
            IAudioService audioService)
        {
            _canvas.worldCamera = uICameraOverlay;
            _audioService = audioService;
            Init();
        }
        
        private void Init()
        {
            foreach (var button in _cancelButtons)
            {
                button.onClick.AddListener(OnCancelled);
            }

            _canvas.enabled = false;
            _specificBoostInfoPanel.Disable();
            _totalBoostInfoPanel.Disable();
        }

        public async UniTask<bool> ShowBoosts(BoostPanelModel totalBoostsModel)
        {
            UpdateBoostInfoPanel(_totalBoostInfoPanel, totalBoostsModel);
            _canvas.enabled = true;
            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            _canvas.enabled = false;
            return result;
        }
        
        public async UniTask<bool> ShowBoosts(BoostPanelModel specificBoostsModel, BoostPanelModel totalBoostsModel)
        {
            UpdateBoostInfoPanel(_specificBoostInfoPanel, specificBoostsModel);
            UpdateBoostInfoPanel(_totalBoostInfoPanel, totalBoostsModel);
            _canvas.enabled = true;
            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            _canvas.enabled = false;
            return result;
        }
        
        
        private void UpdateBoostInfoPanel(BoostInfoPanel panel, BoostPanelModel boost)
        {
            panel.Enable();
            panel.UpdateView(boost);
        }
        
        private void OnCancelled()
        {
            PlayButtonSound();
            _taskCompletion.TrySetResult(true);
        }

        private void PlayButtonSound()
        {
            _audioService.PlayButtonSound();
        }
    }
}