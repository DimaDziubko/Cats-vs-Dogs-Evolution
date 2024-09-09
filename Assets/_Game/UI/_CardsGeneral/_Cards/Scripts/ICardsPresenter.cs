using System;
using System.Collections.Generic;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public interface ICardsPresenter
    {
        event Action<int, CardModel> CardModelUpdated;
        SortedDictionary<int, CardModel> CardModels { get;}
        void OnCardClicked(int id);
        void Init();
        void UpgradeCard(int id);
    }
}