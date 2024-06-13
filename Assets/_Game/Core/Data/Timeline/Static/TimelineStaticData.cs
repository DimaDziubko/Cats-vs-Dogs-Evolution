using System.Collections.Generic;

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