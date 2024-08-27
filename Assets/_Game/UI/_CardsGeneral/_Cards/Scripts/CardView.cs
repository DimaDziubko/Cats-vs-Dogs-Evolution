using _Game.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Slider _progressBar;
        [SerializeField] private TMP_Text _progressLabel;
        [SerializeField] private TMP_Text _levelLabel;
        [SerializeField] private Button _upgradeBtn;
        [SerializeField] private GameObject _newNotification;
        [SerializeField] private Image _colorIdentifier;
        [SerializeField] private GameObject _upgradeNitifier;

        public void Init()
        {
            Unsubscribe();
            Subscribe();
        }

        public void UpdateView(CardModel model)
        {
            _icon.sprite = model.Icon;
            _progressLabel.text = model.Progress;
            _levelLabel.text = model.Level;
            _progressBar.value = model.ProgressValue;
            _newNotification.SetActive(model.Config.IsNew);
            _upgradeNitifier.SetActive(model.ProgressValue - 1 <= Constants.ComparisonThreshold.MONEY_EPSILON);
            _colorIdentifier.material.color = model.Config.ColorIdentifier;
        }

        private void Subscribe()
        {
            _upgradeBtn.onClick.AddListener(OnCardClicked);
        }

        private void Unsubscribe()
        {
            _upgradeBtn.onClick.RemoveAllListeners();
        }

        private void OnCardClicked()
        {
            //TODO Implement later
        }

        public void Release()
        {
            //TODO Implement later
                    
        }
    }
}
