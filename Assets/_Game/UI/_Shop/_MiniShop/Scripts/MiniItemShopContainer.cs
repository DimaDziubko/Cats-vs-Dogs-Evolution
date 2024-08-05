using System.Collections.Generic;
using System.Linq;
using _Game.UI._Shop.Scripts;
using _Game.UI.Factory;
using _Game.Utils;
using Assets._Game.Core._Logger;
using Assets._Game.Core.Services.Audio;
using UnityEngine;

namespace _Game.UI._Shop._MiniShop.Scripts
{
    public class MiniItemShopContainer : MonoBehaviour
    {
        [SerializeField] private Transform _parent;
        
        private readonly List<ShopItem> _items = new List<ShopItem>();
        
        private IShopPresenter _shopPresenter;
        private IUIFactory _uiFactory;
        private IAudioService _audioService;
        private IMyLogger _logger;

        public void Construct(
            IShopPresenter shopPresenter, 
            IUIFactory uiFactory,
            IAudioService audioService,
            IMyLogger logger)
        {
            _shopPresenter = shopPresenter;
            _uiFactory = uiFactory;
            _audioService = audioService;
            _logger = logger;
        }

        private void Init()
        {
            foreach (var item in _items)
            {
                item.Init();
            }
        }
        
        public void UpdateShopItems(List<ShopItemModel> models)
        {
            Cleanup();
            
            var relevantModels = models.Where(x 
                => x.Description.Id == Constants.ConfigKeys.MISSING_KEY);
            
            foreach (var model in relevantModels)
            {
                ShopItemViewType viewType = ShopItemViewType.Type4;
                ShopItem instance = _uiFactory.Get(viewType, _parent);
                instance.Construct(_shopPresenter, model, _audioService);
                
                _items.Add(instance);
            }
            
            _logger.Log("Mini shop items updated");
            Init();
        }
        
        public void Cleanup()
        {
            foreach (var item in _items) item.Release();
            _items.Clear();
        }
    }
}