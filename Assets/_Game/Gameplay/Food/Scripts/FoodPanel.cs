using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Gameplay.Food.Scripts
{
    public class FoodPanel : MonoBehaviour
    {
        [SerializeField] private Image _foodSlider;
        [SerializeField] private Image _foodIconHolder;
        [SerializeField] private TMP_Text _foodAmountLabel;

        public void SetupIcon(Sprite foodIcon) => 
            _foodIconHolder.sprite = foodIcon;

        public void UpdateFillAmount(float progress) => 
            _foodSlider.fillAmount = progress;

        public void OnFoodChanged(int amount) => 
            _foodAmountLabel.text = amount.ToString();
    }
}