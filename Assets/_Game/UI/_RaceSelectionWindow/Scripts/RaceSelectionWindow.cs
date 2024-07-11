using _Game.Core.Services.UserContainer;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Core.UserState;
using Assets._Game.Gameplay._Tutorial.Scripts;
using Assets._Game.Gameplay.Common.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.UI._RaceSelectionWindow.Scripts
{
    public class RaceSelectionWindow : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private TutorialPointerView[] _decorPointers;
        [SerializeField] private RaceSelectionBtn _catSelectionBtn;
        [SerializeField] private RaceSelectionBtn _dogSelectionBtn;

        [SerializeField] private Button _selectBtn;

        [SerializeField] private TutorialStep _tutorialStep;

        private ITutorialManager _tutorialManager;
        private IUserContainer _persistentData;
        private IAudioService _audioService;

        private IRaceStateReadonly RaceState => _persistentData.State.RaceState;


        private UniTaskCompletionSource<bool> _taskCompletion;

        public void Construct(
            IUserContainer persistentData,
            IAudioService audioService,
            IWorldCameraService worldCameraService,
            ITutorialManager tutorialManager)
        {
            _canvas.worldCamera = worldCameraService.UICameraOverlay;
            _persistentData = persistentData;
            _audioService = audioService;
            _tutorialManager = tutorialManager;
            _tutorialManager.Register(_tutorialStep);
        }

        public async UniTask<bool> AwaitForDecision()
        {
            Cleanup();
            InitButtons();

            _canvas.enabled = true;
            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            _canvas.enabled = false;
            return result;
        }

        private void Cleanup()
        {
            _catSelectionBtn.Cleanup();
            _dogSelectionBtn.Cleanup();
            _selectBtn.onClick.RemoveAllListeners();
        }

        private void InitButtons()
        {
            bool hasRace = RaceState.CurrentRace != Race.None;
            _selectBtn.gameObject.SetActive(hasRace);
            UpdatePointerVisibility(!hasRace);
            
            _selectBtn.onClick.AddListener(OnSelectionBtnClicked);
            _catSelectionBtn.Init( Race.Cat,  HandleRaceSelection, RaceState.CurrentRace == Race.Cat);
            _dogSelectionBtn.Init( Race.Dog,  HandleRaceSelection, RaceState.CurrentRace == Race.Dog);
        }

        private void UpdatePointerVisibility(bool isVisible)
        {
            foreach (var pointer in _decorPointers) 
                pointer.SetActive(isVisible);
        }
        
        private void HandleRaceSelection(Race selectedRace)
        {
            UpdatePointerVisibility(false);
            _tutorialStep.ShowStep();
            _catSelectionBtn.SetState(selectedRace == Race.Cat);
            _dogSelectionBtn.SetState(selectedRace == Race.Dog);
            _selectBtn.gameObject.SetActive(true);
        }

        private void OnSelectionBtnClicked()
        {
            _tutorialStep.CompleteStep();
            _audioService.PlayButtonSound();
            _persistentData.ChooseRace(_catSelectionBtn.IsOn ? Race.Cat : Race.Dog);
            _taskCompletion.TrySetResult(true);
        }
    }
}
