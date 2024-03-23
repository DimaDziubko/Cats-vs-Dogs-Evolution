using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using UnityEngine;

namespace _Game.Gameplay._BattleField.Scripts
{
    public interface IShootProxy
    {
        void Shoot(ShootData shootData);
    }
}