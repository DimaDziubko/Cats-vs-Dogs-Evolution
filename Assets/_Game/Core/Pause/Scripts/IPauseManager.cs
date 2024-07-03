namespace Assets._Game.Core.Pause.Scripts
{
    public interface IPauseManager 
    {
        bool IsPaused { get; }
        void Register(IPauseHandler handler);
        public void UnRegister(IPauseHandler handler);
        void SetPaused(bool isPaused);
    }
}