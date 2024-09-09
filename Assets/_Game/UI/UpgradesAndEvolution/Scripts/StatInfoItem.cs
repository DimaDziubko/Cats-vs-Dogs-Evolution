using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.UpgradesAndEvolution.Scripts
{
    public class StatInfoItem : MonoBehaviour
    {
        [SerializeField] private StatType _statType;
        [SerializeField] private Image _statIcon;
        [SerializeField] private TMP_Text _statFullValueLabel;
        [SerializeField] private TMP_Text _boostValueLabel;

        public StatType StatType => _statType;
    
        public void UpdateView(StatInfoModel model, bool showBoost)
        {
            _statIcon.sprite = model.StatIcon;
            _statFullValueLabel.text = model.StatFullValue;
            _boostValueLabel.text = $"(x{model.StatBoostValue})";
            _boostValueLabel.enabled = showBoost;
        }
    }
}