using Assets._Game.Core.Services._FoodBoostService.Scripts;
using Assets._Game.Core.Services.Audio;
using UnityEngine;

namespace _Game.UI._Hud._FoodBoostView
{
    public class FoodBoostView : MonoBehaviour
    {
        [SerializeField] private FoodBoostBtn _foodBoostBtn;
        
        private IFoodBoostService _foodBoostService;
        private IAudioService _audioService;

        public void Construct(
            IFoodBoostService foodBoostService,
            IAudioService audioService)
        {
            _foodBoostService = foodBoostService;
            _audioService = audioService;
            HideFoodBoostBtn();
        }
        
        public void ShowFoodBoostBtn()
        {
            SubscribeFoodBoostBtn();
            _foodBoostBtn.Initialize(OnFoodBoostBtnClicked);
            _foodBoostBtn.Show();
            OnFoodBoostBtnShown();
        }

        private void OnFoodBoostBtnShown() => 
            _foodBoostService.OnFoodBoostShown();

        private void SubscribeFoodBoostBtn() => 
            _foodBoostService.FoodBoostBtnModelChanged += _foodBoostBtn.UpdateBtnState;

        public void HideFoodBoostBtn()
        {
            _foodBoostBtn.Hide();
            _foodBoostBtn.Cleanup();
            UnsubscribeFoodBoostBtn();
        }

        private void UnsubscribeFoodBoostBtn() => 
            _foodBoostService.FoodBoostBtnModelChanged -= _foodBoostBtn.UpdateBtnState;

        private void OnFoodBoostBtnClicked()
        {
            _audioService.PlayButtonSound();
            _foodBoostService.OnFoodBoostBtnClicked();
        }
    }
}