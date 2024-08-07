using Assets._Game.Gameplay._Units.Scripts;

namespace _Game.Core.Configs.Models
{
    public class EnemySpawnSequence
    {
        public int Id;
        public UnitType Type;
        public int Amount;
        public float Cooldown;
        public float StartDelay;
    }
}