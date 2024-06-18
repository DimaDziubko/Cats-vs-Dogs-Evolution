using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Debugger;
using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.UI._MainMenu.Scripts;
namespace _Game.Core._UpgradesChecker
{
    public class UpgradesAvailabilityChecker : IUpgradesAvailabilityChecker, IDisposable
    {
        public event Action<NotificationData> Notify;

        private readonly IMyLogger _logger;
        private readonly IGameInitializer _gameInitializer;
        private readonly IUserContainer _userContainer;

        private IUserCurrenciesStateReadonly Currencies => _userContainer.State.Currencies;
        
        private IEnumerable<Window> RelevantWindows { get; } = new List<Window>()
        {
            Window.Upgrades,
            Window.Evolution,
            Window.UpgradesAndEvolution
        };

        private readonly List<IUpgradeAvailabilityProvider> _upgradeProviders 
            = new List<IUpgradeAvailabilityProvider>();

        private readonly Dictionary<Window, NotificationData> _data
            = new Dictionary<Window, NotificationData>(3);

        public NotificationData GetNotificationData(Window window)
        {
            if (_data.TryGetValue(window, out var data)) return data;
            else
            {
                _logger.Log("Data doesn't contains key {}");
                return null;
            }
        }

        public UpgradesAvailabilityChecker(
            IMyLogger logger,
            IGameInitializer gameInitializer,
            IMyDebugger debugger,
            IUserContainer userContainer)
        {
            _logger = logger;
            _gameInitializer = gameInitializer;
            _userContainer = userContainer;
            gameInitializer.OnPostInitialization += Init;
            debugger.NotificationData = _data;
        }

        public void Register(IUpgradeAvailabilityProvider provider) => 
            _upgradeProviders.Add(provider);

        public void UnRegister(IUpgradeAvailabilityProvider provider) => 
            _upgradeProviders.Remove(provider);

        private void UpdateData()
        {
            ResetWindowAvailability();
            AggregateAvailabilityStates();
            NotifyUpdatedStates();
        }

        private void ResetWindowAvailability()
        {
            foreach (var window in RelevantWindows)
            {
                if (!_data.ContainsKey(window))
                {
                    _data[window] = new NotificationData
                    {
                        Window = window,
                        IsAvailable = false,
                        IsReviewed = false
                    };
                }
                else
                {
                    _data[window].IsAvailable = false;
                }
            }
        }

        private void AggregateAvailabilityStates()
        {
            foreach (var provider in _upgradeProviders)
            {
                bool isAvailable = provider.IsAvailable;
                foreach (var window in provider.AffectedWindows)
                {
                    if (RelevantWindows.Contains(window))
                    {
                        _data[window].IsAvailable |= isAvailable;
                    }
                }
            }
        }

        private void NotifyUpdatedStates()
        {
            foreach (var window in RelevantWindows)
            {
                Notify?.Invoke(_data[window]);
            }
        }

        private void Init()
        {
            Currencies.CoinsChanged += OnCoinsChanged;
            UpdateData();
        }

        private void OnCoinsChanged(bool isPositive)
        {
            if (isPositive)
            {
                ResetReviewed();
                UpdateData();
            }
        }
        
        void IDisposable.Dispose() => 
            _gameInitializer.OnPostInitialization -= Init;

        private void ResetReviewed()
        {
            foreach (var item in _data)
            {
                item.Value.IsReviewed = false;
            }
        }

        public void MarkAsReviewed(Window window)
        {
            if (window == Window.UpgradesAndEvolution)
            {
                _data[window].IsReviewed = CheckAllUpgradesReviewed();
                Notify?.Invoke(_data[window]);
                return;
            }
            _data[window].IsReviewed = true;
            Notify?.Invoke(_data[window]);
        }

        private bool CheckAllUpgradesReviewed()
        {
            return (_data[Window.Upgrades].IsReviewed && _data[Window.Evolution].IsReviewed)
                   || (_data[Window.Upgrades].IsReviewed && !_data[Window.Evolution].IsAvailable)
                   || (!_data[Window.Upgrades].IsAvailable && _data[Window.Evolution].IsReviewed);
        }
    }
}