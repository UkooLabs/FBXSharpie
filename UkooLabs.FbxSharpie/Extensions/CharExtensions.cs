using System;
using System.Collections.Generic;
using System.Text;

namespace UkooLabs.FbxSharpie.Extensions
{
	public static class CharExtensions
	{
		public static bool IsLineEnd(this char c)
		{
			return c == '\r' || c == '\n';
		}

		public static bool IsIdentifierChar(this char c)
		{
			return char.IsLetterOrDigit(c) || c == '_';
		}

		public static bool IsDigit(this char c, bool first)
		{
			if (char.IsDigit(c))
			{
				return true;
			}

			switch (c)
			{
				case '-':
				case '+':
					return true;
				case '.':
				case 'e':
				case 'E':
				case 'X':
				case 'x':
					return !first;
			}
			return false;
		}
	}
}
