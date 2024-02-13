using System.Collections.Generic;
using _Game.Bundles.Bases.Scripts;
using _Game.Bundles.Units.Common.Scripts;
using _Game.Gameplay._UnitBuilder.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Core.Services.Age.Scripts
{
    public interface IAgeStateService
    {
        UnitData GetPlayerUnit(UnitType type);
        UnitBuilderBtnData[] GetUnitBuilderData();
        Sprite GetCurrentFoodIcon { get; }
        BaseData GetForPlayerBase();
        UniTask Init();
    }
}
