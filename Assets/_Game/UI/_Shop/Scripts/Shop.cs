using System;
using _Game.Core._UpgradesChecker;
using _Game.UI.Factory;
using _Game.UI.Header.Scripts;
using Assets._Game.Core._Logger;
using Assets._Game.Core.Services.Audio;
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
        [SerializeField] private Color _color;

        private IHeader _header;
        private IShopPresenter _shopPresenter;
        private IUpgradesAvailabilityChecker _checker;


        public void Construct(
            Camera uICamera,
            IAudioService audioService,
            IHeader header,
            IUIFactory uiFactory,
            IShopPresenter shopPresenter,
            IUpgradesAvailabilityChecker checker, 
            IMyLogger logger)
        {
            _canvas.worldCamera = uICamera;
            _header = header;
            _shopPresenter = shopPresenter;
            _checker = checker;
            _container.Construct(shopPresenter, uiFactory, audioService, logger);
        }

        public void Show()
        {
            ShowName();

            Unsubscribe();
            Subscribe();

            _canvas.enabled = true;
            
            Opened?.Invoke();
            _checker.MarkAsReviewed(Screen);
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
            _header.ShowWindowName(Screen.ToString(), _color);

        public void Hide()
        {
            Unsubscribe();
            _container.Cleanup();
            _canvas.enabled = false;
        }
    }
}