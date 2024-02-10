using System;
using System.Collections.Generic;
using _Game.Bundles.Units.Common.Scripts;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay.Battle.Scripts;
using _Game.UI._StartBattleWindow.Scripts;

namespace _Game.Core.Services.Battle
{
    public interface IBattleStateService
    {
        event Action<BattleData> BattlePrepared;
        event Action<BattleNavigationModel> BattleChange;
        int CurrentBattleIndex { get; }
        BattleNavigationModel NavigationModel { get; }
        bool IsBattlePrepared { get;}
        void MoveToNextBattle();
        void MoveToPreviousBattle();
        UnitData GetEnemy(UnitType type);
        UnitData GetPlayerUnit(UnitType type);
        List<UnitBuilderBtnData> GetUnitBuilderData();
    }
}