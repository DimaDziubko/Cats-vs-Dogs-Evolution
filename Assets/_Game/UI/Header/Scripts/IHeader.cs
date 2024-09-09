using _Game.Core.UserState._State;
using Assets._Game.Core.Services.Camera;
using UnityEngine;

namespace _Game.UI.Header.Scripts
{
    public interface IHeader
    {
        void ShowScreenName(string windowName, Color color);
        void Construct(IUserCurrenciesStateReadonly currenciesState, IWorldCameraService cameraService);
        Vector3 CoinsWalletWorldPosition { get; }
    }
}
