using _Game.Common;
using _Game.Gameplay.Vfx.Factory;
using Assets._Game.Common;

namespace Assets._Game.Gameplay.Vfx.Scripts
{
    public abstract class VfxEntity : GameBehaviour
    {
        public IVfxFactory OriginFactory { get; set; }

        public VfxType Type { get; set; }

        public override void Recycle()
        {
            OriginFactory.Reclaim(Type, this);
        }
    }
}