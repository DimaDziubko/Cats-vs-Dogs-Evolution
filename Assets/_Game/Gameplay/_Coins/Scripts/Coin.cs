﻿using _Game.Common;
using _Game.Gameplay._Coins.Factory;
using UnityEngine;

namespace _Game.Gameplay._Coins.Scripts
{
    public class Coin : GameBehaviour
    {
        [SerializeField] protected Transform _transform;
        public Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }
        
        public ICoinFactory OriginFactory { get; set; }
        public override void Recycle()
        {
            OriginFactory.Reclaim(this);
        }
    }
}