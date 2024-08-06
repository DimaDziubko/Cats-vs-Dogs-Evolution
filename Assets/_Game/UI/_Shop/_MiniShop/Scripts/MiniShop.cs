using System;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._Currencies;
using _Game.UI._Shop.Scripts;
using _Game.UI.Factory;
using _Game.Utils.Extensions;
using Assets._Game.Core._Logger;
using Assets._Game.Core.Services.Audio;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Game.UI._Shop._MiniShop.Scripts
{
    public class MiniShop : MonoBehaviour, IPointerDownHandler
    {
        public event Action Opened;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private MiniItemShopContainer _container;
        [SerializeField] private Button _exitButton;
        [SerializeField] private TMP_Text _coinsLabel;
        
        private IShopPresenter _shopPresenter;

        private UniTaskCompletionSource<bool> _taskCompletion;
        private IAudioService _audioService;
        private IUserContainer _userContainer;

        private IUserCurrenciesStateReadonly Currencies => _userContainer.State.Currencies;

        private float _price;
        
        public void Construct(
            Camera uICameraOverlay, 
            IAudioService audioService, 
            IUIFactory uiFactory, 
            IShopPresenter shopPresenter,
            IMyLogger logger,
            IUserContainer userContainer)
        {
            _canvas.worldCamera = uICameraOverlay;
            _shopPresenter = shopPresenter;
            _audioService = audioService;
            _userContainer = userContainer;

            _container.Construct(shopPresenter, uiFactory, audioService, logger);
        }

        public async UniTask<bool> ShowAndAwaitForDecision(float price)
        {
            _price = price;
            Show();
            _taskCompletion = new UniTaskCompletionSource<bool>();
            var result = await _taskCompletion.Task;
            Hide();
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
            Opened += _shopPresenter.OnShopOpened;
            _shopPresenter.ShopItemsUpdated += _container.UpdateShopItems;
            Currencies.CurrenciesChanged += OnCurrenciesChanged;
            OnCurrenciesChanged(_Currencies.Currencies.Coins, 0, CurrenciesSource.None);
            _exitButton.onClick.AddListener(OnCancelled);
        }

        private void Unsubscribe()
        {
            _exitButton.onClick.RemoveAllListeners();
            Opened -= _shopPresenter.OnShopOpened;
            Currencies.CurrenciesChanged -= OnCurrenciesChanged;
            _shopPresenter.ShopItemsUpdated -= _container.UpdateShopItems;
        }

        public void ForceHide()
        {
            _taskCompletion?.TrySetResult(true);
        }

        private void Hide()
        {
            Unsubscribe();
            _container.Cleanup();
            _canvas.enabled = false;
        }

        private void OnCancelled()
        {
            _audioService.PlayButtonSound();
            _taskCompletion.TrySetResult(true);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            _audioService.PlayButtonSound();
            _taskCompletion.TrySetResult(true);
        }

        private void OnCurrenciesChanged(Currencies type, double delta, CurrenciesSource source)
        {
            UpdateCoinsLabelColor();
            _coinsLabel.text = Currencies.Coins.FormatMoney();
        }

        private void UpdateCoinsLabelColor() => 
            _coinsLabel.color = Currencies.Coins < _price ? Color.red : Color.white;
        
    }
}