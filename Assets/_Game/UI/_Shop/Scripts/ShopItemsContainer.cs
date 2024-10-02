using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.UI.Factory;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Game.UI._Shop.Scripts
{
    public class ShopItemsContainer : MonoBehaviour
    {
        private const int FIXED_COLUMNS_COUNT = 3;

        [SerializeField] private Delimiter[] _delimiters;

        [FormerlySerializedAs("_type1Parent")] [SerializeField]
        private RectTransform _topContainer;

        [FormerlySerializedAs("_type2Parent")] [SerializeField]
        private RectTransform _middleContainer;

        [FormerlySerializedAs("_type3Parent")] [SerializeField]
        private RectTransform _bottomContainer;

        private readonly List<Plug> _plugs = new List<Plug>();

        private readonly List<CoinsBundleView> _coinsBundles = new List<CoinsBundleView>();
        private readonly List<GemsBundleView> _gemsBundles = new List<GemsBundleView>();
        private readonly List<SpeedOfferView> _speedOffers = new List<SpeedOfferView>();
        private readonly List<ProfitOfferView> _profitOffers = new List<ProfitOfferView>();
        private readonly List<AdsGemsPackView> _adsGemsPacks = new List<AdsGemsPackView>();
        private readonly List<FreeGemsPackView> _freeGemsPacks = new List<FreeGemsPackView>();

        private IUIFactory _uiFactory;
        private IMyLogger _logger;

        public void Construct(
            IUIFactory uiFactory,
            IMyLogger logger)
        {
            _uiFactory = uiFactory;
            _logger = logger;
        }

        public void UpdateDecorationElements()
        {
            AddPlugsIfNeeded(_coinsBundles.Count, _middleContainer);
            AddPlugsIfNeeded((_gemsBundles.Count + _adsGemsPacks.Count + _freeGemsPacks.Count), _bottomContainer);

            if (_coinsBundles.Count > 0) _delimiters[0].Show();
            if (_gemsBundles.Count > 0) _delimiters[1].Show();

            _logger.Log("Decoration elements updated");

            LayoutRebuilder.ForceRebuildLayoutImmediate(_topContainer);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_middleContainer);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_bottomContainer);
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

        public void Cleanup()
        {
            foreach (var bundle in _gemsBundles) bundle.Release();
            foreach (var bundle in _coinsBundles) bundle.Release();
            foreach (var offer in _speedOffers) offer.Release();
            foreach (var offer in _profitOffers) offer.Release();
            foreach (var gemsPack in _adsGemsPacks) gemsPack.Release();
            foreach (var gemsPack in _freeGemsPacks) gemsPack.Release();

            foreach (var plug in _plugs) plug.Release();

            _gemsBundles.Clear();
            _coinsBundles.Clear();
            _speedOffers.Clear();
            _profitOffers.Clear();
            _adsGemsPacks.Clear();
            _freeGemsPacks.Clear();

            _plugs.Clear();
            foreach (var delimiter in _delimiters) delimiter.Hide();
        }

        public CoinsBundleView SpawnCoinBundleView(int id)
        {
            CoinsBundleView view = _uiFactory.GetShopItem<CoinsBundleView>(id, _middleContainer);
            _coinsBundles.Add(view);
            return view;
        }

        public GemsBundleView SpawnGemsBundleView(int id)
        {
            GemsBundleView view = _uiFactory.GetShopItem<GemsBundleView>(id, _bottomContainer);
            _gemsBundles.Add(view);
            return view;
        }

        public SpeedOfferView SpawnSpeedOfferView(int id)
        {
            SpeedOfferView view = _uiFactory.GetShopItem<SpeedOfferView>(id, _topContainer);
            _speedOffers.Add(view);
            return view;
        }

        public ProfitOfferView SpawnProfitOfferView(int id)
        {
            ProfitOfferView view = _uiFactory.GetShopItem<ProfitOfferView>(id, _topContainer);
            _profitOffers.Add(view);
            return view;
        }

        public AdsGemsPackView SpawnAdsGemsPackView(int id)
        {
            AdsGemsPackView view = _uiFactory.GetShopItem<AdsGemsPackView>(id, _bottomContainer);
            _adsGemsPacks.Add(view);
            return view;
        }

        public FreeGemsPackView SpawnFreeGemsPackView(int id)
        {
            FreeGemsPackView view = _uiFactory.GetShopItem<FreeGemsPackView>(id, _bottomContainer);
            _freeGemsPacks.Add(view);
            return view;
        }

        public void Remove(CoinsBundleView view)
        {
            if (_coinsBundles.Contains(view))
            {
                view.Cleanup();
                view.Release();
                _coinsBundles.Remove(view);
            }
        }

        public void Remove(GemsBundleView view)
        {
            if (_gemsBundles.Contains(view))
            {
                view.Cleanup();
                view.Release();
                _gemsBundles.Remove(view);
            }
        }

        public void Remove(SpeedOfferView view)
        {
            if (_speedOffers.Contains(view))
            {
                view.Cleanup();
                view.Release();
                _speedOffers.Remove(view);
            }
        }

        public void Remove(ProfitOfferView view)
        {
            if (_profitOffers.Contains(view))
            {
                view.Cleanup();
                view.Release();
                _profitOffers.Remove(view);
            }
        }

        public void Remove(AdsGemsPackView view)
        {
            if (_adsGemsPacks.Contains(view))
            {
                view.Cleanup();
                view.Release();
                _adsGemsPacks.Remove(view);
            }
        }
        
        public void Remove(FreeGemsPackView view)
        {
            if (_freeGemsPacks.Contains(view))
            {
                view.Cleanup();
                view.Release();
                _freeGemsPacks.Remove(view);
            }
        }
    }
}