using System;

namespace Samples
{
    internal class StringComparison
    {
        public static bool CompareTo(string s1, string s2)
        {
            return String.Compare(s1, s2, System.StringComparison.Ordinal) == 0;
        }

        public static bool Equals(string s1, string s2)
        {
            return s1.Equals(s2);
        }

        public static bool DoubleEqualitySign(string s1, string s2)
        {
            return s1 == s2;
        }
    }
}