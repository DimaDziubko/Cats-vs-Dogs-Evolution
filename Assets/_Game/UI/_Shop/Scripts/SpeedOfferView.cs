using _Game.UI.Common.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Shop.Scripts
{
    public class SpeedOfferView : ShopItemView
    {
        [SerializeField] private Image _majorProductIconHolder;
        [SerializeField] private TMP_Text _descriptionLabel;
        [SerializeField] private TMP_Text _valueLabel;
        
        [SerializeField] private TransactionButton _button;
        
        public TransactionButton Button => _button;

        public void Init() => _button.Init();

        public void Cleanup() => _button.Cleanup();

        public void SetMajorIcon(Sprite sprite) => 
            _majorProductIconHolder.sprite = sprite;

        public void SetDescription(string description) => 
            _descriptionLabel.text = description;
        
        public void SetValue(string value) => 
            _valueLabel.text = value;
    }
}