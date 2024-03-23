using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace _Game.Core.Services.Camera
{
    public class WorldCameraService : IWorldCameraService
    {
        public UnityEngine.Camera MainCamera { get; private set; }
        public UnityEngine.Camera UICameraOverlay { get; private set; }

        public float CameraHeight => MainCamera.orthographicSize;
        public float CameraWidth => CameraHeight * MainCamera.aspect;
        
        public WorldCameraService(UnityEngine.Camera mainCamera, UnityEngine.Camera uICameraOverlay)
        {
            MainCamera = mainCamera;
            UICameraOverlay = uICameraOverlay;
        }
        
        public Ray ScreenPointToRay(Vector3 mousePosition)
        {
            return MainCamera.ScreenPointToRay(mousePosition);
        }

        public Vector3 ScreenToWorldPoint(Vector3 point)
        {
            return MainCamera.ScreenToWorldPoint(point);
        }

        public void DisableCamera()
        {
            MainCamera.enabled = false;
        }

        public void EnableCamera()
        {
            MainCamera.enabled = true;
        }
    }
}