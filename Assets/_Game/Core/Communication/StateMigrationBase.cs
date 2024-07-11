﻿using _Game.Core.UserState;
using Assets._Game.Core.UserState;

namespace Assets._Game.Core.Communication
{
    public abstract class StateMigrationBase : IStateMigration
    {
        public abstract string TargetVersion { get; }

        public abstract void Migrate(ref UserAccountState state);
    }
}