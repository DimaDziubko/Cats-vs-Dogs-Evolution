using _Game.Common;
#if cas_advertisment_enabled
using CAS;
#endif
namespace _Game.Core.Services.Analytics
{
#if cas_advertisment_enabled
    public class AdImpressionDto
    {
        public AdType Type;
        public string Network;
        public double Revenue;
        public Placement Placement;
        public string UnitId;
    }
#endif
}