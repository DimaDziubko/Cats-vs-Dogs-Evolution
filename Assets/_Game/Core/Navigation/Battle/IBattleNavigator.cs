﻿using System;
using Assets._Game.UI._StartBattleWindow.Scripts;

namespace _Game.Core.Navigation.Battle
{
    public interface IBattleNavigator
    {
        int CurrentBattle { get;}
        event Action BattleChanged;
        event Action<BattleNavigationModel> NavigationUpdated;
        void OnStartBattleWindowOpened();
        void MoveToPreviousBattle();
        void MoveToNextBattle();
        void OpenNextBattle();
        bool CanMoveToPreviousBattle();
        void ForceMoveToNextBattle();
    }
}