using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.UI.UpgradesAndEvolution.Scripts
{
    public class StatsInfoPanel : MonoBehaviour
    {
        [SerializeField] private StatInfoItem[]  _statInfoItems;

        public void UpdateView(Dictionary<StatType, StatInfoModel> statsInfo, bool isActive)
        {
            gameObject.SetActive(isActive);
            foreach (var item in _statInfoItems)
            {
                if (statsInfo.ContainsKey(item.StatType))
                {
                    item.UpdateView(statsInfo[item.StatType], false);
                }
            }
        }

        public void Cleanup()
        {
            foreach (var item in _statInfoItems)
            {
                item.Cleanup();
            }
        }
    }
}