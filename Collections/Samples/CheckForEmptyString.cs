using System;

namespace Samples
{
    internal class CheckForEmptyString
    {
        public static void UsingLength(string value)
        {
            if (value.Length != 0)
            {
            }
        }

        public static void UsingEmptyString(string value)
        {
            if (value != "")
            {
            }
        }

        public static void UsingEmptyStringConstant(string value)
        {
            if (value != string.Empty)
            {
            }
        }

        public static void UsingMethodIsNullOrEmpty(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
            }
        }
    }
}