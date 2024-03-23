using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Loading
{
    public interface ILoadingScreenProvider
    {
        event Action LoadingCompleted;
        UniTask LoadAndDestroy(ILoadingOperation loadingOperation);
        UniTask LoadAndDestroy(Queue<ILoadingOperation> loadingOperations);
    }
}