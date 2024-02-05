using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Loading
{
    public interface ILoadingScreenProvider 
    {
        UniTask LoadAndDestroy(Queue<ILoadingOperation> loadingOperations);
    }
}