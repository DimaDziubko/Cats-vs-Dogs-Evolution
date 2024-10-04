using _Game.Core.Services.Camera;
using _Game.Core.UserState;
using _Game.Core.UserState._State;
using _Game.UI._Currencies;
using TMPro;
using UnityEngine;

namespace _Game.UI.Header.Scripts
{
    public class Header : MonoBehaviour, IHeader
    {
        [SerializeField] private TMP_Text _windowNameLabel;
        [SerializeField] private CurrenciesUI _currenciesUI;

        public Vector3 CoinsWalletWorldPosition => _currenciesUI.CoinsWalletWorldPosition;
        //Util
        public void SetActive(bool isActive) => gameObject.SetActive(isActive);
        public Vector3 GemsWalletWorldPosition => _currenciesUI.GemsWalletWorldPosition;

        public void Construct(
            IUserCurrenciesStateReadonly currenciesState,
            IWorldCameraService cameraService)
        {
            _currenciesUI.Construct(currenciesState, cameraService);
            _currenciesUI.Show();
        }
        
        public void ShowScreenName(string windowName, Color color)
        {
            _windowNameLabel.text = windowName;
            _windowNameLabel.color = color;
        }

        public void OnDisable()
        {
            _currenciesUI.Hide();
        }
    }
}
