using _Game.Core.Services.Camera;
using _Game.Core.UserState;
using UnityEngine;

namespace _Game.UI.Common.Header.Scripts
{
    public interface IHeader
    {
        void ShowWindowName(string windowName);
        void ShowWallet(IUserCurrenciesStateReadonly currenciesState, IWorldCameraService cameraService);
        Vector3 CoinsWalletWorldPosition { get; }
    }
}
