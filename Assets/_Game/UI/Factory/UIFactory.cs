using _Game.Core.Factory;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI._CardsGeneral._Cards.Scripts;
using _Game.UI._Shop.Scripts;
using UnityEngine;

namespace _Game.UI.Factory
{
    public enum ShopItemViewType
    {
        Type1,
        Type2, 
        Type3,
        Type4,
    }
    
    [CreateAssetMenu(fileName = "UI Factory", menuName = "Factories/UI")]
    public class UIFactory : GameObjectFactory, IUIFactory
    {
        [SerializeField] private ShopItem _shopItemType1, _shopItemType2, _shopItemType3, _shopItemType4;
        [SerializeField] private Plug _shopItemPlug;

        [SerializeField] private CardItemView cardItemView;
        [SerializeField] private TutorialPointerView _tutorialPointer;
        
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
                case ShopItemViewType.Type4:
                    item = CreateGameObjectInstance(_shopItemType4, parent);
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

        public CardItemView GetCard(Transform parent)
        {
            var instance = CreateGameObjectInstance(cardItemView, parent);
            if(instance != null)
                instance.OriginFactory = this;
            return instance;
        }

        public TutorialPointerView GetTutorialPointer(Transform parent)
        {
            var instance = CreateGameObjectInstance(_tutorialPointer, parent);
            if(instance != null)
                instance.OriginFactory = this;
            return instance;
        }

        public void Reclaim(ShopItem item) => 
            Destroy(item.gameObject);

        public void Reclaim(Plug item) => 
            Destroy(item.gameObject);
        
        public void Reclaim(CardItemView cardItemView) => 
            Destroy(cardItemView.gameObject);
            
        public void Reclaim(TutorialPointerView tutorialPointer) => 
            Destroy(tutorialPointer.gameObject);
    }

    public interface IUIFactory
    {
        ShopItem Get(ShopItemViewType type, Transform parent);
        Plug GetShopItemPlug(Transform parent);
        CardItemView GetCard(Transform parent);
        TutorialPointerView GetTutorialPointer(Transform parent);
        void Reclaim(Plug item);
        void Reclaim(ShopItem item);
        void Reclaim(CardItemView cardItemView);
        void Reclaim(TutorialPointerView cardItemView);
    }
}