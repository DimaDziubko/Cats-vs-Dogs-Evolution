using System;
using _Game.Bundles.Units.Common.Scripts;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Gameplay._UnitBuilder.Scripts
{
    [RequireComponent(typeof(Button))]
    public class UnitBuildButton : MonoBehaviour
    {
        public event Action<UnitType, int> Click;

        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private Image _foodIconHolder;
        [SerializeField] private Image _unitIconHolder;

        private Button _button;
        
        [ShowInInspector] private UnitType _type;

        private readonly Color _affordableColor = new Color(1f, 1f, 1f); 
        private readonly Color _expensiveColor = new Color(1f, 0.3f, 0f);

        private int _foodPrice;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
        }
        
        public void UpdateButtonState(int foodAmount)
        {
            bool canAfford = foodAmount >= _foodPrice;
            
            //TODO play animation
            
            _button.interactable = canAfford;
            _priceText.color = canAfford ? _affordableColor : _expensiveColor;
        }

        public void Show(UnitType type, Sprite food, Sprite unitIcon, int foodPrice)
        {
            _type = type;
            
            _foodPrice = foodPrice;
            
            gameObject.SetActive(true);
            _foodIconHolder.sprite = food;
            _unitIconHolder.sprite = unitIcon;
            _priceText.text = foodPrice.ToString();
            
            _button.onClick.AddListener(() => Click?.Invoke(_type, foodPrice));
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            Cleanup();
        }
        
        private void OnDisable()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}