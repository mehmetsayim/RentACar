using System.Text.RegularExpressions;

namespace RentACarWeb
{
    public enum Roles
    {
        Administrators, ProductManagers, OrderManagers, Members
    }
    public static class AppExtensions
    {
        public static string ToSafeUrlString(this string text)
        {
            return Regex.Replace(string.Concat(text.Where(p => char.IsLetterOrDigit(p) || char.IsWhiteSpace(p))), @"\s+", "-").ToLower();
        }
    }
}
