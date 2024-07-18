using _Game.UI._Hud;
using Assets._Game.Core.Services.Camera;
using Assets._Game.Gameplay._UnitBuilder.Scripts;
using Assets._Game.Gameplay.Food.Scripts;
using UnityEngine;

namespace _Game.UI.UnitBuilderBtn.Scripts
{
    public class GameplayUI : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        
        [SerializeField] private FoodPanel _foodPanel;
        [SerializeField] private UnitBuilderUI _unitBuilderUI;
        [SerializeField] private WaveInfoPopup _waveInfo;
        
        public UnitBuilderUI UnitBuilderUI => _unitBuilderUI;
        public FoodPanel FoodPanel => _foodPanel;
        public WaveInfoPopup WaveInfoPopup => _waveInfo;
        
        public void Construct(IWorldCameraService cameraService)
        {
            _canvas.worldCamera = cameraService.UICameraOverlay;
            Hide();
        }
        
        public void Show() => _canvas.enabled = true;

        public void Hide() => _canvas.enabled = false;
    }
}