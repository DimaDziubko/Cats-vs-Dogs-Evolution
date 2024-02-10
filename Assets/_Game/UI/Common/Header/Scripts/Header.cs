using _Game.Core.Services.PersistentData;
using _Game.UI.Common.Scripts;
using TMPro;
using UnityEngine;

namespace _Game.UI.Common.Header.Scripts
{
    public class Header : MonoBehaviour, IHeader
    {
        [SerializeField] private TMP_Text _windowNameLabel;
        [SerializeField] private CurrenciesUI _currenciesUI;

        public void ShowWallet(IPersistentDataService persistentDataService)
        {
            _currenciesUI.Construct(persistentDataService.State.Currencies);
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
