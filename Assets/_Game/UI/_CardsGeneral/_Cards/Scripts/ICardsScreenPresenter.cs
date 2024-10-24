﻿using System;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public interface ICardsScreenPresenter
    {
        event Action<int> CardBought;
        event Action<TransactionButtonModel[]> ButtonModelsChanged;
        ICardsSummoningPresenter CardsSummoningPresenter { get; }
        TransactionButtonModel[] ButtonModels { get;}
        string CardsCountInfo { get; }
        void TryToBuyX1Card();
        void TryToBuyX10Card();
        void OnCardsScreenOpened();
    }
}