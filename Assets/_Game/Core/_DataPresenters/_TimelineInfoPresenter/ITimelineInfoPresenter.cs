using System;
using _Game.UI.TimelineInfoScreen.Scripts;

namespace _Game.Core.DataPresenters._TimelineInfoPresenter
{
    public interface ITimelineInfoPresenter
    {
        event Action<TimelineInfoModel> TimelineInfoDataUpdated;
        void OnTimelineInfoScreenOpened();
        void OnPrepareTimelineInfoData();
    }
}