using System;
using _Game.Gameplay._Units.Scripts;
using _Game.UI.Common.Scripts;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Gameplay._UnitBuilder.Scripts
{
    [RequireComponent(typeof(Button), typeof(CustomButtonPressAnimator))]
    public class UnitBuildButton : MonoBehaviour
    {
        public event Action<ButtonState> ChangeState;
        
        [SerializeField] private GameObject _container;
        
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private Image _foodIconHolder;
        [SerializeField] private Image _unitIconHolder;

        private Button _button;

        [ShowInInspector] private UnitType _type;
        private ButtonState _state = ButtonState.Inactive;
        
        private readonly Color _affordableColor = new Color(1f, 1f, 1f);
        private readonly Color _expensiveColor = new Color(1f, 0.3f, 0f);

        private readonly Color _unitIconAffordableColor = new Color(1f, 1f, 1f, 1f);
        private readonly Color _unitIconExpensiveColor = new Color(1f, 1f, 1f, 0.3f);

        [ShowInInspector]
        private int _foodPrice = int.MaxValue;

        private bool _tempButtonState;

        //Scale animation
        [SerializeField] private RectTransform _transform;
        [SerializeField] private float _animationDuration = 1f;
        [SerializeField] private float _targetScale = 1.2f;
        [SerializeField] private float _initialScale = 1;
        public RectTransform RectTransform => _transform;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            Disable();
        }

        public void Initialize(IUnitBuilder unitBuilder, UnitBuilderBtnData data)
        {
            _container.SetActive(true);
            
            _type = data.Type;

            _foodPrice = data.FoodPrice;
            _priceText.text = data.FoodPrice.ToString();
            
            _foodIconHolder.sprite = data.FoodIcon;
            _unitIconHolder.sprite = data.UnitIcon;
            
            ChangeState -= unitBuilder.OnButtonChangeState;
            ChangeState += unitBuilder.OnButtonChangeState;
            _button.onClick.AddListener(() => unitBuilder.Build(_type, _foodPrice));
        }

        public void UpdateButtonState(int foodAmount)
        {
            bool canAfford = foodAmount >= _foodPrice;
            var newState = canAfford ? ButtonState.Active : ButtonState.Inactive;
            if (_state != newState)
            {
                _state = newState;
                ChangeState?.Invoke(_state);
            }

            if (canAfford && !_button.interactable)
            {
                DoScaleAnimation();
            }
            
            _button.interactable = _tempButtonState = canAfford;

            _priceText.color = canAfford ? _affordableColor : _expensiveColor;
            _unitIconHolder.color = canAfford ? _unitIconAffordableColor : _unitIconExpensiveColor;
        }

        public void Disable()
        {
            _button.interactable = false;
            _foodPrice = int.MaxValue;
        }

        private void DoScaleAnimation()
        {
            _transform.DOScale(_targetScale, _animationDuration * 0.5f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => _transform
                    .DOScale(_initialScale, _animationDuration * 0.5f)
                    .SetEase(Ease.InBack));
        }

        public void Hide()
        {
            _container.SetActive(false);
            Cleanup();
        }

        private void Cleanup() => 
            _button.onClick.RemoveAllListeners();

        public void SetPaused(in bool isPaused)
        {
            if(_tempButtonState == false) return;
            _button.interactable = !isPaused;
        }
    }
}