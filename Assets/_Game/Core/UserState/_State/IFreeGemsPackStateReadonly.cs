using System;

namespace _Game.Core.UserState
{
    public interface IFreeGemsPackStateReadonly
    {
        event Action FreeGemsPackCountChanged;
        int FreeGemPackCount { get; }
        DateTime LastFreeGemPackDay { get;}
    }
}