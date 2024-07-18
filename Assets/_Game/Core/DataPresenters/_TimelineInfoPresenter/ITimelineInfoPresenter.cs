using System;
using _Game.UI.TimelineInfoWindow.Scripts;

namespace _Game.Core.DataPresenters._TimelineInfoPresenter
{
    public interface ITimelineInfoPresenter
    {
        event Action<TimelineInfoModel> TimelineInfoDataUpdated;
        void OnTimelineInfoWindowOpened();
        void OnPrepareTimelineInfoData();
    }
}