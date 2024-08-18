using _Game.Core.Services._FoodBoostService.Scripts;
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
            Hide();
        }

        public void Init()
        {
            Subscribe();
            _foodBoostBtn.Initialize(OnFoodBoostBtnClicked);
        }

        public void Cleanup()
        {
            Unsubscribe();
            _foodBoostBtn.Cleanup();
        }
        
        
        public void Show()
        {
            _foodBoostBtn.Show();
            OnFoodBoostBtnShown();
        }

        public void Hide() => _foodBoostBtn.Hide();

        private void OnFoodBoostBtnShown() => 
            _foodBoostService.OnFoodBoostShown();

        private void Subscribe() => 
            _foodBoostService.FoodBoostBtnModelChanged += _foodBoostBtn.UpdateBtnState;

        private void Unsubscribe()
        {
            _foodBoostService.FoodBoostBtnModelChanged -= _foodBoostBtn.UpdateBtnState;
        }

        private void OnFoodBoostBtnClicked()
        {
            _audioService.PlayButtonSound();
            _foodBoostService.OnFoodBoostBtnClicked();
        }
    }
}