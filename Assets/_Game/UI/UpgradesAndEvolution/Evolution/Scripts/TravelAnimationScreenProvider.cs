using _Game.Core.AssetManagement;
using _Game.UI.UpgradesAndEvolution.Evolution.Scripts;
using Assets._Game.Core.AssetManagement;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.UI.UpgradesAndEvolution.Scripts
{
    public class TravelAnimationScreenProvider : LocalAssetLoader
    {
        private Camera _camera;
        public TravelAnimationScreenProvider(Camera camera)
        {
            _camera = camera;
        }
        public async UniTask<Disposable<TravelAnimationScreen>> Load()
        {
            var window = await
                LoadDisposable<TravelAnimationScreen>(AssetsConstants.TRAVEL_ANIMATION_SCREEN);
            window.Value.Construct(_camera);
            return window;
        }
    }
}
