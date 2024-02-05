namespace _Game.Gameplay.GamePlayManager
{
    public sealed  class BeginGameManager : IBeginGameManager
    {
        private IBeginGameHandler _handler;

        public void Register(IBeginGameHandler handler)
        {
            if (_handler == null)
                _handler = handler;
        }

        public void UnRegister(IBeginGameHandler handler)
        {
            _handler = null;
        }

        public void TriggerBeginGame()
        {
           _handler.BeginGame();
        }
    }
}