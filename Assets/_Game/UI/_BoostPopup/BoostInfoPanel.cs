using _Game.Gameplay._Boosts.Scripts;
using TMPro;
using UnityEngine;

namespace _Game.UI._BoostPopup
{
    public class BoostInfoPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _boostInfoPanelNameLabel;
        [SerializeField] private BoostInfoItem[] _items;

        public void Enable() => 
            gameObject.SetActive(true);

        public void Disable() => 
            gameObject.SetActive(false);

        public void UpdateView(BoostPanelModel boost)
        {
            _boostInfoPanelNameLabel.text = boost.Name;
            foreach (var item in _items)
            {
                item.UpdateView(boost.BoostItemModels[item.BoostType], true);
            }
        }
    }
}