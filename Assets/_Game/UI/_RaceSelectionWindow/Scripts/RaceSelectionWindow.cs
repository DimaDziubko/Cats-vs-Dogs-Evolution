using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using _Game.Core.Services.PersistentData;
using _Game.Gameplay.Common.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._RaceSelectionWindow.Scripts
{
    public class RaceSelectionWindow : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Button _firstFactionBtn;
        [SerializeField] private Button _secondFactionBtn;

        private IUserContainer _persistentData;
        private IAudioService _audioService;

        private UniTaskCompletionSource<bool> _taskCompletion;

        public void Construct(
            IUserContainer persistentData,
            IAudioService audioService,
            IWorldCameraService worldCameraService)
        {
            _canvas.worldCamera = worldCameraService.UICameraOverlay;
            _persistentData = persistentData;
            _audioService = audioService;
            
            _firstFactionBtn.onClick.AddListener(OnFirstFactionBtnClicked);   
            _secondFactionBtn.onClick.AddListener(OnSecondFactionBtnClicked);   
        }

        public async UniTask<bool> AwaitForDecision()
        {
            _canvas.enabled = true;
            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            _canvas.enabled = false;
            return result;
        }

        private void OnFirstFactionBtnClicked()
        {
            _persistentData.ChooseRace(Race.Cat);
            _audioService.PlayButtonSound();
            
            _taskCompletion.TrySetResult(true);
            Cleanup();
        }

        private void OnSecondFactionBtnClicked()
        {
            _persistentData.ChooseRace(Race.Dog);
            _audioService.PlayButtonSound();
            
            _taskCompletion.TrySetResult(true);
            Cleanup();
        }

        private void Cleanup()
        {
            _firstFactionBtn.onClick.RemoveAllListeners();
            _secondFactionBtn.onClick.RemoveAllListeners();
        }
    }
}
