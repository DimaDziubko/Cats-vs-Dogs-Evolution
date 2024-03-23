using UnityEngine;

namespace _Game.Gameplay._BattleField.Scripts
{
    public interface ICoinSpawner
    {
        void SpawnLootCoin(Vector3 position, float amount);
    }
}