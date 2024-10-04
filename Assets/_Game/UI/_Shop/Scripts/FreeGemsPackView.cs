using _Game.UI.Common.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Shop.Scripts
{
    public class FreeGemsPackView : ShopItemView
    {
        [SerializeField] private Image _majorProductIconHolder;
        [SerializeField] private Image _minorProductIconHolder;
        [SerializeField] private TMP_Text _quantityLabel;
        
        [SerializeField] private TMP_Text _timerLabel;
        [SerializeField] private GameObject _timerView;

        [SerializeField] private TransactionButton _button;
        
        public TransactionButton Button => _button;

        public void Init() => _button.Init();

        public void Cleanup() => _button.Cleanup();

        public void SetMajorIcon(Sprite sprite) => 
            _majorProductIconHolder.sprite = sprite;

        public void SetMinorIcon(Sprite sprite) => 
            _minorProductIconHolder.sprite = sprite;

        public void SetQuantity(string quantity) => 
            _quantityLabel.text = quantity;

        public void SetActiveTimerView(bool isActive) => _timerView.SetActive(isActive);
        public void UpdateTimer(string remainingTime) => _timerLabel.text = remainingTime;
    }
}