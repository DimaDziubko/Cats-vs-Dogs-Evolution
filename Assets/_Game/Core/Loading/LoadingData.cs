using System.Collections.Generic;
using Assets._Game.Core.Loading;
using Assets._Game.Core.LoadingScreen;

namespace _Game.Core.Loading
{
    public class LoadingData
    {
        public LoadingScreenType Type;
        public Queue<ILoadingOperation> Operations;
    }
}