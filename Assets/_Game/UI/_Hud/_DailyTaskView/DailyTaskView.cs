using _Game.Core._Logger;
using _Game.Gameplay._DailyTasks.Scripts;
using _Game.Gameplay._Tutorial.Scripts;
using Assets._Game.Core.Services.Audio;
using Assets._Game.Gameplay._Tutorial.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Hud._DailyTaskView
{
    public class DailyTaskView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _dailyInfo;
        [SerializeField] private TMP_Text _progress;
        [SerializeField] private TMP_Text _reward;
        [SerializeField] private DailyTaskViewAnimator _animator;
        [SerializeField] private TutorialStep _tutorialStep;
        
        private IDailyTaskPresenter _presenter;
        private IAudioService _audioService;
        private ITutorialManager _tutorialManager;

        private IMyLogger _logger;

        public void Construct(
            IDailyTaskPresenter presenter,
            IAudioService audioService,
            ITutorialManager tutorialManager,
            IMyLogger logger)
        {
            _presenter = presenter;
            _audioService = audioService;
            _tutorialManager = tutorialManager;
            _logger = logger;
        }

        public void Init()
        {
            _tutorialManager.Register(_tutorialStep);
            Subscribe();
            UpdateDailyTask(_presenter.CurrentDto);
            Show();
        }

        public void Show()
        {
            UpdateDailyTask(_presenter.CurrentDto);
            _animator.PlayAppearAnimation(null);
        }

        public void Hide()
        {
            _tutorialStep.CancelStep();
            gameObject.SetActive(false);
        }

        private void Subscribe()
        {
            _presenter.DailyTaskUpdated += UpdateDailyTask;
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void UpdateDailyTask(DailyTaskDto dto)
        {
            if (gameObject.activeInHierarchy == false)
            {
                gameObject.SetActive(true);
            }
            _button.interactable = dto.IsCompleted;
            _reward.text = dto.Reward;
            _progress.text = dto.Progress;
            _dailyInfo.text = dto.DailyInfo;
            if (!dto.IsUnlocked)
            {
                _animator.StopNotificationAnimation();
                Hide();
                return;
            }
            if (dto.IsRunOut)
            {
                _animator.StopNotificationAnimation();
                Hide();
                return;
            }

            if (dto.IsCompleted)
            {
                _tutorialStep.ShowStep();
                _animator.PlayNotificationAnimation();
            }
        }

        private void Unsubscribe()
        {
            _presenter.DailyTaskUpdated -= UpdateDailyTask;
            _button.onClick.RemoveAllListeners();
        }

        private void OnButtonClicked()
        {
            _tutorialStep.CompleteStep();
            _animator.StopNotificationAnimation();
            _audioService.PlayButtonSound();
            _animator.PlayRefreshAnimation(_presenter.CompleteTask);
        }

        public void Cleanup()
        {
            _tutorialManager.UnRegister(_tutorialStep);
            Unsubscribe();
        }
    }
}