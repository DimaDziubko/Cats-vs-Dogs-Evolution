using _Game.Common;
using _Game.Core._GameListenerComposite;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._BattleSpeed.Scripts;
using _Game.Gameplay._Weapon.Factory;
using _Game.Gameplay._Weapon.Scripts;
using Assets._Game.Core.Pause.Scripts;
using Assets._Game.Gameplay._BattleField.Scripts;
using Assets._Game.Gameplay._Weapon.Scripts;

namespace _Game.Creatives.Scripts
{
    public class CrProjectileSpawner : IShootProxy, IBattleSpeedListener, IPauseHandler
    {
        private readonly IProjectileFactory _projectileFactory;
        private readonly IVFXProxy _vfxProxy;
        private readonly IBattleSpeedManager _speedManager;

        private readonly GameBehaviourCollection _projectiles = new GameBehaviourCollection();
        private readonly IInteractionCache _cache;

        public CrProjectileSpawner(IProjectileFactory projectileFactory,
            IPauseManager pauseManager,
            IInteractionCache cache,
            IVFXProxy vfxProxy,
            IBattleSpeedManager speedManager)
        {
            _projectileFactory = projectileFactory;
            _cache = cache;
            _vfxProxy = vfxProxy;
            _speedManager = speedManager;

            pauseManager.AddHandler(this);
            speedManager.Register(this);
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

        void IShootProxy.Shoot(ShootData data)
        {
            Projectile projectile = _projectileFactory.Get(data.Faction, data.WeaponId);
            if(projectile == null) return;

            projectile.PrepareIntro(
                _vfxProxy,
                data.LaunchPosition, 
                data.Target, 
                _cache, 
                data.LaunchRotation,
                _speedManager.CurrentSpeedFactor);
            
            _projectiles.Add(projectile);
        }
        
        public void OnBattleSpeedFactorChanged(float speedFactor)
        {
            _projectiles.SetBattleSpeedFactor(speedFactor);
        }
    }
}