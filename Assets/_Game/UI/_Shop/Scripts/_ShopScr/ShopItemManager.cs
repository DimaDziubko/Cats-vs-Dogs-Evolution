using System;
using System.Collections.Generic;
using System.Linq;
using _Game.UI._Shop.Scripts.Common;
using Zenject;

namespace _Game.UI._Shop.Scripts._ShopScr
{
    public class ShopItemManager<TModel, TView, TPresenter>
        where TModel : ShopItem
        where TView : ShopItemView
        where TPresenter : IProductPresenter, IDisposable
    {
        private readonly Dictionary<TModel, TPresenter> _presenters = new Dictionary<TModel, TPresenter>();
        private readonly IFactory<TModel, TView, TPresenter> _presenterFactory;
        private readonly Func<TModel, bool> _availabilityCheck;
        
        private ShopItemsContainer _shopItemsContainer;

        public ShopItemManager(
            ShopItemsContainer shopItemsContainer,
            IFactory<TModel, TView, TPresenter> presenterFactory,
            Func<TModel, bool> availabilityCheck)
        {
            _shopItemsContainer = shopItemsContainer;
            _presenterFactory = presenterFactory;
            _availabilityCheck = availabilityCheck;
        }

        public void AddItem(TModel model, ShopSubContainer container)
        {
            if (!_presenters.ContainsKey(model))
            {
                TView view = _shopItemsContainer.SpawnShopItemView<TView>(model.ShopItemViewId, container);
                TPresenter presenter = _presenterFactory.Create(model, view);
                _presenters.Add(model, presenter);
                presenter.Initialize();
                view.Init();
            }
        }

        public void RemoveItem(TModel model)
        {
            if (_presenters.ContainsKey(model))
            {
                var presenter = _presenters[model];
                presenter.Dispose();
                _shopItemsContainer.Remove(presenter.View);
                _presenters.Remove(model);
            }
        }

        public void ClearItems()
        {
            foreach (var pair in _presenters)
            {
                pair.Value.Dispose();
                _shopItemsContainer.Remove(pair.Value.View);
            }

            _presenters.Clear();
        }

        public bool IsAnyItemAvailable()
        {
            return _presenters.Keys.Any(_availabilityCheck);
        }

        public void SetShopContainer(ShopItemsContainer shopContainer) => 
            _shopItemsContainer = shopContainer;
    }
}