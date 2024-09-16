using _Game.Core._DataPresenters.UnitUpgradePresenter;
using _Game.Core.AssetManagement;
using _Game.Core.Services.Audio;
using _Game.Core.Services.Camera;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.UpgradesAndEvolution.Scripts
{
    public interface IStatsPopupProvider
    {
        UniTask<Disposable<StatsPopup>> Load();
    }

    public class StatsPopupProvider : LocalAssetLoader, IStatsPopupProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IStatsPopupPresenter _statsPopupPresenter;

        public StatsPopupProvider(
            IWorldCameraService cameraService, 
            IAudioService audioService,
            IStatsPopupPresenter statsPopupPresenter)
        {
            _cameraService = cameraService;
            _audioService = audioService;
            _statsPopupPresenter = statsPopupPresenter;
        }
        
        public async UniTask<Disposable<StatsPopup>> Load()
        {
            var popup = await LoadDisposable<StatsPopup>(AssetsConstants.STATS_POPUP);
            popup.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService,
                _statsPopupPresenter);
            return popup;
        }
    }
}