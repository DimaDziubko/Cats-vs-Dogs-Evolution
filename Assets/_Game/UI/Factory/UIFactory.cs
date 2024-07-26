using _Game.Core.Factory;
using _Game.UI._Shop.Scripts;
using UnityEngine;

namespace _Game.UI.Factory
{
    public enum ShopItemViewType
    {
        Type1,
        Type2,
        Type3,
    }
    
    [CreateAssetMenu(fileName = "UI Factory", menuName = "Factories/UI")]
    public class UIFactory : GameObjectFactory, IUIFactory
    {
        [SerializeField] private ShopItem _shopItemType1, _shopItemType2, _shopItemType3;
        [SerializeField] private Plug _shopItemPlug;

        public ShopItem Get(ShopItemViewType type, Transform parent)
        {
            ShopItem item = null;
            switch (type)
            {
                case ShopItemViewType.Type1:
                    item = CreateGameObjectInstance(_shopItemType1, parent);
                    break;
                case ShopItemViewType.Type2:
                    item = CreateGameObjectInstance(_shopItemType2, parent);
                    break;
                case ShopItemViewType.Type3:
                    item = CreateGameObjectInstance(_shopItemType3, parent);
                    break;
            }
            
            if(item != null)
                item.OriginFactory = this;
            
            return item;
        }
        
        public Plug GetShopItemPlug(Transform parent)
        {
            var instance = CreateGameObjectInstance(_shopItemPlug, parent);
            if(instance != null)
                instance.OriginFactory = this;
            return instance;
        }

        public void Reclaim(ShopItem item)
        {
            Destroy(item.gameObject);
        }
        
        public void Reclaim(Plug item)
        {
            Destroy(item.gameObject);
        }
    }

    public interface IUIFactory
    {
        ShopItem Get(ShopItemViewType type, Transform parent);
        Plug GetShopItemPlug(Transform parent);
        void Reclaim(Plug item);
        void Reclaim(ShopItem item);
    }
}