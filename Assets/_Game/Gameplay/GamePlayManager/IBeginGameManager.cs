namespace _Game.Gameplay.GamePlayManager
{
    public interface IBeginGameManager
    {
        void Register(IBeginGameHandler handler);
        void TriggerBeginGame();
        void UnRegister(IBeginGameHandler handler);
    }
}