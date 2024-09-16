using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.TimelineInfoScreen.Scripts
{
    public interface ITimelineInfoScreenProvider
    {
        UniTask<Disposable<TimelineInfoScreen>> Load();
    }
}