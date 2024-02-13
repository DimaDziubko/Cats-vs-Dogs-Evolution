using _Game.Core.Services.PersistentData;
using _Game.Core.UserState;
using _Game.Utils.Extensions;
using TMPro;
using UnityEngine;

namespace _Game.UI.Common.Scripts
{
    public class CurrenciesUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _coinsLabel;

        private IUserCurrenciesStateReadonly _currencies;
        public void Construct(IUserCurrenciesStateReadonly  currencies)
        {
            _currencies = currencies;
        }

        public void Show()
        {
            _currencies.CoinsChanged += OnCurrenciesChanged;
            OnCurrenciesChanged();
        }
        
        private void OnCurrenciesChanged()
        {
            _coinsLabel.text = _currencies.Coins.FormatMoney();
        }
        
        public void Hide()
        {
            _currencies.CoinsChanged -= OnCurrenciesChanged;
        }
    }
}