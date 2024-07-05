using UnityEngine;

namespace Assets._Game.Gameplay._BattleField.Scripts
{
    public interface ICoinSpawner
    {
        void SpawnLootCoin(Vector3 position, float amount);
    }
}