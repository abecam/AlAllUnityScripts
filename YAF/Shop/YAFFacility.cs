

using System;
using System.Collections.Generic;
using System.Linq;

public class YAFFacility
{
    private static readonly SortedDictionary<double, string> abbrevations = new SortedDictionary<double, string>
     {
        { 1000000D, "M" },
         { 1000000000D, "B" },
            { 1000000000000D, "T" },
            { 1000000000000000D, "Qua" },
            { 1000000000000000000D, "Qui" },
            { 1000000000000000000000D, "Sext" },
            { 1000000000000000000000000D, "Sept" },
        { 1000000000000000000000000000D, "Oct" },
        { 1000000000000000000000000000000D, "Non" },
        { 1000000000000000000000000000000000D, "Deci" },
        { 1000000000000000000000000000000000000D, "Undec" },
        { 1000000000000000000000000000000000000000D, "Duodeci" },
        { 1000000000000000000000000000000000000000000D, "Tredeci" },
        { 1000000000000000000000000000000000000000000000D, "Quattuordeci" },
        { 1000000000000000000000000000000000000000000000000D, "Quindeci" },
        { 1000000000000000000000000000000000000000000000000000D, "Sexdeci" },
        { 1000000000000000000000000000000000000000000000000000000D, "Octodeci" },
        { 1000000000000000000000000000000000000000000000000000000000D, "Novemdeci" },
        { 1000000000000000000000000000000000000000000000000000000000000D, "Viginti" },
        { 1000000000000000000000000000000000000000000000000000000000000000D, "Centillion" },
     };

    public static string AbbreviateNumber(double number)
    {
        for (int i = abbrevations.Count - 1; i >= 0; i--)
        {
            KeyValuePair<double, string> pair = abbrevations.ElementAt(i);
            if (number >= pair.Key)
            {
                double floorNumber = Math.Floor(number / pair.Key);
                String numBase = "A lot";
                if (floorNumber < int.MaxValue)
                { 
                    int roundedNumber = (int )Math.Floor(number / pair.Key);
                    numBase = "" + roundedNumber;
                }

                return numBase + pair.Value;
            }
        }
        if (number < int.MaxValue)
        { 
            return ((int)number).ToString();
        }
        else
        {
            return "A lot";
        }
    }
}
