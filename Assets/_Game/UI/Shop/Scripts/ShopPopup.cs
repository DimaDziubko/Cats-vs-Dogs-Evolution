using _Game.Core.Communication;
using _Game.Core.Services.Audio;
using _Game.Core.Services.PersistentData;
using _Game.UI.Common.Scripts;
using _Game.Utils.Popups;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.Shop.Scripts
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
        private IPersistentDataService _persistentData;
        private IUserStateCommunicator _communicator;
        private IAlertPopupProvider _alertPopupProvider;
        
        public void Construct(
            Camera uICamera,
            IPersistentDataService persistentData,
            IAudioService audioService,
            IUserStateCommunicator communicator,
            IAlertPopupProvider alertPopupProvider)
        {
            _canvas.worldCamera = uICamera;
            _audioService = audioService;
            _persistentData = persistentData;
            _communicator = communicator;
            _alertPopupProvider = alertPopupProvider;
        }

        public async UniTask<bool> AwaitForExit()
        {
            _canvas.enabled = true;
            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            _canvas.enabled = false;
            return result;
        }

        private void SaveGame()
        {
            _communicator.SaveUserState(_persistentData.State);
        }

        private void OnCloseBtnClick()
        {
            _audioService.PlayButtonSound();
            _taskCompletion.TrySetResult(true);
        }
        
    }
}