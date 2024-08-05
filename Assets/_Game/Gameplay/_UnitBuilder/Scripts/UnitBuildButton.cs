using System;
using _Game.UI.Common.Scripts;
using Assets._Game.Gameplay._UnitBuilder.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.UI.Common.Scripts;
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

        public UnitType UnitType;

        [SerializeField] private GameObject _container;

        [SerializeField] private RectTransform _transform;
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private Image _foodIconHolder;
        [SerializeField] private Image _unitIconHolder;
        [SerializeField] private UnitBuilderBtnScaleAnimation _animation;
        
        private Button _button;

        private ButtonState _state = ButtonState.Inactive;

        private readonly Color _affordableColor = new Color(1f, 1f, 1f);
        private readonly Color _expensiveColor = new Color(1f, 0.3f, 0f);

        private readonly Color _unitIconAffordableColor = new Color(1f, 1f, 1f, 1f);
        private readonly Color _unitIconExpensiveColor = new Color(1f, 1f, 1f, 0.3f);

        private IUnitBuilder _unitBuilder;

        [ShowInInspector]
        private int _foodPrice = int.MaxValue;

        private bool _tempButtonState;

        public RectTransform RectTransform => _transform;

        private void Awake()
        {
            _button = GetComponent<Button>();
            Disable();
        }

        private void Show()
        {
            _container.SetActive(true);
        }

        public void Initialize(IUnitBuilder unitBuilder, UnitBuilderBtnModel model)
        {
            _unitBuilder = unitBuilder;
            
            if (!model.DynamicData.IsUnlocked)
            {
                Hide();
                return;
            }

            Show();

            UnitType = model.StaticData.Type;

            _foodPrice = model.StaticData.FoodPrice;
            if(_priceText != null) _priceText.text = _foodPrice.ToString();
            if(_foodIconHolder != null) _foodIconHolder.sprite = model.DynamicData.FoodIcon;
            if(_unitIconHolder != null) _unitIconHolder.sprite = model.StaticData.UnitIcon;

            ChangeState -= unitBuilder.OnButtonChangeState;
            ChangeState += unitBuilder.OnButtonChangeState;
            _button.onClick.AddListener(() => unitBuilder.Build(UnitType, _foodPrice));
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
                _animation.DoScaleAnimation();
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

        public void Hide()
        {
            _container.SetActive(false);
            Cleanup();
        }

        private void Cleanup()
        {
            ChangeState -= _unitBuilder.OnButtonChangeState;
            _button.onClick.RemoveAllListeners();
        }

        public void SetPaused(in bool isPaused)
        {
            if (_tempButtonState == false) return;
            _button.interactable = !isPaused;
        }
    }
}