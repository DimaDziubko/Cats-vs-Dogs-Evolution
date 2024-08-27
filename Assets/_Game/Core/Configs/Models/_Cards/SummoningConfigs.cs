﻿using System;
using System.Collections.Generic;
using _Game.UI._CardsGeneral._Cards.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Core.Configs.Models._Cards
{
    [CreateAssetMenu(fileName = "CardsConfig", menuName = "Configs/Summoning")]
    public class SummoningConfigs : ScriptableObject
    {
        public int Id;
        [FormerlySerializedAs("LevelDropRates")] public List<CardsSummoning> CardsSummoningConfigs;
    }

    [Serializable]
    public class CardsSummoning
    {
        public int Level;
        [FormerlySerializedAs("CardsRequiredForNextLevel")] public int CardsRequiredForLevel;
        
        [FormerlySerializedAs("AccumulatedCardsRequiredForNextLevel")] [HideInInspector]
        public int AccumulatedCardsRequiredForLevel; 
        
        [HorizontalGroup("Rates"), LabelWidth(60)]
        public float Common;
        [HorizontalGroup("Rates"), LabelWidth(60)]
        public float Rare;
        [HorizontalGroup("Rates"), LabelWidth(60)]
        public float Epic;
        [HorizontalGroup("Rates"), LabelWidth(60)]
        public float Legendary;

        public string ForType(CardType viewCardType)
        {
            float rate;
            
            switch (viewCardType)
            {
                case CardType.Common:
                    rate = Common;
                    break;
                case CardType.Rare:
                    rate = Rare;
                    break;
                case CardType.Epic:
                    rate = Epic;
                    break;
                case CardType.Legendary:
                    rate = Legendary;
                    break;
                default:
                    rate = Common;
                    break;
            }
            return rate.ToString("0.000") + "%";
        }
    }
}