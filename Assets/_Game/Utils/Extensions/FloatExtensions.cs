using System;
using System.Globalization;
using System.Text;

namespace Assets._Game.Utils.Extensions
{
    public static class FloatExtensions
    {
        public static string ToInvariantString(this float number)
        {
            return number.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string FormatTime(this float value)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(value);
            StringBuilder sb = new StringBuilder();

            bool includeDays = timeSpan.Days > 0;
            bool includeHours = timeSpan.Hours > 0 || includeDays;
            bool includeMinutes = timeSpan.Minutes > 0 || includeHours;

            if (includeDays)
                sb.Append($"{timeSpan.Days}d ");
            if (includeHours)
                sb.Append($"{timeSpan.Hours}h ");
            if (includeMinutes)
                sb.Append($"{timeSpan.Minutes}m ");
            sb.Append($"{timeSpan.Seconds}s");

            return sb.ToString().Trim();
        }
        
        public static string ToSpeedFormat(this float value)
        {
            CultureInfo ci = CultureInfo.InvariantCulture;
            return Math.Round(value, 2).ToString("0.##", ci) + "/s";
        }
        
        public static string FormatMoney(this float price)
        {
            CultureInfo ci = CultureInfo.InvariantCulture;

            if (price < 1_000)
            {
                // Prices below 1,000 are displayed as whole numbers
                return Math.Round(price).ToString("0", ci);
            }
            else if (price >= 1_000 && price < 1_000_000)
            {
                // Handle formatting for thousands
                float priceInThousands = price / 1_000;
                int integerPart = (int)priceInThousands;
                float decimalPart = priceInThousands - integerPart;

                if (Math.Abs(decimalPart) < 0.01f)
                {
                    return integerPart.ToString("0", ci) + "k";
                }
                else if (Math.Abs(decimalPart) < 0.1f)
                {
                    // Round to no decimal places if the first decimal place is zero
                    return integerPart.ToString("0", ci) + "k";
                }
                else
                {
                    // Use two decimal places only when necessary
                    return priceInThousands.ToString("0.##", ci) + "k";
                }
            }
            else if (price >= 1_000_000 && price < 1_000_000_000)
            {
                // Handle formatting for millions
                float priceInMillions = price / 1_000_000;
                int integerPart = (int)priceInMillions;
                float decimalPart = priceInMillions - integerPart;

                if (Math.Abs(decimalPart) < 0.01f)
                {
                    return integerPart.ToString("0", ci) + "m";
                }
                else if (Math.Abs(decimalPart) < 0.1f)
                {
                    // If the first decimal is zero, display without decimals
                    return integerPart.ToString("0", ci) + "m";
                }
                else
                {
                    return priceInMillions.ToString("0.##", ci) + "m";
                }
            }
            else if (price >= 1_000_000_000)
            {
                // Handle formatting for billions
                float priceInBillions = price / 1_000_000_000;
                int integerPart = (int)priceInBillions;
                float decimalPart = priceInBillions - integerPart;

                if (Math.Abs(decimalPart) < 0.01f)
                {
                    return integerPart.ToString("0", ci) + "b";
                }
                else if (Math.Abs(decimalPart) < 0.1f)
                {
                    return integerPart.ToString("0", ci) + "b";
                }
                else
                {
                    return priceInBillions.ToString("0.##", ci) + "b";
                }
            }
            
            return price.ToString("0", ci);

        }
    }
}