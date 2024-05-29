using _Game.Core.Services.Camera;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay.Food.Scripts;
using UnityEngine;

namespace _Game.UI.UnitBuilderBtn.Scripts
{
    public class GameplayUI : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        
        [SerializeField] private FoodPanel _foodPanel;
        [SerializeField] private UnitBuilderUI _unitBuilderUI;
        
        public UnitBuilderUI UnitBuilderUI => _unitBuilderUI;
        public FoodPanel FoodPanel => _foodPanel;
        
        public void Construct(IWorldCameraService cameraService)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;
            Hide();
        }
        
        public void Show() => _canvas.enabled = true;

        public void Hide() => _canvas.enabled = false;
    }
}