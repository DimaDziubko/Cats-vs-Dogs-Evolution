using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._GameInitializer;
using _Game.Core.Debugger;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.Currencies;
using Assets._Game.Core._Logger;
using Assets._Game.Core.UserState;

namespace Assets._Game.Core._UpgradesChecker
{
    public class UpgradesAvailabilityChecker : IUpgradesAvailabilityChecker, IDisposable
    {
        public event Action<NotificationData> Notify;

        private readonly IMyLogger _logger;
        private readonly IGameInitializer _gameInitializer;
        private readonly IUserContainer _userContainer;

        private IUserCurrenciesStateReadonly Currencies => _userContainer.State.Currencies;
        
        private IEnumerable<Screen> RelevantWindows { get; } = new List<Screen>()
        {
            Screen.Upgrades,
            Screen.Evolution,
            Screen.UpgradesAndEvolution
        };

        private readonly List<IUpgradeAvailabilityProvider> _upgradeProviders 
            = new List<IUpgradeAvailabilityProvider>();

        private readonly Dictionary<Screen, NotificationData> _data
            = new Dictionary<Screen, NotificationData>(3);

        public NotificationData GetNotificationData(Screen screen)
        {
            if (_data.TryGetValue(screen, out var data)) return data;
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
                        Screen = window,
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
            Currencies.CurrenciesChanged += OnCurrenciesChanged;
            UpdateData();
        }

        private void OnCurrenciesChanged(Currencies currencies, bool isPositive)
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

        public void MarkAsReviewed(Screen screen)
        {
            if (screen == Screen.UpgradesAndEvolution)
            {
                _data[screen].IsReviewed = CheckAllUpgradesReviewed();
                Notify?.Invoke(_data[screen]);
                return;
            }
            _data[screen].IsReviewed = true;
            Notify?.Invoke(_data[screen]);
        }

        private bool CheckAllUpgradesReviewed()
        {
            return (_data[Screen.Upgrades].IsReviewed && _data[Screen.Evolution].IsReviewed)
                   || (_data[Screen.Upgrades].IsReviewed && !_data[Screen.Evolution].IsAvailable)
                   || (!_data[Screen.Upgrades].IsAvailable && _data[Screen.Evolution].IsReviewed);
        }
    }
}