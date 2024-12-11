using System;
using System.Globalization;
using System.Text.RegularExpressions;

public class CreditCardValidator
{
    static bool IsValidCreditCardNumber(string cardNumber)
    {
        int sum = 0;
        bool alternate = false;

        for (int i = cardNumber.Length - 1; i >= 0; i--)
        {
            int n = int.Parse(cardNumber[i].ToString());

            if (alternate)
            {
                n *= 2;
                if (n > 9)
                {
                    n -= 9;
                }
            }

            sum += n;
            alternate = !alternate;
        }

        return (sum % 10 == 0);
    }

    static string IsValidUSACardType(string cardNumber)
    {
        // Visa: Length 13 or 16, prefix 4
        if (Regex.IsMatch(cardNumber, @"^4[0-9]{12}(?:[0-9]{3})?$"))
            return "VISA";

        // MasterCard: Length 16, prefix 51-55 or 2221-2720
        if (Regex.IsMatch(cardNumber, @"^5[1-5][0-9]{14}$") || Regex.IsMatch(cardNumber, @"^2(2[2-9][0-9]{2}|[3-7][0-9]{3})[0-9]{12}$"))
            return "MASTERCARD";

        // American Express: Length 15, prefix 34 or 37
        if (Regex.IsMatch(cardNumber, @"^3[47][0-9]{13}$"))
            return "AMERICANEXPRESS";

        // Discover: Length 16, prefix 6011, 622126-622925, 644-649, 65
        if (Regex.IsMatch(cardNumber, @"^6(?:011|5[0-9]{2})(?:[0-9]{12})$") || Regex.IsMatch(cardNumber, @"^6(?:22[1-9][0-9]{3}|4[4-9][0-9]{2})(?:[0-9]{10})$"))
            return "DISCOVER";

        // Add more patterns as needed

        return "";
    }

    // Combined check for Luhn algorithm and known USA card types
    public static string IsValidUSACreditCard(string cardNumber)
    {
        if (IsValidCreditCardNumber(cardNumber))
        {
            return IsValidUSACardType(cardNumber);
        }
        return "";
    }

    public static bool IsValidExpiryDate(string expiryDate)
    {
        // Try parsing the date in MM/YY format
        if (DateTime.TryParseExact(expiryDate, "MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
        {
            return !IsExpired(parsedDate);
        }

        // Try parsing the date in MM/YYYY format
        if (DateTime.TryParseExact(expiryDate, "MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
        {
            return !IsExpired(parsedDate);
        }

        return false;
    }

    private static bool IsExpired(DateTime expiryDate)
    {
        // The expiry date is the end of the month
        expiryDate = expiryDate.AddMonths(1).AddDays(-1);

        // Compare expiry date with the current date
        return expiryDate < DateTime.Today;
    }
}
