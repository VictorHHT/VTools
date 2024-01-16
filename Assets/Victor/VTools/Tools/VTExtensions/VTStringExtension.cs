using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Victor.Tools
{
    public static class VTStringExtension
    {
        public enum RichTextBuiltInColorNames { black, white, silver, grey, brown, blue, lightblue, darkblue, cyan, navy, green, lime, red, maroon, orange, yellow, purple, teal, olive };

        /// <summary>
        /// Check whether the string is null or empty
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool NullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// Check whether the string is null or all white spaces
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool NullOrWhiteSpace(this string s)
        {
            return string.IsNullOrWhiteSpace(s);
        }

        /// <summary>
        /// Check whether the string is null or empty or contains onlh of white spaces characters
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool NullOrEmptyOrWhiteSpace(this string s)
        {
            if (string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s))
            {
                return true;
            }

            return false;
        }

        public static bool EmptyOrWhiteSpace(this string s)
        {
            if (s != null && (s.Length == 0 || s.Trim().Length == 0))
            {
                return true;                
            }

            return false;
        }

        /// <summary>
        /// Compare one version with another, the separater must be . and the number needs to be the same
        /// </summary>
        /// <param name="version"></param>
        /// <param name="versionToCompare"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static bool VersionMinorThan(this string version, string versionToCompare)
        {
            string[] array = version.Split(new char[]
            {
                '.'
            });
            string[] array2 = versionToCompare.Split(new char[]
            {
                '.'
            });
            if (array.Length != array2.Length)
            {
                throw new ArgumentException("Invalid, two versions must have the same number of \".\" separater");
            }
            for (int i = 0; i < array.Length; i++)
            {
                int num = Convert.ToInt32(array[i]);
                int num2 = Convert.ToInt32(array2[i]);
                if (i == array.Length - 1)
                {
                    return num < num2;
                }
                if (num != num2)
                {
                    if (num < num2)
                    {
                        return true;
                    }
                    if (num > num2)
                    {
                        return false;
                    }
                }
            }
            throw new ArgumentException("Invalid");
        }

        public static string Truncate(this string value, int maxLength, string truncateString = "...")
        {
            if (string.IsNullOrEmpty(value))
                return value;
            return value.Length <= maxLength ? value : $"{value.Substring(0, maxLength)}{truncateString}";
        }

        public static IEnumerable<int> AllIndicesOf(this string haystack, string needle, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            if (string.IsNullOrEmpty(needle))
            {
                yield break;
            }

            for (var index = 0; ; index += needle.Length)
            {
                // Get the index of the first occurrence of the needle in the haystack
                index = haystack.IndexOf(needle, index, stringComparison);

                if (index == -1)
                {
                    break;
                }

                yield return index;
            }
        }

        public static string Filter(this string s, bool allowLetter = true, bool allowNumber = true, bool allowWhiteSpace = true, bool allowSymbol = true, bool allowPunctuation = true)
        {
            var sb = new StringBuilder();

            foreach (var c in s)
            {
                if ((!allowLetter && char.IsLetter(c)) ||
                    (!allowNumber && char.IsNumber(c)) ||
                    (!allowWhiteSpace && char.IsWhiteSpace(c)) ||
                    (!allowSymbol && char.IsSymbol(c)) ||
                    (!allowPunctuation && char.IsPunctuation(c)))
                {
                    continue;
                }

                sb.Append(c);
            }

            return sb.ToString();
        }

        public static bool IsWordDelimiter(char c)
        {
            return char.IsWhiteSpace(c) || char.IsSymbol(c) || char.IsPunctuation(c);
        }

        public static string TrimEnd(this string source, string value)
        {
            if (!source.EndsWith(value))
            {
                return source;
            }

            return source.Remove(source.LastIndexOf(value));
        }

        public static string TrimStart(this string source, string value)
        {
            if (!source.StartsWith(value))
            {
                return source;
            }

            return source.Substring(value.Length);
        }

        public static string FirstCharacterToLower(this string s)
        {
            if (string.IsNullOrEmpty(s) || char.IsLower(s, 0))
            {
                return s;
            }

            return char.ToLowerInvariant(s[0]) + s[1..];
        }

        public static string FirstCharacterToUpper(this string s)
        {
            if (string.IsNullOrEmpty(s) || char.IsUpper(s, 0))
            {
                return s;
            }
            
            return char.ToUpperInvariant(s[0]) + s[1..];
        }

        private static readonly Regex guidRegex = new Regex(@"[a-fA-F0-9]{8}(\-[a-fA-F0-9]{4}){3}\-[a-fA-F0-9]{12}");

        public static bool IsUnityGUID(string value)
        {
            return guidRegex.IsMatch(value);
        }

        // Rich Text

        public static string Color(this string s, Color color)
        {
            if (s.NullOrEmptyOrWhiteSpace())
            {
                return s;
            }

            return $"<color=#{VTColor.ColorToHex(color)}>{s}</color>";
        }

        /// <summary>
        /// Use rich text to colorize the string
        /// </summary>
        /// <param name="s"></param>
        /// <param name="colorHex">If useBuiltInName is true, you should pass in built in names, hex string if false</param>
        /// <param name="useBuiltInName"></param>
        /// <returns></returns>
        public static string Color(this string s, string colorHex)
        {
            if (s.NullOrEmptyOrWhiteSpace())
            {
                return s;
            }

            return $"<color=#{colorHex}>{s}</color>";
        }

        /// <summary>
        /// Use rich text to colorize the string
        /// </summary>
        /// <param name="s"></param>
        /// <param name="builtInColorName"></param>
        /// <returns></returns>
        public static string Color(this string s, RichTextBuiltInColorNames builtInColorName)
        {
            if (s.NullOrEmptyOrWhiteSpace())
            {
                return s;
            }

            return $"<color={GetBuiltInColorName(builtInColorName)}>{s}</color>";
        }

        /// <summary>
        /// Use rich text to return a bold version of the input string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Bold(this string s)
        {
            if (s.NullOrEmptyOrWhiteSpace())
            {
                return s;
            }

            return $"<b>{s}</b>";
        }

        /// <summary>
        /// Use rich text to return an italic version of the input string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Italic(this string s)
        {
            if (s.NullOrEmptyOrWhiteSpace())
            {
                return s;
            }

            return $"<i>{s}</i>";
        }

        /// <summary>
        /// Use rich text to change the size of the string
        /// </summary>
        /// <param name="s"></param>
        /// <param name="textSize"></param>
        /// <returns></returns>
        public static string Size(this string s, int textSize)
        {
            if (s.NullOrEmptyOrWhiteSpace())
            {
                return s;
            }

            textSize = Math.Max(textSize, 1);
            return $"<size={textSize}>{s}</size>";
        }

        private static string GetBuiltInColorName(RichTextBuiltInColorNames builtInColorName)
        {
            string colorName = null;
            switch (builtInColorName)
            {
                case RichTextBuiltInColorNames.black:
                    colorName = Enum.GetName(typeof(RichTextBuiltInColorNames), 0);
                    break;
                case RichTextBuiltInColorNames.white:
                    colorName = Enum.GetName(typeof(RichTextBuiltInColorNames), 1);
                    break;
                case RichTextBuiltInColorNames.silver:
                    colorName = Enum.GetName(typeof(RichTextBuiltInColorNames), 2);
                    break;
                case RichTextBuiltInColorNames.grey:
                    colorName = Enum.GetName(typeof(RichTextBuiltInColorNames), 3);
                    break;
                case RichTextBuiltInColorNames.brown:
                    colorName = Enum.GetName(typeof(RichTextBuiltInColorNames), 4);
                    break;
                case RichTextBuiltInColorNames.blue:
                    colorName = Enum.GetName(typeof(RichTextBuiltInColorNames), 5);
                    break;
                case RichTextBuiltInColorNames.lightblue:
                    colorName = Enum.GetName(typeof(RichTextBuiltInColorNames), 6);
                    break;
                case RichTextBuiltInColorNames.darkblue:
                    colorName = Enum.GetName(typeof(RichTextBuiltInColorNames), 7);
                    break;
                case RichTextBuiltInColorNames.cyan:
                    colorName = Enum.GetName(typeof(RichTextBuiltInColorNames), 8);
                    break;
                case RichTextBuiltInColorNames.navy:
                    colorName = Enum.GetName(typeof(RichTextBuiltInColorNames), 9);
                    break;
                case RichTextBuiltInColorNames.green:
                    colorName = Enum.GetName(typeof(RichTextBuiltInColorNames), 10);
                    break;
                case RichTextBuiltInColorNames.lime:
                    colorName = Enum.GetName(typeof(RichTextBuiltInColorNames), 11);
                    break;
                case RichTextBuiltInColorNames.red:
                    colorName = Enum.GetName(typeof(RichTextBuiltInColorNames), 12);
                    break;
                case RichTextBuiltInColorNames.maroon:
                    colorName = Enum.GetName(typeof(RichTextBuiltInColorNames), 13);
                    break;
                case RichTextBuiltInColorNames.orange:
                    colorName = Enum.GetName(typeof(RichTextBuiltInColorNames), 14);
                    break;
                case RichTextBuiltInColorNames.yellow:
                    colorName = Enum.GetName(typeof(RichTextBuiltInColorNames), 15);
                    break;
                case RichTextBuiltInColorNames.purple:
                    colorName = Enum.GetName(typeof(RichTextBuiltInColorNames), 16);
                    break;
                case RichTextBuiltInColorNames.teal:
                    colorName = Enum.GetName(typeof(RichTextBuiltInColorNames), 17);
                    break;
                case RichTextBuiltInColorNames.olive:
                    colorName = Enum.GetName(typeof(RichTextBuiltInColorNames), 18);
                    break;
            }
            return colorName;
        }
    }

}
