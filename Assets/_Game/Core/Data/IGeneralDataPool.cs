﻿using _Game.Core.Data.Age.Dynamic;
using _Game.Core.Data.Age.Static;
using _Game.Core.Data.Battle;
using _Game.Core.Data.Timeline.Static;

namespace _Game.Core.Data
{
    public interface IGeneralDataPool
    {
        TimelineStaticData TimelineStaticData { get; set; }
        AgeStaticData AgeStaticData { get; set; }
        AgeDynamicData AgeDynamicData { get; set; }
        BattleStaticData BattleStaticData { get; set; }

        void CleanupAgeData() =>
            AgeStaticData.Cleanup();

        void CleanupBattleData() => 
            BattleStaticData.Cleanup();
    }
}