﻿// Copyright (c) Service Stack LLC. All Rights Reserved.
// License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Samples.ServiceStack
{
    public static class StringUtils
    {
#if !SL5
        static readonly Regex StripHtmlUnicodeRegEx = new Regex(@"&(#)?([xX])?([^ \f\n\r\t\v;]+);", RegexOptions.Compiled);
#else
        static readonly Regex StripHtmlUnicodeRegEx = new Regex(@"&(#)?([xX])?([^ \f\n\r\t\v;]+);");
#endif

        public static string ConvertHtmlCodes(this string html)
        {
            return StripHtmlUnicodeRegEx.Replace(html, ConvertHtmlCodeToCharacter);
        }

        static string ConvertHtmlCodeToCharacter(Match match)
        {
            // http://www.w3.org/TR/html5/syntax.html#character-references
            // match.Groups[0] is the entire match, the sub groups start at index one
            if (!match.Groups[1].Success)
            {
                string convertedValue;
                if (HtmlCharacterCodes.TryGetValue(match.Value, out convertedValue))
                {
                    return convertedValue;
                }
                return match.Value; // ambiguous ampersand
            }
            string decimalString = match.Groups[3].Value;
            ushort decimalValue;
            if (match.Groups[2].Success)
            {
                bool parseWasSuccessful = ushort.TryParse(decimalString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out decimalValue);
                if (!parseWasSuccessful)
                {
                    return match.Value; // ambiguous ampersand
                }
            }
            else
            {
                bool parseWasSuccessful = ushort.TryParse(decimalString, out decimalValue);
                if (!parseWasSuccessful)
                {
                    return match.Value; // ambiguous ampersand
                }
            }
            return ((char)decimalValue).ToString(CultureInfo.InvariantCulture);
        }

        public static string ToChar(this int codePoint)
        {
            return Convert.ToChar(codePoint).ToString(CultureInfo.InvariantCulture);
        }

        // http://www.w3.org/TR/html5/entities.json
        // TODO: conditional compilation for NET45 that uses ReadOnlyDictionary
        public static readonly IDictionary<string, string> HtmlCharacterCodes = new SortedDictionary<string, string>
		{
		    { @"&Aacute;", 193.ToChar() },
			{ @"&aacute;", 225.ToChar() }
		
		};
    }
}