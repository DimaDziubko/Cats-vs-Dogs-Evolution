using _Game.Core.UserState;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Core.UserState;
using UnityEngine;

namespace Assets._Game.UI.Common.Header.Scripts
{
    public interface IHeader
    {
        void ShowWindowName(string windowName);
        void Construct(IUserCurrenciesStateReadonly currenciesState, IWorldCameraService cameraService);
        Vector3 CoinsWalletWorldPosition { get; }
    }
}
