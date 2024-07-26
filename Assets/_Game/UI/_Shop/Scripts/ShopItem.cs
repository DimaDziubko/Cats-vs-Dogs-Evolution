using _Game.UI.Common.Scripts;
using _Game.UI.Factory;
using _Game.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Shop.Scripts
{
    public class ShopItem : MonoBehaviour
    {
        [SerializeField] private Image _majorProductIconHolder;
        [SerializeField] private Image _minorProductIconHolder;
        [SerializeField] private TMP_Text _quantity;
        [SerializeField] private TMP_Text _description;

        [SerializeField] private TransactionButton _button;
        public IUIFactory OriginFactory { get; set; }

        public void Release()
        {
            OriginFactory.Reclaim(this);
        }

        public void Construct(ShopItemModel model)
        {
            if(_majorProductIconHolder != null)
                _majorProductIconHolder.sprite = model.ItemStaticData.MajorProductIcon;
            if (_minorProductIconHolder != null)
                _minorProductIconHolder.sprite = model.ItemStaticData.MinorProductIcon;
            if (_quantity != null)
            {
                _quantity.text = model.Description.Config.Quantity.ToString();
            }
            if (_description != null)
            {
                _description.text = model.Description.Config.Description;
            }

            if (_button != null)
            {
                var price = model.Description.Id != Constants.ConfigKeys.MISSING_KEY 
                    ? model.Description.Product.metadata.localizedPriceString 
                    : model.Description.Config.Price.ToString();
                _button.Construct(model.ItemStaticData.CurrencyIcon);
                _button.Init();
                _button.UpdateButtonState(model.CanAfford, price);
            }
        }
    }
}