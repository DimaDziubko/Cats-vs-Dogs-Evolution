using System.Collections.Generic;
using _Game.UI.UpgradesAndEvolution.Scripts;
using UnityEngine;

namespace _Game.Core.DataPresenters.UnitUpgradePresenter
{
    public class WarriorInfoItemModel
    {
        public string TimelineNumberInfo;
        public Sprite Icon;
        public Dictionary<StatType, StatInfoModel> StatInfoModels;
    }
}