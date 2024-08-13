using _Game.Gameplay._DailyTasks.Scripts;
using Assets._Game.Core.Services.Audio;
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

        private IDailyTaskPresenter _presenter;
        private IAudioService _audioService;

        public void Construct(
            IDailyTaskPresenter presenter,
            IAudioService audioService)
        {
            _presenter = presenter;
            _audioService = audioService;
            Show();
        }

        private void Show()
        {
            Subscribe();
            UpdateDailyTask(_presenter.CurrentDto);
            gameObject.SetActive(true);
            _animator.PlayAppearAnimation(null);
        }

        private void Hide()
        {
            Unsubscribe();
            gameObject.SetActive(false);
        }

        private void Subscribe()
        {
            _presenter.DailyTaskUpdated += UpdateDailyTask;
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void UpdateDailyTask(DailyTaskDto dto)
        {
            _button.interactable = dto.IsCompleted;
            _reward.text = dto.Reward;
            _progress.text = dto.Progress;
            _dailyInfo.text = dto.DailyInfo;
            if (dto.IsRunOut)
            {
                _animator.StopNotificationAnimation();
                Hide();
            }

            if (dto.IsCompleted)
            {
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
            _animator.StopNotificationAnimation();
            _audioService.PlayButtonSound();
            _animator.PlayRefreshAnimation(_presenter.CompleteTask);
        }
    }
}