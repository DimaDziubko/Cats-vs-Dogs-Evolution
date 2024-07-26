using System;
using System.Collections.Generic;
using _Game.Core.Services.IAP;

namespace CodeBase.Infrastructure.Services.IAP
{
    [Serializable]
    public class ProductConfigWrapper
    {
        public List<ProductConfig> Configs;
    }
}