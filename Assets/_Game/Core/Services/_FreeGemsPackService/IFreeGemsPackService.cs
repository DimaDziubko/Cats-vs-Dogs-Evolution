using System;
using _Game.Core.Services.IAP;

namespace _Game.Core.Services._FreeGemsPackService
{
    public interface IFreeGemsPackService
    {
        event Action FreeGemsPackUpdated;
        ProductDescription FreeGemsPack { get; }
        void OnFreeGemsPackBtnClicked();
    }
}