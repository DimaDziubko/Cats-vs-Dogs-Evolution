using _Game.Bundles.Units.Common.Scripts;
using _Game.Gameplay.UpgradesAndEvolution.Scripts;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.Common.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.UI.UpgradesAndEvolution.Upgrades.Scripts
{
    public class UpgradesWindow : MonoBehaviour, IUIWindow
    {
        public string Name => "Upgrades";
        
        [SerializeField] private Canvas _canvas;
        [SerializeField] private UpgradeUnitItem[] _unitItems;

        private IUpgradesAndEvolutionService _upgradesAndEvolutionService;
        private IHeader _header;


        public async UniTask Construct(
            IHeader header,
            IUpgradesAndEvolutionService upgradesAndEvolutionService)
        {
            _upgradesAndEvolutionService = upgradesAndEvolutionService;
            _header = header;
            
            InitItems();
            
            await UpdateUIElements();
        }

        private void InitItems()
        {
            for (int i = 0; i < _unitItems.Length; i++)
            {
                _unitItems[i].Setup((UnitType)i, HandleUnitItemClick);
            }
        }

        private async void HandleUnitItemClick(UnitType type)
        {
            _upgradesAndEvolutionService.PurchaseUnit(type);
            await UpdateUIElements();
        }
        
        private async UniTask UpdateUIElements()
        {
            await UpdateUnitItems();
        }

        private async UniTask UpdateUnitItems()
        {
            var models = await _upgradesAndEvolutionService.GetUpgradeItems();
            for (int i = 0; i < models.Length; i++)
            {
                _unitItems[i]
                    .UpdateUI(
                        models[i].IsBought,
                        models[i].CanAfford,
                        models[i].Price,
                        models[i].Icon,
                        models[i].Name);
            }
        }
        
        public void Show()
        {
            _canvas.enabled = true;
            _header.ShowWindowName(Name);
        }
        
        public void Hide()
        {
            _canvas.enabled = false;
        }
        
    }
}