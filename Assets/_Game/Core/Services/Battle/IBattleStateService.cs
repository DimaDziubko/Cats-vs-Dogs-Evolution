using System;
using System.Threading.Tasks;
using _Game.Bundles.Bases.Scripts;
using _Game.Bundles.Units.Common.Scripts;
using _Game.Gameplay.Battle.Scripts;
using _Game.UI._StartBattleWindow.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Services.Battle
{
    public interface IBattleStateService
    {
        event Action<BattleData> BattlePrepared;
        event Action<BattleNavigationModel> BattleChange;
        int CurrentBattleIndex { get; }
        BattleData BattleData { get; }
        BattleNavigationModel NavigationModel { get; }
        bool IsBattlePrepared { get;}
        void MoveToNextBattle();
        void MoveToPreviousBattle();
        UnitData GetEnemy(UnitType type);
        BaseData GetForEnemyBase();
        UniTask Init();
    }
}