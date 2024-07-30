using _Game.Common;
using CAS;

namespace _Game.Core.Services.Analytics
{
    public class AdImpressionDto
    {
        public AdType Type;
        public string Network;
        public double Revenue;
        public Placement Placement;
        public string UnitId;
    }
}