using System;
using System.Text.RegularExpressions;

// source : http://www.mikeobrien.net/blog/parseexact-for-strings
// example:
//
// string value = "2/17/2009 10:57:42 AM...Executing file 26 of 81 files";
// string[] parts = value.ParseExact("{0}...Executing file {1} of {2} files");
// foreach (string part in parts)
//     Console.WriteLine(part);
// Console.ReadKey();
namespace Onha.Kiet
{
    public static class StringExtensions
    {
        public static string[] ParseExact(
            this string data,
            string format)
        {
            return ParseExact(data, format, false);
        }

        public static string[] ParseExact(
            this string data,
            string format,
            bool ignoreCase)
        {
            string[] values;

            if (TryParseExact(data, format, out values, ignoreCase))
                return values;
            else
                throw new ArgumentException("Format not compatible with value.");
        }

        public static bool TryExtract(
            this string data,
            string format,
            out string[] values)
        {
            return TryParseExact(data, format, out values, false);
        }

        public static bool TryParseExact(
            this string data,
            string format,
            out string[] values,
            bool ignoreCase)
        {
            int tokenCount = 0;
            format = Regex.Escape(format).Replace("\\{", "{");

            for (tokenCount = 0; ; tokenCount++)
            {
                string token = string.Format("{{0}}", tokenCount);
                if (!format.Contains(token)) break;
                format = format.Replace(token,
                    string.Format("(?'group{0}'.*)", tokenCount));
            }

            RegexOptions options =
                ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;

            Match match = new Regex(format, options).Match(data);

            if (tokenCount != (match.Groups.Count - 1))
            {
                values = new string[] { };
                return false;
            }
            else
            {
                values = new string[tokenCount];
                for (int index = 0; index < tokenCount; index++)
                    values[index] =
                        match.Groups[string.Format("group{0}", index)].Value;
                return true;
            }
        }

        public static string RemoveCatrigeReturn(this string data)
        {
            var badChars = new char[] { '\r', '\n', ' '};
            return data.Trim(badChars).Replace("\r","").Replace("\n","").Replace("\t", "");
        }
    }
}