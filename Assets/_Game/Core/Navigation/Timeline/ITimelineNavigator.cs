﻿using System;

namespace _Game.Core.Navigation.Timeline
{
    public interface ITimelineNavigator
    {
        public event Action TimelineChanged;
    }
}