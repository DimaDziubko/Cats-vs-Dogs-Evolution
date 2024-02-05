using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace _Game.Core.Services.Camera
{
    public class WorldCameraService : IWorldCameraService
    {
        public UnityEngine.Camera Camera => UnityEngine.Camera.main;
        public UnityEngine.Camera UICameraOverlay { get; private set; }

        public WorldCameraService(UnityEngine.Camera uICameraOverlay)
        {
            UICameraOverlay = uICameraOverlay;
        }
        
        public void AddUICameraToStack(UnityEngine.Camera camera)
        {
            var cameraData = Camera.GetUniversalAdditionalCameraData();
            cameraData.cameraStack.Add(camera);
        }

        public void SetCameraRenderType(UnityEngine.Camera camera, CameraRenderType type)
        {
            var cameraData = camera.GetUniversalAdditionalCameraData();
            cameraData.renderType = type;
        }

        public Ray ScreenPointToRay(Vector3 mousePosition)
        {
            return Camera.ScreenPointToRay(mousePosition);
        }

        public Vector3 ScreenToWorldPoint(Vector3 point)
        {
            return Camera.ScreenToWorldPoint(point);
        }
    }
}