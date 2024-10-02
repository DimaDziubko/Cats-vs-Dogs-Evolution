using System.Collections.Generic;
using _Game.UI._Shop.Scripts;

namespace _Game.Core.Services._AdsGemsPackService
{
    public interface IAdsGemsPackService
    {
        Dictionary<int, AdsGemsPack> GetAdsGemsPacks();
        void OnAdsGemsPackBtnClicked(AdsGemsPack pack);
    }
}