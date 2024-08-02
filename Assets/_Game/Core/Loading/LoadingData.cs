using System.Collections.Generic;
using _Game.Core.LoadingScreen;
using Assets._Game.Core.Loading;

namespace _Game.Core.Loading
{
    public class LoadingData
    {
        public LoadingScreenType Type;
        public Queue<ILoadingOperation> Operations;
    }
}