using System;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public interface ICardsSummoningPresenter
    {
        event Action<CardsSummoningModel> CardsSummoningModelChanged;
        void Init();
        CardsSummoningModel CardsSummoningModel { get;}
        void Cleanup();
    }
}