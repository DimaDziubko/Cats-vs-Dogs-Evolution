﻿using _Game.Utils.Extensions;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Game.UI.Hud
{
    public class CoinCounterView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _coinLabel;
        [SerializeField] private float _scaleDuration = 0.2f;
        [SerializeField] private float _targetScale = 1.2f;
        [SerializeField] private float _defaultScale = 1f;

        private bool _isAnimating;

        public void UpdateCoins(float newAmount)
        {
            _coinLabel.transform.DOKill();

            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(_coinLabel.transform.DOScale(_targetScale, _scaleDuration / 2))
                .Append(_coinLabel.transform.DOScale(_defaultScale, _scaleDuration / 2))
                .OnComplete(() => _coinLabel.text = newAmount.FormatMoney());
        }

        public void Clear()
        {
            _coinLabel.text = "0";
        }
    }
}