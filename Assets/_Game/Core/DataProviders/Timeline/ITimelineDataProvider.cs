using _Game.Core.Data.Timeline.Static;
using Cysharp.Threading.Tasks;

namespace _Game.Core.DataProviders.Timeline
{
    public interface ITimelineDataProvider
    {
        UniTask<TimelineStaticData> Load();
    }
}