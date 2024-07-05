using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace Assets._Game.UI.TimelineInfoWindow.Scripts
{
    public interface ITimelineInfoWindowProvider
    {
        UniTask<Disposable<TimelineInfoWindow>> Load();
    }
}