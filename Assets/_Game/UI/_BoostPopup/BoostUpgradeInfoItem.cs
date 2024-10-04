using _Game.Gameplay._Boosts.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._BoostPopup
{
    public class BoostUpgradeInfoItem : MonoBehaviour
    {
        [SerializeField] private Image _iconPlaceholder;
        [SerializeField] private TMP_Text _valueLabel;
        [SerializeField] private TMP_Text _deltaValueLabel;

        [SerializeField] private BoostType _boostType;

        public BoostType BoostType => _boostType;

        public void Enable() => gameObject.SetActive(true);

        public void Disable() => gameObject.SetActive(false);

        public void UpdateView(BoostUpgradeInfoItemModel model)
        {
            _iconPlaceholder.sprite = model.Icon;
            _valueLabel.text = model.DisplayValue;
            _deltaValueLabel.text = model.Delta;
            if(model.IsUpgraded) Enable();
            else
            {
                Disable();
            }
            
            Debug.Log($"UPGRADE CARD: TYPE {model.Type} IS UPGRADED: {model.IsUpgraded}");
        }
    }
}