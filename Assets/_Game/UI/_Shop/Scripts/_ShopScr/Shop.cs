﻿using System;
using _Game.Core._Logger;
using _Game.Core._UpgradesChecker;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.Factory;
using _Game.UI.Header.Scripts;
using Assets._Game.UI.Common.Scripts;
using UnityEngine;

namespace _Game.UI._Shop.Scripts._ShopScr
{
    [RequireComponent(typeof(Canvas))]
    public class Shop : MonoBehaviour, IGameScreen
    {
        public event Action Opened;
        public GameScreen GameScreen => GameScreen.Shop;
        public ShopItemsContainer Container => _container;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private ShopItemsContainer _container;
        [SerializeField] private Color _color;

        private IHeader _header;
        private IShopPresenter _shopPresenter;
        private IUpgradesAvailabilityChecker _checker;


        public void Construct(
            Camera uICamera,
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
            _container.Construct(uiFactory, logger);
            shopPresenter.Shop = this;
            _canvas.enabled = false;
        }

        public void Show()
        {
            ShowName();

            Unsubscribe();
            Subscribe();
            
            Opened?.Invoke();
            _checker.MarkAsReviewed(GameScreen);
            
            _canvas.enabled = true;
        }
        
        private void Subscribe() => Opened += _shopPresenter.OnShopOpened;

        private void Unsubscribe() => Opened -= _shopPresenter.OnShopOpened;

        private void ShowName() => 
            _header.ShowScreenName(GameScreen.ToString(), _color);

        public void Hide()
        {
            _shopPresenter.OnShopClosed();
            Unsubscribe();
            _container.Cleanup();
            _canvas.enabled = false;
        }
    }
}