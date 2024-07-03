using Assets._Game.Gameplay._Units.Scripts;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Assets._Game.Gameplay._Weapon.Scripts
{
    public class ShootData
    {
        public Faction Faction;
        public ITarget Target;
        public WeaponType WeaponType;
        public Vector3 LaunchPosition;
        public Quaternion LaunchRotation;
    }
}