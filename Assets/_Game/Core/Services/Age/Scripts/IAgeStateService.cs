using System;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Services.Age.Scripts
{
    public interface IAgeStateService
    {
        UnitData GetPlayerUnit(UnitType type);
        Sprite GetCurrentFoodIcon { get; }
        BaseData GetForPlayerBase();
        UniTask Init();
        WeaponData ForWeapon(WeaponType type);
        event Action<BaseData> BaseDataUpdated;
        event Action<UnitBuilderBtnData[]> BuilderDataUpdated;
        event Action AgeUpdated;
        void OnBuilderStarted();
    }
}
