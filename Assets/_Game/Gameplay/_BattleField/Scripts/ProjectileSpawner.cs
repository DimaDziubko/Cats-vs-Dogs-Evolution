using _Game.Gameplay._BattleSpeed.Scripts;
using _Game.Gameplay._Weapon.Factory;
using _Game.Gameplay._Weapon.Scripts;
using Assets._Game.Common;
using Assets._Game.Gameplay._BattleField.Scripts;
using Assets._Game.Gameplay._Weapon.Scripts;

namespace _Game.Gameplay._BattleField.Scripts
{
    public class ProjectileSpawner : IShootProxy
    {
        private readonly IProjectileFactory _projectileFactory;
        private readonly IVFXProxy _vfxProxy;
        private readonly IBattleSpeedManager _battleSpeedManager;

        private readonly GameBehaviourCollection _projectiles = new GameBehaviourCollection();
        private readonly IInteractionCache _cache;

        public ProjectileSpawner(
            IProjectileFactory projectileFactory,
            IInteractionCache cache,
            IVFXProxy vfxProxy,
            IBattleSpeedManager battleSpeedManager)
        {
            _projectileFactory = projectileFactory;
            _cache = cache;
            _vfxProxy = vfxProxy;
            _battleSpeedManager = battleSpeedManager;
        }

        public void GameUpdate(float deltaTime)
        {
            _projectiles.GameUpdate(deltaTime);
        }

        public void SetPaused(bool isPaused)
        {
            _projectiles.SetPaused(isPaused);
        }

        public void Cleanup()
        {
            _projectiles.Clear();
        }

        async void  IShootProxy.Shoot(ShootData data)
        {
            Projectile projectile = await _projectileFactory.GetAsync(data.Faction, data.WeaponId);
            if(projectile == null) return;

            projectile.PrepareIntro(
                _vfxProxy,
                data.LaunchPosition, 
                data.Target, 
                _cache,
                data.LaunchRotation,
                _battleSpeedManager.CurrentSpeedFactor);
            
            _projectiles.Add(projectile);
        }

        public void SetSpeedFactor(float speedFactor) => _projectiles.SetBattleSpeedFactor(speedFactor);
    }
}