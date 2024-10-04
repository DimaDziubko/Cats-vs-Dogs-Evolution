using _Game.Creatives.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.UI.Common.Scripts;
using Assets._Game.Gameplay._Units.Scripts;
using Assets._Game.UI.Common.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Creatives.Scripts
{
    [RequireComponent(typeof(Button), typeof(CustomButtonPressAnimator))]
    public class CrUnitBuilButton : MonoBehaviour
    {
        public UnitType UnitType;


        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _headerText;
        [SerializeField] private Image _unityIcon;
        [SerializeField] private TextMeshProUGUI _priceText;



        private ButtonState _state = ButtonState.Inactive;

        private float _priceToBuy;


        //Init
        public void OnCoinsChanged(float amount)
        {
            if (amount >= _priceToBuy)
            {
                _button.interactable = true;
                _priceText.color = Color.black;
            }
            else
            {
                _button.interactable = false;
                _priceText.color = Color.red;
            }
        }

        public void InitButtonData(string nameUnit, Sprite unitSprite, float price)
        {
            _priceToBuy = price;
            _headerText.text = nameUnit;
            _unityIcon.sprite = unitSprite;
            _priceText.text = price.ToString();
        }

        public void Initialize(CrUnitBuilderViewController crUnitBuilderViewController)
        {
            _button.onClick.AddListener(() => crUnitBuilderViewController.Build(UnitType, _priceToBuy));
        }

        private void OnDisable()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}