using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Debugger;
using _Game.UI._Hud;
using _Game.UI._MainMenu.Scripts;

namespace _Game.UI.Global
{
    public class UINotifier : IUINotifier
    {
        private IMyLogger _logger;

        public UINotifier(
            IMyLogger logger)
        {
            //debugger.UINotifier = this;
            _logger = logger;
        }

        private readonly List<IUIListener> _listeners = new List<IUIListener>();

        public void Register(IUIListener listener) => 
            _listeners.Add(listener);

        public void Unregister(IUIListener listener) => 
            _listeners.Remove(listener);

        void IUINotifier.OnScreenOpened(GameScreen screen)
        {
            foreach (var listener in _listeners)
            {
                listener.OnScreenOpened(screen);
            }
        }

        void IUINotifier.OnScreenClosed(GameScreen screen)
        {
            foreach (var listener in _listeners)
            {
                listener.OnScreenClosed(screen);
            }
        }
    }
}