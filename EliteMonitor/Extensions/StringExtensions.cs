using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EliteMonitor.Extensions
{
    public static class StringExtensions
    {

        public static bool EqualsIgnoreCase(this string s, string compare)
        {
            return s.ToLower().Equals(compare.ToLower());
        }

        public static bool EndsWithIgnoreCase(this string s, string compare)
        {
            return s.ToLower().EndsWith(compare.ToLower());
        }

        public static bool ContainsIgnoreCase(this string s, string compare)
        {
            return s.ToLower().Contains(compare.ToLower());
        }

        public static string FromNthDeliminator(this string orig, int n, char delim)
        {
            string[] data = orig.Split(delim);
            if (Math.Abs(n) >= data.Length)
                throw new IndexOutOfRangeException();
            string @out = string.Empty;
            if (n > 0)
            {
                for (int x = n; x < data.Length; x++)
                {
                    @out += (@out.Length > 0 ? delim.ToString() : "") + data[x];
                }
            }
            else
            {
                for (int x = 0; x < data.Length - Math.Abs(n); x++)
                {
                    @out += (@out.Length > 0 ? delim.ToString() : "") + data[x];
                }
            }
            return @out;
        }

        public static string[] SplitWithEscapes(this string str, char split)
        {
            var regex = new Regex(String.Format(@"(?<!\\){0}", split.ToString()));
            return regex.Split(str);
        }

        public static bool StartsWithIgnoreCase(this string str, string what)
        {
            return str.ToLower().StartsWith(what.ToLower());
        }

        public static string Replace(this string str, List<string> needles, List<string> replacements) => Replace(str, needles.ToArray(), replacements.ToArray());
        public static string Replace(this string str, string[] needles, string[] replacements)
        {
            if (needles.Length != replacements.Length)
                throw new InvalidOperationException("Number of needles does not match the number of replacers");
            string ret = str;
            for (int x = 0; x < needles.Length; x++)
                ret = ret.Replace(needles[x], replacements[x]);
            return ret;
        }

        public static string Join(this string[] strings, string separator) // I'm lazy
        {
            return String.Join(separator, strings);
        }

        public static string CapitaliseFirst(this string s)
        {
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static string JoinWithDifferingLast(this string[] strings, string deliminator, string lastDeliminator = " and ")
        {
            int x = strings.Length;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < strings.Length; i++)
            {
                string delim = deliminator;
                if (i == x - 1)
                    delim = lastDeliminator;
                sb.Append(string.Format("{0}{1}", (i > 0 ? delim : ""), strings[i]));
            }
            return sb.ToString();
        }


        public class SearchPointNotFoundException : Exception { }
        public static string GetStringBetween(this string str, string start, string end)
        {
            int s = str.IndexOf(start);
            int e = str.IndexOf(end);

            if (s == -1 || e == -1)
                throw new SearchPointNotFoundException();
            else if (e > str.Length) // impossible, but better safe than sorry
                throw new ArgumentOutOfRangeException();

            s += start.Length;
            string return_str = str.Substring(s, e - s);
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("{0} // {1} | {2} | {3}", s, e, return_str, str));
#endif
            return return_str;
        }
    }
}
