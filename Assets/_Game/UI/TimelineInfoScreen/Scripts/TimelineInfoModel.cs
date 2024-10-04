using System.Collections.Generic;
using Assets._Game.UI.TimelineInfoWindow.Scripts;

namespace _Game.UI.TimelineInfoScreen.Scripts
{
    public class TimelineInfoModel
    {
        public int CurrentAge;
        public string TimelineInfo;
        public string DifficultyInfo;
        public bool ShowDifficulty;
        public List<TimelineInfoItemModel> Models;
    }
}