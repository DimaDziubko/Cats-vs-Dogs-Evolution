using _Game.Core.Services._FoodBoostService.Scripts;
using _Game.Core.Services.Audio;
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
        }

        public void Init()
        {
            Subscribe();
            _foodBoostBtn.Initialize(OnFoodBoostBtnClicked);
            Hide();
        }

        public void Cleanup()
        {
            Unsubscribe();
            _foodBoostBtn.Cleanup();
        }
        
        
        public void Show()
        {
            Debug.Log("Food boost btn show");
            gameObject.SetActive(true);
            OnFoodBoostBtnShown();
        }

        public void Hide()
        {
            Debug.Log("Food boost btn hide");
            gameObject.SetActive(false);
        }

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