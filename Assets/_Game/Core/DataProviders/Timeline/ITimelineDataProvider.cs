using Assets._Game.Core.Data.Timeline.Static;
using Cysharp.Threading.Tasks;

namespace Assets._Game.Core.DataProviders.Timeline
{
    public interface ITimelineDataProvider
    {
        UniTask<TimelineStaticData> Load();
    }
}