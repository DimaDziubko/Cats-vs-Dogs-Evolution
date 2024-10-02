using _Game.Core.Services.IAP;
using _Game.Temp;
using _Game.UI.Common.Scripts;
using _Game.UI.Factory;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Game.UI._Shop.Scripts
{
    public class ShopItemView : MonoBehaviour
    {
        public int Id => _id;

        [SerializeField] private int _id;

        public IUIFactory OriginFactory { get; set; }

        public void Release()
        {
            // _button.Click -= OnTransactionButtonClicked;
            // _button.InactiveClick -= OnInactiveButtonClicked;
            // _button.Cleanup();
            OriginFactory.Reclaim(this);
        }


        // [SerializeField] private Image _majorProductIconHolder;


        // [SerializeField] private Image _minorProductIconHolder;


        // [SerializeField] private TMP_Text _quantity;


        // [SerializeField] private TMP_Text _description;


        // [FormerlySerializedAs("_valueLabel")] [SerializeField] private TMP_Text _infoLabel;


        //


        // [SerializeField] private TransactionButton _button;

        //

        // private IProductPresenter _productPresenter;

        //

        // public void Construct(IProductPresenter productPresenter)

        // {

        //     _productPresenter = productPresenter;

        //

        //     if (_infoLabel != null)

        //     {

        //         _infoLabel.text = productPresenter.Info;

        //     }

        //     

        //     if(_majorProductIconHolder != null)

        //         _majorProductIconHolder.sprite = productPresenter.MajorProductIcon;

        //     if (_minorProductIconHolder != null)

        //         _minorProductIconHolder.sprite = productPresenter.MinorProductIcon;

        //     

        //     if (_quantity != null)

        //     {

        //         _quantity.text = productPresenter.Quantity;

        //     }

        //     

        //     if (_description != null)

        //     {

        //         _description.text = productPresenter.Description;

        //     }

        //

        //     string price = productPresenter.Price;

        //     Sprite icon = productPresenter.CurrencyIcon;

        //     ButtonState buttonState = productPresenter.ButtonState;

        //     

        //     _button.Construct(icon);

        //     _button.Init();

        //     _button.UpdateButtonState(buttonState, price);

        // }

        //

        // public void Init()

        // {

        //     _button.Init();

        //     _button.Click += OnTransactionButtonClicked;

        //     _button.InactiveClick += OnInactiveButtonClicked;

        // }

        //
        //
        // private void OnTransactionButtonClicked() => _productPresenter.OnBuyButtonClicked();
        //
        // private void OnInactiveButtonClicked() => GlobalEvents.RaiseOnInsufficientFunds();
    }
}