using _Game.Common;
using _Game.Core.Pause.Scripts;
using _Game.Gameplay._BattleSpeed.Scripts;
using _Game.Gameplay._Weapon.Factory;
using _Game.Gameplay._Weapon.Scripts;

namespace _Game.Gameplay._BattleField.Scripts
{
    public class ProjectileSpawner : IShootProxy, IPauseHandler, IBattleSpeedHandler
    {
        private readonly IProjectileFactory _projectileFactory;
        private readonly IVFXProxy _vfxProxy;
        private readonly IBattleSpeedManager _speedManager;

        private readonly GameBehaviourCollection _projectiles = new GameBehaviourCollection();
        private readonly IInteractionCache _cache;

        public ProjectileSpawner(
            IProjectileFactory projectileFactory,
            IPauseManager pauseManager,
            IInteractionCache cache,
            IVFXProxy vfxProxy,
            IBattleSpeedManager speedManager)
        {
            _projectileFactory = projectileFactory;
            _cache = cache;
            _vfxProxy = vfxProxy;
            _speedManager = speedManager;
            
            pauseManager.Register(this);
            speedManager.Register(this);
        }

        public void GameUpdate()
        {
            _projectiles.GameUpdate();
        }

        void IPauseHandler.SetPaused(bool isPaused)
        {
            _projectiles.SetPaused(isPaused);
        }

        public void Cleanup()
        {
            _projectiles.Clear();
        }

        void IShootProxy.Shoot(ShootData data)
        {
            Projectile projectile = _projectileFactory.Get(data.Faction, data.WeaponType);

            projectile.PrepareIntro(
                _vfxProxy,
                data.LaunchPosition, 
                data.Target, 
                _cache,
                data.LaunchRotation,
                _speedManager.CurrentSpeedFactor);
            
            _projectiles.Add(projectile);
        }

        public void SetFactor(float speedFactor)
        {
            _projectiles.SetBattleSpeedFactor(speedFactor);
        }
    }
}