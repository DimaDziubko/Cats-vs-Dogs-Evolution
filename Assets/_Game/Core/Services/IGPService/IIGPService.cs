using System.Collections.Generic;
using _Game.Core.Services.IAP;

namespace _Game.Core.Services.IGPService
{
    public interface IIGPService
    {
        List<ProductDescription> Products();
    }
}