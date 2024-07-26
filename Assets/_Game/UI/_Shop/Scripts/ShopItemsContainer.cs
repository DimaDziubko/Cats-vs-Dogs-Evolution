using System.Collections.Generic;
using _Game.UI.Factory;
using UnityEngine;

namespace _Game.UI._Shop.Scripts
{
    public class ShopItemsContainer : MonoBehaviour
    {
        private const int FIXED_COLUMNS_COUNT = 3;
        
        [SerializeField] private Delimiter[] _delimiters;
        [SerializeField] private Transform _type1Parent, _type2Parent, _type3Parent;

        private readonly List<ShopItem> _itemsType1 = new List<ShopItem>();
        private readonly List<ShopItem> _itemsType2 = new List<ShopItem>();
        private readonly List<ShopItem> _itemsType3 = new List<ShopItem>();
        private readonly List<Plug> _plugs = new List<Plug>();

        private Dictionary<ShopItemViewType, Transform> _parents;
        
        private IUIFactory _uiFactory;

        public void Construct(IUIFactory uiFactory)
        {
            _parents = new Dictionary<ShopItemViewType, Transform>
            {
                {ShopItemViewType.Type1, _type1Parent},
                {ShopItemViewType.Type2, _type2Parent},
                {ShopItemViewType.Type3, _type3Parent}
            };
            _uiFactory = uiFactory;
        }

        public void UpdateShopItems(List<ShopItemModel> models)
        {
            Cleanup();
            
            foreach (var model in models)
            {
                var viewType = model.Description.Config.ItemViewType;
                ShopItem instance = _uiFactory.Get(viewType, _parents[viewType]);
                instance.Construct(model);
                
                switch (viewType)
                {
                    case ShopItemViewType.Type1:
                        _itemsType1.Add(instance);
                        break;
                    case ShopItemViewType.Type2:
                        _itemsType2.Add(instance);
                        break;
                    case ShopItemViewType.Type3:
                        _itemsType3.Add(instance);
                        break;
                }
            }

            AddPlugsIfNeeded(_itemsType2, _type2Parent);
            AddPlugsIfNeeded(_itemsType3, _type3Parent);
            
            if (_itemsType1.Count > 0) _delimiters[0].Show();
            if (_itemsType2.Count > 0) _delimiters[1].Show();
            
            Debug.Log("Shop items updated");
        }

        private void AddPlugsIfNeeded(List<ShopItem> items, Transform parent)
        {
            int remainder = items.Count % FIXED_COLUMNS_COUNT;
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

        public void Cleanup()
        {
            foreach (var item in _itemsType1) item.Release();
            foreach (var item in _itemsType2) item.Release();
            foreach (var item in _itemsType3) item.Release();
            foreach (var plug in _plugs) plug.Release();
            _itemsType1.Clear();
            _itemsType2.Clear();
            _itemsType3.Clear();
            _plugs.Clear();
            foreach (var delimiter in _delimiters) delimiter.Hide();
        }
    }
}