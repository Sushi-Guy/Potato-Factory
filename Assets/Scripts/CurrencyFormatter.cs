using UnityEngine;

public static class CurrencyFormatter
{
    // Our list of cozy tycoon suffixes matching your exact layout
    private static readonly string[] Suffixes = {
        "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc"
    };

    public static string FormatMoney(double value)
    {
        if (value < 1000)
        {
            // For small amounts, just show the exact whole number (e.g., $450)
            return value.ToString("F0"); 
        }

        int suffixIndex = 0;

        // Keep dividing by 1000 until the number is under 1000, 
        // shifting us down the suffix list line by line
        while (value >= 1000 && suffixIndex < Suffixes.Length - 1)
        {
            value /= 1000;
            suffixIndex++;
        }

        // Returns the number with 2 decimal places followed by the abbreviation 
        // Example: 1,540,000 becomes "1.54M"
        return value.ToString("F2") + Suffixes[suffixIndex];
    }
}