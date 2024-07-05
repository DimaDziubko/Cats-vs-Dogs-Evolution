using Assets._Game.Core.Data.Age.Dynamic;
using Assets._Game.Core.Data.Age.Static;
using Assets._Game.Core.Data.Battle;
using Assets._Game.Core.Data.Timeline.Static;
using Assets._Game.Gameplay._UnitBuilder.Scripts;
using Assets._Game.Gameplay._Units.Scripts;

namespace Assets._Game.Core.Data
{
    public interface IGeneralDataPool
    {
        TimelineStaticData TimelineStaticData { get; set; }
        AgeStaticData AgeStaticData { get; set; }
        AgeDynamicData AgeDynamicData { get; set; }
        BattleStaticData BattleStaticData { get; set; }
        UnitBuilderBtnStaticData GetBuilderButtonData(UnitType type);

        void CleanupAgeData() =>
            AgeStaticData.Cleanup();

        void CleanupBattleData() => 
            BattleStaticData.Cleanup();
    }
}