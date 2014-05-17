using System;
namespace Phoenix.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Make an integer representation of a Social Security number into a string version with hyphens
        /// </summary>
        /// <param name="ssn">Social Security number</param>
        /// <returns></returns>
        public static string ToSocialSecurityNumberString(this int ssn)
        {
            var temp = ssn.ToString();
            return string.Format("{0}-{1}-{2}", temp.Substring(0, 3), temp.Substring(3, 2), temp.Substring(5, 4));
        }
        /// <summary>
        /// Make a string Social Security number into a string version with hyphens
        /// </summary>
        /// <param name="ssn">Social Security number</param>
        /// <returns></returns>
        public static string ToSocialSecurityNumberString(this string ssn)
        {
            return string.Format("{0}-{1}-{2}", ssn.Substring(0, 3), ssn.Substring(3, 2), ssn.Substring(5, 4));
        }

        public static int? TryParseNullableInt(this string value)
        {
            int outValue;
            return int.TryParse(value, out outValue) ? (int?)outValue : null;
        }

        public static DateTime? TryParseNullableDateTime(this string value)
        {
            DateTime outValue;
            return DateTime.TryParse(value, out outValue) ? (DateTime?)outValue : null;
        }
    }
}
