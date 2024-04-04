using System;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Gameplay.Battle.Scripts;
using _Game.UI._StartBattleWindow.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Services.Battle
{
    public interface IBattleStateService
    {
        event Action<BattleData> BattlePrepared;
        event Action<BattleNavigationModel> NavigationUpdated;
        int CurrentBattleIndex { get; }
        BattleData BattleData { get; }
        void MoveToNextBattle();
        void MoveToPreviousBattle();
        UnitData GetEnemy(UnitType type);
        BaseData ForEnemyBase();
        UniTask Init();
        WeaponData ForWeapon(WeaponType type);
        void OpenNextBattle(int nextBattle);
        void OnStartBattleWindowOpened();
        UniTask ChangeRace();
    }
}