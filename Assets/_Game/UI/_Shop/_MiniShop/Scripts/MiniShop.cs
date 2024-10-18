using System;
using _Game.Core._Logger;
using _Game.Core.Services.Audio;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._Currencies;
using _Game.UI.Factory;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Shop._MiniShop.Scripts
{
    public class MiniShop : MonoBehaviour
    {
        public event Action Opened;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private MiniItemShopContainer _container;
        [SerializeField] private Button[] _exitButtons;
        [SerializeField] private TMP_Text _coinsLabel;
        
        private IMiniShopPresenter _miniShopPresenter;

        private UniTaskCompletionSource<bool> _taskCompletion;
        private IAudioService _audioService;
        private IUserContainer _userContainer;

        private IUserCurrenciesStateReadonly Currencies => _userContainer.State.Currencies;
        public MiniItemShopContainer Container => _container;

        private float _price;
        
        public void Construct(
            Camera uICameraOverlay, 
            IAudioService audioService, 
            IUIFactory uiFactory, 
            IMiniShopPresenter miniShopPresenter,
            IMyLogger logger,
            IUserContainer userContainer)
        {
            _canvas.worldCamera = uICameraOverlay;
            _miniShopPresenter = miniShopPresenter;
            _audioService = audioService;
            _userContainer = userContainer;

            _miniShopPresenter.MiniShop = this;
            _container.Construct(uiFactory, logger);
        }

        public async UniTask<bool> ShowAndAwaitForDecision(float price)
        {
            _price = price;
            Show();
            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            return result;
        }

        private void Show()
        {
            Unsubscribe();
            Subscribe();

            _canvas.enabled = true;
            
            Opened?.Invoke();
        }
        
        private void Subscribe()
        {
            Opened += _miniShopPresenter.OnMiniShopOpened;
            Currencies.CurrenciesChanged += OnCurrenciesChanged;
            OnCurrenciesChanged(CurrencyType.Coins, 0, CurrenciesSource.None);
            foreach (var button in _exitButtons)
            {
                button.onClick.AddListener(OnCancelled);
            }
           
        }

        private void Unsubscribe()
        {
            Opened -= _miniShopPresenter.OnMiniShopOpened;
            Currencies.CurrenciesChanged -= OnCurrenciesChanged;
            foreach (var button in _exitButtons)
            {
                button.onClick.RemoveAllListeners();
            }
        }

        public void ForceHide()
        {
            _miniShopPresenter.OnMiniShopClosed();
            _taskCompletion?.TrySetResult(true);
        }
        private void OnCancelled()
        {
            _miniShopPresenter.OnMiniShopClosed();
            _audioService.PlayButtonSound();
            _taskCompletion.TrySetResult(true);
        }
        
        private void OnCurrenciesChanged(CurrencyType type, double delta, CurrenciesSource source)
        {
            UpdateCoinsLabelColor();
            _coinsLabel.text = Currencies.Coins.FormatMoney();
        }

        private void UpdateCoinsLabelColor() => 
            _coinsLabel.color = Currencies.Coins < _price ? Color.red : Color.white;

        public void Cleanup()
        {
            Unsubscribe();
            _container.Cleanup();
            _canvas.enabled = false;
        }
    }
}