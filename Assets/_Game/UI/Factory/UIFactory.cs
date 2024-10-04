using System.Collections.Generic;
using _Game.Core.Factory;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI._CardsGeneral._Cards.Scripts;
using _Game.UI._Shop.Scripts;
using UnityEngine;
using Zenject;

namespace _Game.UI.Factory
{
    [CreateAssetMenu(fileName = "UI Factory", menuName = "Factories/UI")]
    public class UIFactory : GameObjectFactory, IUIFactory, IInitializable
    {
        [SerializeField] private List<ShopItemView> _shopItemViews;
        private Dictionary<int, ShopItemView> _shopItemPrefabs;

        [SerializeField] private Plug _shopItemPlug;

        [SerializeField] private CardItemView cardItemView;

        [SerializeField] private TutorialPointerView _tutorialPointer;

        void IInitializable.Initialize()
        {
            _shopItemPrefabs = new Dictionary<int, ShopItemView>();
            foreach (var prefab in _shopItemViews)
            {
                int id = prefab.Id;
                if (!_shopItemPrefabs.ContainsKey(id))
                {
                    _shopItemPrefabs.Add(id, prefab);
                }
                else
                {
                    Debug.LogWarning($"Duplicate id {id} found in _shopItemPrefabs.");
                }
            }
        }
        
        public T GetShopItem<T>(int id, Transform parent) where T : ShopItemView
        {
            if (_shopItemPrefabs.TryGetValue(id, out var prefab))
            {
                var instance = CreateGameObjectInstance(prefab, parent) as T;
                if (instance != null)
                    instance.OriginFactory = this;
                return instance;
            }
            else
            {
                Debug.LogError($"Prefab for type {id} not found.");
                return null;
            }
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

        public void Reclaim(ShopItemView itemView) => 
            Destroy(itemView.gameObject);

        public void Reclaim(Plug item) => 
            Destroy(item.gameObject);

        public void Reclaim(CardItemView cardItemView) => 
            Destroy(cardItemView.gameObject);

        public void Reclaim(TutorialPointerView tutorialPointer) => 
            Destroy(tutorialPointer.gameObject);
    }

    public interface IUIFactory
    {
        Plug GetShopItemPlug(Transform parent);
        CardItemView GetCard(Transform parent);
        TutorialPointerView GetTutorialPointer(Transform parent);
        void Reclaim(Plug item);
        void Reclaim(ShopItemView itemView);
        void Reclaim(CardItemView cardItemView);
        void Reclaim(TutorialPointerView cardItemView);
        T GetShopItem<T>(int id, Transform parent) where T : ShopItemView;
    }
}