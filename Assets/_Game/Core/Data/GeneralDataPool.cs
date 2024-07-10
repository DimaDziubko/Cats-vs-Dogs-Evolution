using System.Collections.Generic;
using _Game.Core.Data.Age.Static;
using _Game.Core.Data.Battle;
using Assets._Game.Core._Logger;
using Assets._Game.Core.Data.Age.Dynamic;
using Assets._Game.Core.Data.Age.Static;
using Assets._Game.Core.Data.Timeline.Static;
using Assets._Game.Core.Debugger;
using Assets._Game.Gameplay._UnitBuilder.Scripts;
using Assets._Game.Gameplay._Units.Scripts;

namespace Assets._Game.Core.Data
{
    public class GeneralDataPool : IGeneralDataPool
    {
        public TimelineStaticData TimelineStaticData { get; set; }
        public AgeStaticData AgeStaticData { get; set; }
        public AgeDynamicData AgeDynamicData { get; set; }
        public BattleStaticData BattleStaticData { get; set; }
        
        
        public Dictionary<UnitType, UnitBuilderBtnStaticData> GetBuilderButtonsData() => AgeStaticData.GetBuilderButtonsData();
        public UnitBuilderBtnStaticData GetBuilderButtonData(UnitType type) => AgeStaticData.GetBuilderButtonData(type);

        public GeneralDataPool(
            IMyLogger logger,
            IMyDebugger debugger)
        {
            debugger.GeneralDataPool = this;
        }

        public UnitData GetUnitForAge(UnitType type) => AgeStaticData.ForUnit(type);

        public UnitData GetUnitForBattle(int battle, UnitType type) => BattleStaticData.ForUnit(battle, type);
    }
}