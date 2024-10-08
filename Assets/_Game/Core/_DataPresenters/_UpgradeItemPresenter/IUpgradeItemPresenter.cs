﻿using System;
using _Game.UI.UpgradesAndEvolution.Upgrades.Scripts;

namespace _Game.Core._DataPresenters._UpgradeItemPresenter
{
    public interface IUpgradeItemPresenter 
    {
        event Action<UpgradeItemModel> UpgradeItemUpdated;
        void UpgradeItem(UpgradeItemType type, float price);
        void OnUpgradesScreenOpened();
    }
}