using _Game.Core.DataPresenters.UnitUpgradePresenter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.UpgradesAndEvolution.Scripts
{
    public class UnitInfoItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timelineInfoLabel;
        [SerializeField] private Image _iconPlaceholder;
        [SerializeField] private StatInfoItem[] _statsInfoItems;

        public void UpdateView(WarriorInfoItemModel model, bool showTimelineInfoNumber)
        {
            _timelineInfoLabel.enabled = showTimelineInfoNumber;
            _timelineInfoLabel.text = model.TimelineNumberInfo;
            _iconPlaceholder.sprite = model.Icon;
            foreach (var item in _statsInfoItems)
            {
                if(model.StatInfoModels.TryGetValue(item.StatType, out StatInfoModel itemModel))
                {
                    item.UpdateView(itemModel, true);
                }
            }
        }
    }
}

