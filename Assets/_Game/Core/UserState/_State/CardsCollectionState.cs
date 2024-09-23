using System;
using System.Collections.Generic;
using System.Linq;

namespace _Game.Core.UserState._State
{
    public interface ICardsCollectionStateReadonly
    {
        event Action<int> CardsSummoningLevelChanged;
        event Action<int> CardsSummoningProgressChanged;
        event Action<int> CardUpgraded;
        event Action CardsCollectionChanged;
        event Action<List<int>> CardsCollected;
        int CardsSummoningLevel { get; }
        int CardsSummoningProgressCount { get; }
        List<Card> Cards { get; }
        int LastDropIdx { get; }
    }

    public class CardsCollectionState : ICardsCollectionStateReadonly
    {
        public event Action<int> CardsSummoningLevelChanged;
        public event Action<int> CardsSummoningProgressChanged;
        public event Action<int> CardUpgraded;
        public event Action CardsCollectionChanged;
        public event Action<List<int>> CardsCollected;

        public int CardSummoningLevel;
        public int CardsSummoningProgressCount;
        public int LastDropIdx;
        public List<Card> Cards;

        int ICardsCollectionStateReadonly.CardsSummoningLevel => CardSummoningLevel;
        int ICardsCollectionStateReadonly.LastDropIdx => LastDropIdx;
        int ICardsCollectionStateReadonly.CardsSummoningProgressCount => CardsSummoningProgressCount;
        List<Card> ICardsCollectionStateReadonly.Cards => Cards;


        public void ChangeCardSummoningLevel(int newLevel)
        {
            CardSummoningLevel = newLevel;
            LastDropIdx = 0;
            CardsSummoningLevelChanged?.Invoke(CardSummoningLevel);
        }

        private void ChangeCardSummoningProgressCount(int delta)
        {
            CardsSummoningProgressCount += delta;
            CardsSummoningProgressChanged?.Invoke(CardsSummoningProgressCount);
        }

        public void UpgradeCard(int id, int needForUpgrade)
        {
            var card = Cards.FirstOrDefault(x => x.Id == id);
            if (card != null)
            {
                card.Level++;
                card.Count -= needForUpgrade;
            }
            CardUpgraded?.Invoke(id);
        }
        
        public void AddCards(List<int> cardsId)
        {
            var cardsDictionary = Cards.ToDictionary(card => card.Id, card => card);

            foreach (var cardId in cardsId)
            {
                if (cardsDictionary.TryGetValue(cardId, out var existingCard))
                {
                    existingCard.Count++;
                }
                else
                {
                    var newCard = new Card
                    {
                        Id = cardId,
                        Level = 1,
                        Count = 0,
                        Equipped = false,
                        EquippedSlot = -1
                    };
                    Cards.Add(newCard);
                    cardsDictionary[cardId] = newCard;
                }
            }
            
            ChangeCardSummoningProgressCount(cardsId.Count);
            
            CardsCollected?.Invoke(cardsId);
            CardsCollectionChanged?.Invoke();
        }

        public void ChangeLastDropIdx(int nextIndex)
        {
            LastDropIdx = nextIndex;
        }
    }
}