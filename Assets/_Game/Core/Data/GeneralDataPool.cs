﻿using _Game.Core._Logger;
using _Game.Core.Data.Age.Dynamic;
using _Game.Core.Data.Age.Static;
using _Game.Core.Data.Battle;
using _Game.Core.Data.Timeline.Static;
using _Game.Core.Debugger;

namespace _Game.Core.Data
{
    public class GeneralDataPool : IGeneralDataPool
    {
        public TimelineStaticData TimelineStaticData { get; set; }
        public AgeStaticData AgeStaticData { get; set; }
        public AgeDynamicData AgeDynamicData { get; set; }
        public BattleStaticData BattleStaticData { get; set; }

        public GeneralDataPool(
            IMyLogger logger)
        {

        }
    }
}