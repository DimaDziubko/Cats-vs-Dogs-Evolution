using System;
using System.Globalization;

namespace _Game.Utils.Extensions
{
    public static class FloatExtensions
    {
        public static string ToSpeedFormat(this float value)
        {
            CultureInfo ci = CultureInfo.InvariantCulture;
            return Math.Round(value, 2).ToString("0.##", ci) + "/s";
        }
        
        public static string FormatMoney(this float price)
        {
            CultureInfo ci = CultureInfo.InvariantCulture;
            
            if (price >= 1_000 && price < 10_000)
            {
                if (price % 1000 == 0)
                {
                    return (price / 1000).ToString("0", ci) + "k";
                }
                else
                {
                    return (price / 1000).ToString("0.##", ci) + "k"; 
                }
            }
            else if (price >= 10_000 && price < 1_000_000)
            {
                if (price % 1000 == 0)
                {
                    return (price / 1000).ToString("0", ci) + "k";
                }
                else
                {
                    return (price / 1000).ToString("0.#", ci) + "k"; 
                }
            }
            else if (price >= 1_000_000 && price < 1_000_000_000)
            {
                if (price % 1_000_000 == 0)
                {
                    return (price / 1_000_000).ToString("0", ci) + "m";
                }
                else
                {
                    return (price / 1_000_000).ToString("0.#", ci) + "m";
                }
            }
            else if (price >= 1_000_000_000)
            {
                if (price % 1_000_000_000 == 0)
                {
                    return (price / 1_000_000_000).ToString("0", ci) + "b";
                }
                else
                {
                    return (price / 1_000_000_000).ToString("0.#", ci) + "b";
                }
            }
            else
            {
                return price.ToString("0", ci);
            }
        }
    }
}