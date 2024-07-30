using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace Assets._Game.UI.TimelineInfoWindow.Scripts
{
    public interface ITimelineInfoScreenProvider
    {
        UniTask<Disposable<global::_Game.UI.TimelineInfoWindow.Scripts.TimelineInfoScreen>> Load();
    }
}