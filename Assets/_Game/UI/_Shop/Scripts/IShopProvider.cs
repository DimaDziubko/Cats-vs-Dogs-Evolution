﻿using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._Shop.Scripts
{
    public interface IShopProvider 
    {
        UniTask<Disposable<Shop>> Load();
    }
}