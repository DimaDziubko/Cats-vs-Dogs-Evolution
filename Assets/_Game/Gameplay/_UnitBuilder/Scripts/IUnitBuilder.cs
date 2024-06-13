using _Game.Gameplay._Units.Scripts;
using _Game.UI.Common.Scripts;

namespace _Game.Gameplay._UnitBuilder.Scripts
{
    public interface IUnitBuilder
    {
        public void StartBuilder();
        public void StopBuilder();
        void Build(UnitType type, int foodPrice);
        void OnButtonChangeState(ButtonState state);
    }
}