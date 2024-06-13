namespace _Game.Gameplay._Tutorial.Scripts
{
    public interface ITutorialManager
    {
        public void Register(ITutorialStep tutorialStep);
        public void UnRegister(ITutorialStep tutorialStep);
    }
}