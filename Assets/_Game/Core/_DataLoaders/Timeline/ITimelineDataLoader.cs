using _Game.Core.Data.Timeline.Static;
using Cysharp.Threading.Tasks;

namespace _Game.Core.DataProviders.Timeline
{
    public interface ITimelineDataLoader
    {
        UniTask<TimelineStaticData> Load(int timelineId);
    }
}