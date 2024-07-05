using System;
using Assets._Game.UI.TimelineInfoWindow.Scripts;

namespace Assets._Game.Core.DataPresenters._TimelineInfoPresenter
{
    public interface ITimelineInfoPresenter
    {
        event Action<TimelineInfoModel> TimelineInfoDataUpdated;
        void OnTimelineInfoWindowOpened();
    }
}