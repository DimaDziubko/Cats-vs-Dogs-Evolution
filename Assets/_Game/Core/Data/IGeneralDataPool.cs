using _Game.Core.Data.Age.Dynamic;
using _Game.Core.Data.Age.Static;
using _Game.Core.Data.Battle;
using _Game.Core.Data.Timeline.Static;
using _Game.UI._Shop.Scripts;
using Assets._Game.Gameplay._UnitBuilder.Scripts;
using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Core.Data
{
    public interface IGeneralDataPool
    {
        TimelineStaticData TimelineStaticData { get; set; }
        AgeStaticData AgeStaticData { get; set; }
        AgeDynamicData AgeDynamicData { get; set; }
        BattleStaticData BattleStaticData { get; set; }
        DataPool<int, ShopItemStaticData> ShopItemStaticDataPool { get; set; }
        
        UnitBuilderBtnStaticData GetBuilderButtonData(UnitType type);

        void CleanupAgeData() =>
            AgeStaticData.Cleanup();

        void CleanupBattleData() => 
            BattleStaticData.Cleanup();
    }
}