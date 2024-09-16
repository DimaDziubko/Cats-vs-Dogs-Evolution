using UnityEngine;

namespace _Game.Core.Services.Camera
{
    public interface IWorldCameraService 
    {
        UnityEngine.Camera MainCamera { get; }
        UnityEngine.Camera UICameraOverlay { get; }
        float CameraHeight { get; }
        float CameraWidth { get; }
        Ray ScreenPointToRay(Vector3 mousePosition);
        Vector3 ScreenToWorldPoint(Vector3 vector3);
        void EnableMainCamera();
    }
}