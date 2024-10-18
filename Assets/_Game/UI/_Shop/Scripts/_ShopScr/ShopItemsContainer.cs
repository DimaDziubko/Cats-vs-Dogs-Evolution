using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.UI._Shop.Scripts._DecorAndUtils;
using _Game.UI.Factory;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Shop.Scripts._ShopScr
{
    public enum ShopSubContainer
    {
        Top,
        Middle,
        Bottom
    }
    
    public class ShopItemsContainer : MonoBehaviour
    {
        private const int FIXED_COLUMNS_COUNT = 3;

        [SerializeField] private Delimiter[] _delimiters;

        [SerializeField] private RectTransform _topContainer;
        [SerializeField] private RectTransform _middleContainer;
        [SerializeField] private RectTransform _bottomContainer;

        private Dictionary<ShopSubContainer, RectTransform> _subContainers;
        
        private readonly List<Plug> _plugs = new List<Plug>();
        
        private readonly List<ShopItemView> _shopItems = new List<ShopItemView>();

        private IUIFactory _uiFactory;
        private IMyLogger _logger;

        public void Construct(
            IUIFactory uiFactory,
            IMyLogger logger)
        {
            _uiFactory = uiFactory;
            _logger = logger;
            
            _subContainers = new Dictionary<ShopSubContainer, RectTransform>()
            {
                {ShopSubContainer.Top, _topContainer},
                {ShopSubContainer.Middle, _middleContainer},
                {ShopSubContainer.Bottom, _bottomContainer},
            };
        }

        public void UpdateDecorationElements()
        {
            AddPlugsIfNeeded(GetCountInCategory(_middleContainer), _middleContainer);
            AddPlugsIfNeeded(GetCountInCategory(_bottomContainer), _bottomContainer);

            if (GetCountInCategory(_middleContainer) > 0) _delimiters[0].Show();
            if (GetCountInCategory(_bottomContainer) > 0) _delimiters[1].Show();

            LayoutRebuilder.ForceRebuildLayoutImmediate(_topContainer);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_middleContainer);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_bottomContainer);
        }

        public T SpawnShopItemView<T>(int id, ShopSubContainer subContainer) where T : ShopItemView
        {
            T view = _uiFactory.GetShopItem<T>(id, _subContainers[subContainer]);
            _shopItems.Add(view);
            return view;
        }

        public void Cleanup()
        {
            foreach (var item in _shopItems)
            {
                item.Release();
            }

            foreach (var plug in _plugs) plug.Release();

            _shopItems.Clear();
            _plugs.Clear();
            
            foreach (var delimiter in _delimiters) delimiter.Hide();
        }

        public void Remove(ShopItemView view)
        {
            if (_shopItems.Contains(view))
            {
                view.Cleanup();
                view.Release();
                _shopItems.Remove(view);
            }
        }

        private void AddPlugsIfNeeded(int amount, Transform parent)
        {
            int remainder = amount % FIXED_COLUMNS_COUNT;
            if (remainder != 0)
            {
                int plugsNeeded = FIXED_COLUMNS_COUNT - remainder;
                for (int i = 0; i < plugsNeeded; i++)
                {
                    var plug = _uiFactory.GetShopItemPlug(parent);
                    _plugs.Add(plug);
                }
            }
        }

        private int GetCountInCategory(RectTransform container) => 
            _shopItems.FindAll(item => item.transform.parent == container).Count;
    }
}