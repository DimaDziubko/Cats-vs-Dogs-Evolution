using System;
using _Game.Bundles.Units.Common.Scripts;
using _Game.Core.Services.Battle;
using UnityEngine;

namespace _Game.Gameplay._UnitBuilder.Scripts
{
    public class UnitBuilder : MonoBehaviour
    {
        public event Action<UnitType, int> UnitBuildRequested;
        
        [SerializeField] private UnitBuildButton[] _buttons;

        private IBattleStateService _battleState;

        public void Construct(IBattleStateService battleState)
        {
            _battleState = battleState;
        }

        public void StartBuilder()
        {
            Cleanup();
            
            UpdateBuilderData();
            
            foreach (var button in _buttons)
            {
                button.Click += OnButtonClick;
            }
        }

        public void UpdateButtonsState(int foodAmount)
        {
            foreach (var button in _buttons)
            {
                button.UpdateButtonState(foodAmount);
            }
        }

        public void StopBuilder()
        {
            foreach (var button in _buttons)
            {
                button.Click -= OnButtonClick;
            }
        }

        private void UpdateBuilderData()
        {
            //TODO Init buttons
            var data = _battleState.GetUnitBuilderData();
            
            for (int i = 0; i < data.Count; i++)
            {
                _buttons[i].Show(
                    data[i].Type,
                    data[i].Food,
                    data[i].UnitIcon,
                    data[i].FoodPrice);
            }
            
        }

        private void OnButtonClick(UnitType type, int foodPrice)
        {
            UnitBuildRequested?.Invoke(type, foodPrice);
        }

        private void Cleanup()
        {
            foreach (var button in _buttons)
            {
                button.Hide();
            }
        }
    }
}