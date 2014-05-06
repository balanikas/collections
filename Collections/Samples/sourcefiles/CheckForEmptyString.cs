using System;
using System.Diagnostics;

namespace Samples
{
    internal class CheckForEmptyString
    {
        public static void UsingLength(string value)
        {
            Trace.WriteLine("1");
            Console.WriteLine("2");
            Debug.WriteLine("3");
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