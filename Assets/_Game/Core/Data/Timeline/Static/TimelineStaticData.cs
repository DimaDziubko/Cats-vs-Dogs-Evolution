using System.Collections.Generic;
using Assets._Game.Core.Data.Timeline.Static;

namespace _Game.Core.Data.Timeline.Static
{
    public class TimelineStaticData
    {
        public Dictionary<int, TimlineInfoItemStaticData> TimelineInfoItems { get; set; }

        public TimlineInfoItemStaticData ForInfoItem(int ageIndex)
        {
            return TimelineInfoItems[ageIndex];
        }
    }
}