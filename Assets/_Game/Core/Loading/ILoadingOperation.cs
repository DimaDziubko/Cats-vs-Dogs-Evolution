using System;
using Cysharp.Threading.Tasks;

namespace Assets._Game.Core.Loading
{
    public interface ILoadingOperation
    {
        public string Description { get; }
        public UniTask Load(Action<float> onProgress);
    }
}
