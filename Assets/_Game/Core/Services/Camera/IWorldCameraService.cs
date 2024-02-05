using UnityEngine;

namespace _Game.Core.Services.Camera
{
    public interface IWorldCameraService 
    {
        UnityEngine.Camera Camera { get; }
        UnityEngine.Camera UICameraOverlay { get; }
        void AddUICameraToStack(UnityEngine.Camera camera);
        Ray ScreenPointToRay(Vector3 mousePosition);
        Vector3 ScreenToWorldPoint(Vector3 vector3);
    }
}