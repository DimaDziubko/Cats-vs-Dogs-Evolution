using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Unity.VisualScripting;

namespace Assets._Game.Utils.Extensions
{
    public static class FloatExtensions
    {
        private static readonly List<(double Threshold, string Currency)> CurrencyThresholds =
            new List<(double, string)>
            {
                (Math.Pow(10, 93), "BA"),
                (Math.Pow(10, 90), "AZ"),
                (Math.Pow(10, 87), "AY"),
                (Math.Pow(10, 84), "AX"),
                (Math.Pow(10, 81), "AW"),
                (Math.Pow(10, 78), "AV"),
                (Math.Pow(10, 75), "AU"),
                (Math.Pow(10, 72), "AT"),
                (Math.Pow(10, 69), "AS"),
                (Math.Pow(10, 66), "AR"),
                (Math.Pow(10, 63), "AQ"),
                (Math.Pow(10, 60), "AP"),
                (Math.Pow(10, 57), "AO"),
                (Math.Pow(10, 54), "AN"),
                (Math.Pow(10, 51), "AM"),
                (Math.Pow(10, 48), "AL"),
                (Math.Pow(10, 45), "AK"),
                (Math.Pow(10, 42), "AJ"),
                (Math.Pow(10, 39), "AI"),
                (Math.Pow(10, 36), "AH"),
                (Math.Pow(10, 33), "AG"),
                (Math.Pow(10, 30), "AF"),
                (Math.Pow(10, 27), "AE"),
                (Math.Pow(10, 24), "AD"),
                (Math.Pow(10, 21), "AC"),
                (1e18, "AB"),
                (1e15, "AA"),
                (1e12, "T"),
                (1e9, "B"),
                (1e6, "M"),
                (1e3, "K"),
            };

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
            var absValue = Math.Abs(price);

            foreach (var (threshold, currency) in CurrencyThresholds)
            {
                if (absValue >= threshold)
                {
                    var newNum = price / threshold;
                    return CurrencyString(newNum, currency);
                }
            }

            return string.Format("{0:0}", price);

            #region Old
            //CultureInfo ci = CultureInfo.InvariantCulture;

            //if (price < 1_000)
            //{
            //    // Prices below 1,000 are displayed as whole numbers
            //    return Math.Round(price).ToString("0", ci);
            //}
            //else if (price >= 1_000 && price < 1_000_000)
            //{
            //    // Handle formatting for thousands
            //    float priceInThousands = price / 1_000;
            //    int integerPart = (int)priceInThousands;
            //    float decimalPart = priceInThousands - integerPart;

            //    if (Math.Abs(decimalPart) < 0.01f)
            //    {
            //        return integerPart.ToString("0", ci) + "k";
            //    }
            //    else if (Math.Abs(decimalPart) < 0.1f)
            //    {
            //        // Round to no decimal places if the first decimal place is zero
            //        return integerPart.ToString("0", ci) + "k";
            //    }
            //    else
            //    {
            //        // Use two decimal places only when necessary
            //        return priceInThousands.ToString("0.##", ci) + "k";
            //    }
            //}
            //else if (price >= 1_000_000 && price < 1_000_000_000)
            //{
            //    // Handle formatting for millions
            //    float priceInMillions = price / 1_000_000;
            //    int integerPart = (int)priceInMillions;
            //    float decimalPart = priceInMillions - integerPart;

            //    if (Math.Abs(decimalPart) < 0.01f)
            //    {
            //        return integerPart.ToString("0", ci) + "m";
            //    }
            //    else if (Math.Abs(decimalPart) < 0.1f)
            //    {
            //        // If the first decimal is zero, display without decimals
            //        return integerPart.ToString("0", ci) + "m";
            //    }
            //    else
            //    {
            //        return priceInMillions.ToString("0.##", ci) + "m";
            //    }
            //}
            //else if (price >= 1_000_000_000)
            //{
            //    // Handle formatting for billions
            //    float priceInBillions = price / 1_000_000_000;
            //    int integerPart = (int)priceInBillions;
            //    float decimalPart = priceInBillions - integerPart;

            //    if (Math.Abs(decimalPart) < 0.01f)
            //    {
            //        return integerPart.ToString("0", ci) + "b";
            //    }
            //    else if (Math.Abs(decimalPart) < 0.1f)
            //    {
            //        return integerPart.ToString("0", ci) + "b";
            //    }
            //    else
            //    {
            //        return priceInBillions.ToString("0.##", ci) + "b";
            //    }
            //}

            //return price.ToString("0", ci);
            #endregion
        }
        private static string CurrencyString(double newNum, string cur)
        {
            var str = "";

            var round = RoundNumber(newNum);
            if (round.ToString().Length >= 3)
            {
                str = round.ToString();
            }
            else
            {
                str = newNum.ToString("0.#");
            }

            //Debug.Log("MONEY_" + str);
            return str + cur;
        }
        public static double RoundNumber(double num)
        {
            if (num >= 1000)
            {
                // 51264 5 * 100000
                num = Math.Ceiling(num);
                string numStr = num.ToString();
                int fisrtNum = int.Parse(numStr.Substring(0, 1));
                double multiplied = 1;
                for (int i = 0; i < numStr.Length - 1; i++)
                {
                    multiplied = multiplied * 10;
                }

                return fisrtNum * multiplied;
            }
            else
            {
                return Math.Ceiling(num);
            }
        }
    }
}