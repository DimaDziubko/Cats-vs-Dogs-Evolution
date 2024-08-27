using _Game.UI._CardsGeneral._Cards.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._CardsGeneral._Summoning.Scripts
{
    public class CardSummoningView : MonoBehaviour
    {
        [SerializeField] private Image _cardColorIcon;
        [SerializeField] private TMP_Text _typeLabel;
        [SerializeField] private TMP_Text _summoningLabel;

        public CardType CardType;

        public void UpdateView(CardSummoningModel model)
        {
            _typeLabel.text = CardType.ToString();
            _summoningLabel.text = model.SummoningValue;

            _typeLabel.color = model.Color;
            _summoningLabel.color = model.Color;
            _cardColorIcon.color = model.Color;
        }
    }
}