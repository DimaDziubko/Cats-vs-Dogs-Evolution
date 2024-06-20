using System.Collections.Generic;
using _Game.Core.LoadingScreen;

namespace _Game.Core.Loading
{
    public class LoadingData
    {
        public LoadingScreenType Type;
        public Queue<ILoadingOperation> Operations;
    }
}