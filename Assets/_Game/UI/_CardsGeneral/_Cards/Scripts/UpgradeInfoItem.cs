using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class UpgradeInfoItem : MonoBehaviour
    {
        [SerializeField] private Image _boostIconPlaceholder;
        [SerializeField] private TMP_Text _boostNameLabel;
        [SerializeField] private TMP_Text _currentBoostValueLabel;
        [SerializeField] private TMP_Text _nextBoostValueLabel;

        public void Enable()
        {
            gameObject.SetActive(true);
        }
        
        public void Disable()
        {
            gameObject.SetActive(false);
        }
        
        public void UpdateView(BoostItemModel model)
        {
            _boostIconPlaceholder.sprite = model.BoostIcon;
            _boostNameLabel.text = model.BoostName;
            _currentBoostValueLabel.text = model.CurrentValue;
            _nextBoostValueLabel.text = model.NextValue;
        }
    }
}