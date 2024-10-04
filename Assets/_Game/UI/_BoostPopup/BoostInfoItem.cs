using _Game.Gameplay._Boosts.Scripts;
using _Game.Utils.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._BoostPopup
{
    public class BoostInfoItem : MonoBehaviour
    {
        [SerializeField] private Image _iconPlaceholder;
        [SerializeField] private TMP_Text _infoLabel;

        [SerializeField] private BoostType _boostType;

        public BoostType BoostType => _boostType;

        public void UpdateView(BoostInfoItemModel model, bool needToShowName)
        {
            gameObject.SetActive(model.IsActive);
            _iconPlaceholder.sprite = model.Icon;
            _infoLabel.text = needToShowName ? $"{model.Type.ToName()} x{model.DisplayValue}" : $"x{model.DisplayValue}";
        }
    }
}