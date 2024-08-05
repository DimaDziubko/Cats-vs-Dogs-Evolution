using System;

namespace _Game.Core._GameListenerComposite
{
    public interface IFoodConsumer : IFoodListener
    {
        event Action<int, bool> ChangeFood;
    }
}