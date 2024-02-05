using _Game.Gameplay.UpgradesAndEvolution.Scripts;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.Common.Scripts;
using UnityEngine;

namespace _Game.UI.UpgradesAndEvolution.Upgrades.Scripts
{
    public class UpgradesWindow : MonoBehaviour, IUIWindow
    {
        [SerializeField] private Canvas _canvas;

        [SerializeField] private UpgradeUnitItem[] _unitItems;

        private IUpgradesAndEvolutionService _upgradesAndEvolutionService;
        private IHeader _header;

        public string Name => "Upgrades";

        public void Construct(
            IHeader header,
            IUpgradesAndEvolutionService upgradesAndEvolutionService)
        {
            _upgradesAndEvolutionService = upgradesAndEvolutionService;
            _header = header;
            
            InitInitItems();
            
            UpdateUIElements();
        }

        private void InitInitItems()
        {
            for (int i = 0; i < _unitItems.Length; i++)
            {
                int index = i; 
                _unitItems[i].Setup(index, HandleUnitItemClick);
            }
        }

        private void HandleUnitItemClick(int unitIndex)
        {
            _upgradesAndEvolutionService.PurchaseUnit(unitIndex);
            UpdateUIElements();
        }
        
        private void UpdateUIElements()
        {
            UpdateUnitItems();
        }

        private void UpdateUnitItems()
        {
            var models = _upgradesAndEvolutionService.GetUpgradeItems();
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