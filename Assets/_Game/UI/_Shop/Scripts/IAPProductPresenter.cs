using System;
using _Game.Core.Data;
using _Game.Core.Services.Audio;
using _Game.Core.Services.IAP;
using _Game.UI.Common.Scripts;
using _Game.UI.Factory;
using UnityEngine;

namespace _Game.UI._Shop.Scripts
{
    public class IAPProductPresenter : IProductPresenter
    {
        // public event Action StateChanged;
        //
        // public ButtonState ButtonState => ButtonState.Active;
        // public ShopItemViewType ViewType => _description.Config.ItemViewType;
        // public string Price => _description.Product.metadata.localizedPriceString;
        //
        // public string Info
        // {
        //     get
        //     {
        //         switch (_description.Config.ShopItemType)
        //         {
        //             case ShopItemType.x1_5:
        //                 return "x1.5";
        //             case ShopItemType.x2:
        //                 return "x2";
        //             case ShopItemType.Gems:
        //                 return String.Empty;
        //             case ShopItemType.Coins:
        //                 return String.Empty;
        //             default:
        //                 return String.Empty;
        //         }
        //     }
        // }
        //
        // public Sprite CurrencyIcon =>
        //     _generalDataPool.ShopItemStaticDataPool.ForType(_description.Config.Id).CurrencyIcon;
        // public Sprite MajorProductIcon =>
        //     _generalDataPool.ShopItemStaticDataPool.ForType(_description.Config.Id).MajorProductIcon;
        // public Sprite MinorProductIcon => 
        //     _generalDataPool.ShopItemStaticDataPool.ForType(_description.Config.Id).MinorProductIcon;
        // public string Quantity => _description.Config.Quantity.ToString();
        // public string Description => _description.Config.Description;
        //
        // private ProductDescription _description;
        // private readonly IIAPService _iapService;
        // private readonly IGeneralDataPool _generalDataPool;
        // private readonly IAudioService _audioService;
        //
        // public IAPProductPresenter(
        //     ProductDescription description, 
        //     IIAPService iapService,
        //     IGeneralDataPool generalDataPool,
        //     IAudioService audioService)
        // {
        //     _description = description;
        //     _iapService = iapService;
        //     _generalDataPool = generalDataPool;
        //     _audioService = audioService;
        // }
        //
        // void IProductPresenter.OnBuyButtonClicked()
        // {
        //     _iapService.StartPurchase(_description.Id);
        //     _audioService.PlayButtonSound();
        // }
    }
}