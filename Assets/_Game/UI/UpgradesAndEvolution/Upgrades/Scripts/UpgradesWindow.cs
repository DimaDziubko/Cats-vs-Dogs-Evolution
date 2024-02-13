using System.Collections.Generic;
using _Game.Bundles.Units.Common.Scripts;
using _Game.Core.Services.Upgrades.Scripts;
using _Game.UI.Common.Header.Scripts;
using _Game.UI.Common.Scripts;
using UnityEngine;

namespace _Game.UI.UpgradesAndEvolution.Upgrades.Scripts
{
    public class UpgradesWindow : MonoBehaviour, IUIWindow
    {
        public string Name => "Upgrades";
        
        [SerializeField] private Canvas _canvas;
        [SerializeField] private UpgradeUnitItem[] _unitItems;

        private IUpgradesService _upgradesService;
        private IHeader _header;


        public void Construct(
            IHeader header,
            IUpgradesService upgradesService)
        {
            _upgradesService = upgradesService;
            _header = header;
            
            InitItems();
            UpdateUIElements();
        }

        private void InitItems()
        {
            foreach (var unitItem in _unitItems)
            {
                unitItem.Init();
                unitItem.Upgrade += HandleUnitItemUpgrade;
            }
        }

        private void HandleUnitItemUpgrade(UnitType type)
        {
            _upgradesService.PurchaseUnit(type);
        }
        
        private void UpdateUIElements()
        {
            //TODO Check
            var models = _upgradesService.GetUpgradeUnitItems();
            UpdateUnitUnitItems(models);
        }

        private void UpdateUnitUnitItems(List<UpgradeUnitItemModel> models)
        {
            for (int i = 0; i < models.Count; i++)
            {
                _unitItems[i]
                    .UpdateUI(
                        models[i].Type,
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
            _upgradesService.UpgradeUnitItemsUpdated += UpdateUnitUnitItems;
        }
        
        public void Hide()
        {
            _canvas.enabled = false;
            _upgradesService.UpgradeUnitItemsUpdated -= UpdateUnitUnitItems;
            
            foreach (var item in _unitItems)
            {
                item.Upgrade -= HandleUnitItemUpgrade;
                item.Cleanup();
            }
        }
        
    }
}