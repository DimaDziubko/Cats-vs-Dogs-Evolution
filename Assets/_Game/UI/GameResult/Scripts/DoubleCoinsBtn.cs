using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Game.Gameplay.GameResult.Scripts
{
    public class DoubleCoinsBtn : MonoBehaviour
    {
        [SerializeField] private TMP_Text _loadingText;
        [SerializeField] private TMP_Text _x2Text;
        [SerializeField] private Image _adsIconHolder;
        
        [SerializeField] private Button _button;

        public void Initialize(Action callback)
        {
            _button.onClick.AddListener(() => callback?.Invoke());
        }

        public void SetInteractable(bool isInteractable)
        {
            _button.interactable = isInteractable;
            _adsIconHolder.enabled = isInteractable;
            _x2Text.enabled = isInteractable;

            _loadingText.enabled = !isInteractable;
        }
        
        public void Cleanup()
        {
            _button.onClick.RemoveAllListeners();
        }
        
    }
}