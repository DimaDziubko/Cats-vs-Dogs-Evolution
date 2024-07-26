using System;
using _Game.UI.Factory;
using Assets._Game.Core.Services.Audio;
using Assets._Game.UI.Common.Header.Scripts;
using Assets._Game.UI.Common.Scripts;
using UnityEngine;
using Screen = _Game.UI._MainMenu.Scripts.Screen;

namespace _Game.UI._Shop.Scripts
{
    [RequireComponent(typeof(Canvas))]
    public class Shop : MonoBehaviour, IUIScreen
    {
        public event Action Opened;
        public Screen Screen => Screen.Shop;
        
        [SerializeField] private Canvas _canvas;
        [SerializeField] private ShopItemsContainer _container;
        
        private IHeader _header;
        private IShopPresenter _shopPresenter;


        public void Construct(
            Camera uICamera,
            IAudioService audioService,
            IHeader header,
            IUIFactory uiFactory,
            IShopPresenter shopPresenter)
        {
            _canvas.worldCamera = uICamera;
            _header = header;
            _shopPresenter = shopPresenter;
            _container.Construct(shopPresenter, uiFactory, audioService);
        }

        public void Show()
        {
            ShowName();

            Unsubscribe();
            Subscribe();

            _canvas.enabled = true;
            
            Opened?.Invoke();
        }

        public void Init()
        {
            _container.Init();
        }

        private void Subscribe()
        {
            Opened += _shopPresenter.OnShopOpened;
            _shopPresenter.ShopItemsUpdated += _container.UpdateShopItems;
        }

        private void Unsubscribe()
        {
            Opened -= _shopPresenter.OnShopOpened;
            _shopPresenter.ShopItemsUpdated -= _container.UpdateShopItems;
        }

        private void ShowName() => 
            _header.ShowWindowName(Screen.ToString());

        public void Hide()
        {
            Unsubscribe();
            _container.Cleanup();
            _canvas.enabled = false;
        }
    }
}