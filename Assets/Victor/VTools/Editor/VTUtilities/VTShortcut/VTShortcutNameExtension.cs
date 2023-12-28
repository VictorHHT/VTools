using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Victor.Tools
{
    public static class VTShortcutNameExtension
    {
        public static string ToFormattedString(this KeyCode code)
		{
			string name;

			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				if (s_OSXSpecificKeyCodeNames.TryGetValue(code, out name))
				{
					return name;
				}
			}
			else
			{
				if (s_OSXFallbackKeyCodenames.TryGetValue(code, out name))
				{
					return name;
				}
			}

			if (s_KeyCodeNames.TryGetValue(code, out name))
			{
				return name;
			}

			return code.ToString();
		}

		public static string ToFormattedString(this ShortcutModifiers modifier)
		{
			var sb = new StringBuilder();

			// Mac OSX prefers control(don't include ) -> option -> shift -> command -> (Individual Key Press)
			if (Application.platform == RuntimePlatform.OSXEditor)
			{
				if (modifier.HasAlt())
				{
					sb.Append("⌥");
				}

				if (modifier.HasShift())
				{
					sb.Append("⇧");
				}

				if (modifier.HasAction())
				{
					sb.Append("⌘");
				}
			}
			else
			{
                // Windows prefers Ctrl -> Alt -> Shift
                var keys = new List<string>();

                if (modifier.HasAction())
                {
                    keys.Add("Ctrl");
                }

                if (modifier.HasAlt())
                {
                    keys.Add("Alt");
                }

                if (modifier.HasShift())
                {
                    keys.Add("Shift");
                }

                sb.Append(string.Join("+", keys));
            }

			return sb.ToString();
		}

		static Dictionary<KeyCode, string> s_KeyCodeNames = new Dictionary<KeyCode, string>
		{
			{KeyCode.Exclaim, "!"},
			{KeyCode.DoubleQuote, "\""},
			{KeyCode.Hash, "#"},
			{KeyCode.Dollar, "$"},
			{KeyCode.Percent, "%"},
			{KeyCode.Ampersand, "&"},
			{KeyCode.Quote, "'"},
			{KeyCode.LeftParen, "("},
			{KeyCode.RightParen, ")"},
			{KeyCode.Asterisk, "*"},
			{KeyCode.Plus, "+"},
			{KeyCode.Comma, ","},
			{KeyCode.Minus, "-"},
			{KeyCode.Period, "."},
			{KeyCode.Slash, "/"},
			{KeyCode.Alpha0, "0"},
			{KeyCode.Alpha1, "1"},
			{KeyCode.Alpha2, "2"},
			{KeyCode.Alpha3, "3"},
			{KeyCode.Alpha4, "4"},
			{KeyCode.Alpha5, "5"},
			{KeyCode.Alpha6, "6"},
			{KeyCode.Alpha7, "7"},
			{KeyCode.Alpha8, "8"},
			{KeyCode.Alpha9, "9"},
			{KeyCode.Colon, ":"},
			{KeyCode.Semicolon, ";"},
			{KeyCode.Less, "<"},
			{KeyCode.Equals, "="},
			{KeyCode.Greater, ">"},
			{KeyCode.Question, "?"},
			{KeyCode.At, "@"},
			{KeyCode.LeftBracket, "["},
			{KeyCode.Backslash, "\\"},
			{KeyCode.RightBracket, "]"},
			{KeyCode.Caret, "^"},
			{KeyCode.Underscore, "_"},
			{KeyCode.BackQuote, "`"},
			{KeyCode.LeftCurlyBracket, "{"},
			{KeyCode.Pipe, "|"},
			{KeyCode.RightCurlyBracket, "}"},
			{KeyCode.Tilde, "~"},
			{KeyCode.Keypad0, "Num 0"},
			{KeyCode.Keypad1, "Num 1"},
			{KeyCode.Keypad2, "Num 2"},
			{KeyCode.Keypad3, "Num 3"},
			{KeyCode.Keypad4, "Num 4"},
			{KeyCode.Keypad5, "Num 5"},
			{KeyCode.Keypad6, "Num 6"},
			{KeyCode.Keypad7, "Num 7"},
			{KeyCode.Keypad8, "Num 8"},
			{KeyCode.Keypad9, "Num 9"},
			{KeyCode.KeypadPeriod, "Num ."},
			{KeyCode.KeypadDivide, "Num /"},
			{KeyCode.KeypadMultiply, "Num *"},
			{KeyCode.KeypadMinus, "Num -"},
			{KeyCode.KeypadPlus, "Num +"},
			{KeyCode.KeypadEnter, "Num Enter"},
			{KeyCode.KeypadEquals, "Num ="},
		};

		static Dictionary<KeyCode, string> s_OSXSpecificKeyCodeNames = new Dictionary<KeyCode, string>
		{
			{KeyCode.Backspace, "⌫"},
			{KeyCode.Tab, "⇥"},
			{KeyCode.Return, "↩"},
			{KeyCode.Escape, "⎋"},
			{KeyCode.Delete, "⌦"},
			{KeyCode.UpArrow, "↑"},
			{KeyCode.DownArrow, "↓"},
			{KeyCode.RightArrow, "→"},
			{KeyCode.LeftArrow, "←"},
			{KeyCode.Home, "↖"},
			{KeyCode.End, "↘"},
			{KeyCode.PageUp, "⇞"},
			{KeyCode.PageDown, "⇟"},
		};

		static Dictionary<KeyCode, string> s_OSXFallbackKeyCodenames = new Dictionary<KeyCode, string>
		{
			{KeyCode.Return, "Enter"},
			{KeyCode.Escape, "Esc"},
			{KeyCode.Delete, "Del"},
			{KeyCode.UpArrow, "Up Arrow"},
			{KeyCode.DownArrow, "Down Arrow"},
			{KeyCode.RightArrow, "Right Arrow"},
			{KeyCode.LeftArrow, "Left Arrow"},
			{KeyCode.PageUp, "Page Up"},
			{KeyCode.PageDown, "Page Down"},
		};
	}
}