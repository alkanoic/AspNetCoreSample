using System.Globalization;

namespace CodeGen;

public static class StringExtensions
{
    public static string ToCamelCase(this string str)
    {
        if (string.IsNullOrWhiteSpace(str)) return "";
        if (str.Trim().Length == 1) return char.ToLower(str.Trim().First(), CultureInfo.InvariantCulture).ToString();
        return char.ToLower(str.First(), CultureInfo.InvariantCulture) + str.Substring(1);
    }

    public static string LastCharRemove(this string str, int removeCount = 1)
    {
        if (string.IsNullOrWhiteSpace(str)) return "";
        return str.Remove(str.Length - removeCount);
    }
}
