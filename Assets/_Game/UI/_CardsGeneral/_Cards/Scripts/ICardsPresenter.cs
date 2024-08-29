using System;
using System.Collections;
using System.Collections.Generic;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public interface ICardsPresenter
    {
        event Action<int, CardModel> CardModelUpdated;
        Dictionary<int, CardModel> CardModels { get;}
        void Init();
    }
}