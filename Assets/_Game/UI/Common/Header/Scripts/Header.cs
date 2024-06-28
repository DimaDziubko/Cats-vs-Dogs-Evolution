using _Game.Core.Services.Camera;
using _Game.Core.UserState;
using _Game.UI.Common.Scripts;
using TMPro;
using UnityEngine;

namespace _Game.UI.Common.Header.Scripts
{
    public class Header : MonoBehaviour, IHeader
    {
        [SerializeField] private TMP_Text _windowNameLabel;
        [SerializeField] private CurrenciesUI _currenciesUI;

        public Vector3 CoinsWalletWorldPosition => _currenciesUI.CoinsWalletWorldPosition;

        public void Construct(
            IUserCurrenciesStateReadonly currenciesState,
            IWorldCameraService cameraService)
        {
            _currenciesUI.Construct(currenciesState, cameraService);
            _currenciesUI.Show();
        }
        
        public void ShowWindowName(string windowName)
        {
            _windowNameLabel.text = windowName;
        }

        public void OnDisable()
        {
            _currenciesUI.Hide();
        }
    }
}
