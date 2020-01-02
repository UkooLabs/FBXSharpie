using System;
using System.Linq;
using UkooLabs.FbxSharpie.Tokens;
using UkooLabs.FbxSharpie.Tokens.Value;
using UkooLabs.FbxSharpie.Tokens.ValueArray;

namespace UkooLabs.FbxSharpie.Extensions
{
	public static class TokenExtension
	{
		public static string GetAsString(this Token token)
		{
			if (!TryGetAsString(token, out var result))
			{
				throw new NotSupportedException();
			}
			return result;
		}

		public static bool TryGetAsString(this Token token, out string result)
		{
			if (token.ValueType == ValueTypeEnum.None)
			{
				if (token is StringToken stringToken)
				{
					result = stringToken.Value;
					return true;
				}
			}
			else if (token.TokenType == TokenTypeEnum.Value && token.ValueType != ValueTypeEnum.None)
			{
				if (token is BooleanToken booleanToken)
				{
					result = booleanToken.Value ? "T" : "F";
					return true;
				}
				if (token is ShortToken shortToken)
				{
					result = shortToken.Value.ToString();
					return true;
				}
				if (token is IntegerToken integerToken)
				{
					result = integerToken.Value.ToString();
					return true;
				}
				if (token is LongToken longToken)
				{
					result = longToken.Value.ToString();
					return true;
				}
				if (token is FloatToken floatToken)
				{
					result = floatToken.Value.ToString();
					return true;
				}
				if (token is DoubleToken doubleToken)
				{
					result = doubleToken.Value.ToString();
					return true;
				}
			}
			result = null;
			return false;
		}

		public static float GetAsFloat(this Token token)
		{
			if (!TryGetAsFloat(token, out var result))
			{
				throw new NotSupportedException();
			}
			return result;
		}

		public static bool TryGetAsFloat(this Token token, out float result)
		{
			if (token.TokenType == TokenTypeEnum.Value && token.ValueType != ValueTypeEnum.None)
			{
				if (token is BooleanToken booleanToken)
				{
					result = booleanToken.Value ? 1 : 0;
					return true;
				}
				if (token is ShortToken shortToken)
				{
					result = shortToken.Value;
					return true;
				}
				if (token is IntegerToken integerToken)
				{
					result = integerToken.Value;
					return true;
				}
				if (token is LongToken longToken)
				{
					result = longToken.Value;
					return true;
				}
				if (token is FloatToken floatToken)
				{
					result = floatToken.Value;
					return true;
				}
				if (token is DoubleToken doubleToken)
				{
					result = (float)doubleToken.Value;
					return true;
				}
			}
			result = 0;
			return false;
		}

		public static double GetAsDouble(this Token token)
		{
			if (!TryGetAsDouble(token, out var result))
			{
				throw new NotSupportedException();
			}
			return result;
		}

		public static bool TryGetAsDouble(this Token token, out double result)
		{
			if (token.TokenType == TokenTypeEnum.Value && token.ValueType != ValueTypeEnum.None)
			{
				if (token is BooleanToken booleanToken)
				{
					result = booleanToken.Value ? 1 : 0;
					return true;
				}
				if (token is ShortToken shortToken)
				{
					result = shortToken.Value;
					return true;
				}
				if (token is IntegerToken integerToken)
				{
					result = integerToken.Value;
					return true;
				}
				if (token is LongToken longToken)
				{
					result = longToken.Value;
					return true;
				}
				if (token is FloatToken floatToken)
				{
					result = floatToken.Value;
					return true;
				}
				if (token is DoubleToken doubleToken)
				{
					result = doubleToken.Value;
					return true;
				}
			}
			result = 0;
			return false;
		}

		public static double[] GetAsDoubleArray(this Token token)
		{
			if (!TryGetAsDoubleArray(token, out var result))
			{
				throw new NotSupportedException();
			}
			return result;
		}

		public static bool TryGetAsDoubleArray(this Token token, out double[] result)
		{
			if (token.TokenType == TokenTypeEnum.ValueArray && token.ValueType != ValueTypeEnum.None)
			{
				if (token is BooleanArrayToken booleanArrayToken)
				{
					result = (from item in booleanArrayToken.Values select (double)(item ? 1 : 0)).ToArray();
					return true;
				}
				if (token is ByteArrayToken byteArrayToken)
				{
					result = (from item in byteArrayToken.Values select (double)item).ToArray();
					return true;
				}
				if (token is IntegerArrayToken integerArrayToken)
				{
					result = (from item in integerArrayToken.Values select (double)item).ToArray();
					return true;
				}
				if (token is LongArrayToken longArrayToken)
				{
					result = (from item in longArrayToken.Values select (double)item).ToArray();
					return true;
				}
				if (token is FloatArrayToken floatArrayToken)
				{
					result = (from item in floatArrayToken.Values select (double)item).ToArray();
					return true;
				}
				if (token is DoubleArrayToken doubleArrayToken)
				{
					result = doubleArrayToken.Values;
					return true;
				}
			}
			result = null;
			return false;
		}

		public static long GetAsLong(this Token token)
		{
			if (!TryGetAsLong(token, out var result))
			{
				throw new NotSupportedException();
			}
			return result;
		}

		public static bool TryGetAsLong(this Token token, out long result)
		{
			if (token.TokenType == TokenTypeEnum.Value && token.ValueType != ValueTypeEnum.None)
			{
				if (token is BooleanToken booleanToken)
				{
					result = booleanToken.Value ? 1 : 0;
					return true;
				}
				if (token is ShortToken shortToken)
				{
					result = shortToken.Value;
					return true;
				}
				if (token is IntegerToken integerToken)
				{
					result = integerToken.Value;
					return true;
				}
				if (token is LongToken longToken)
				{
					result = longToken.Value;
					return true;
				}
				if (token is FloatToken floatToken)
				{
					result = (long)floatToken.Value;
					return true;
				}
				if (token is DoubleToken doubleToken)
				{
					result = (long)doubleToken.Value;
					return true;
				}
			}
			result = 0;
			return false;
		}

		public static int[] GetAsIntArray(this Token token)
		{
			if (!TryGetAsIntArray(token, out var result))
			{
				throw new NotSupportedException();
			}
			return result;
		}

		public static bool TryGetAsIntArray(this Token token, out int[] result)
		{
			if (token.TokenType == TokenTypeEnum.ValueArray && token.ValueType != ValueTypeEnum.None)
			{
				if (token is BooleanArrayToken booleanArrayToken)
				{
					result = (from item in booleanArrayToken.Values select item ? 1 : 0).ToArray();
					return true;
				}
				if (token is ByteArrayToken byteArrayToken)
				{
					result = (from item in byteArrayToken.Values select (int)item).ToArray();
					return true;
				}
				if (token is IntegerArrayToken integerArrayToken)
				{
					result = integerArrayToken.Values.ToArray();
					return true;
				}
				if (token is LongArrayToken longArrayToken)
				{
					result = (from item in longArrayToken.Values select (int)item).ToArray();
					return true;
				}
				if (token is FloatArrayToken floatArrayToken)
				{
					result = (from item in floatArrayToken.Values select (int)item).ToArray();
					return true;
				}
				if (token is DoubleArrayToken doubleArrayToken)
				{
					result = (from item in doubleArrayToken.Values select (int)item).ToArray();
					return true;
				}
			}
			result = null;
			return false;
		}

		public static float[] GetAsFloatArray(this Token token)
		{
			if (!TryGetAsFloatArray(token, out var result))
			{
				throw new NotSupportedException();
			}
			return result;
		}

		public static bool TryGetAsFloatArray(this Token token, out float[] result)
		{
			if (token.TokenType == TokenTypeEnum.ValueArray && token.ValueType != ValueTypeEnum.None)
			{
				if (token is BooleanArrayToken booleanArrayToken)
				{
					var values = booleanArrayToken.Values.ToArray();
					result = (from item in values select (float)(item ? 1 : 0)).ToArray();
					return true;
				}
				if (token is ByteArrayToken byteArrayToken)
				{
					var values = byteArrayToken.Values.ToArray();
					result = (from item in values select (float)item).ToArray();
					return true;
				}
				if (token is IntegerArrayToken integerArrayToken)
				{
					var values = integerArrayToken.Values.ToArray();
					result = (from item in values select (float)item).ToArray();
					return true;
				}
				if (token is LongArrayToken longArrayToken)
				{
					var values = longArrayToken.Values.ToArray();
					result = (from item in values select (float)item).ToArray();
					return true;
				}
				if (token is FloatArrayToken floatArrayToken)
				{
					result = floatArrayToken.Values.ToArray();
					return true;
				}
				if (token is DoubleArrayToken doubleArrayToken)
				{
					var values = doubleArrayToken.Values.ToArray();
					result = (from item in values select (float)item).ToArray();
					return true;
				}
			}
			result = null;
			return false;
		}

		public static long[] GetAsLongArray(this Token token)
		{
			if (!TryGetAsLongArray(token, out var result))
			{
				throw new NotSupportedException();
			}
			return result;
		}

		public static bool TryGetAsLongArray(this Token token, out long[] result)
		{
			if (token.TokenType == TokenTypeEnum.ValueArray && token.ValueType != ValueTypeEnum.None)
			{
				if (token is BooleanArrayToken booleanArrayToken)
				{
					var values = booleanArrayToken.Values.ToArray();
					result = (from item in values select (long)(item ? 1 : 0)).ToArray();
					return true;
				}
				if (token is ByteArrayToken byteArrayToken)
				{
					var values = byteArrayToken.Values.ToArray();
					result = (from item in values select (long)item).ToArray();
					return true;
				}
				if (token is IntegerArrayToken integerArrayToken)
				{
					var values = integerArrayToken.Values.ToArray();
					result = (from item in values select (long)item).ToArray();
					return true;
				}
				if (token is LongArrayToken longArrayToken)
				{
					result = longArrayToken.Values.ToArray();
					return true;
				}
				if (token is FloatArrayToken floatArrayToken)
				{
					var values = floatArrayToken.Values.ToArray();
					result = (from item in values select (long)item).ToArray();
					return true;
				}
				if (token is DoubleArrayToken doubleArrayToken)
				{
					var values = doubleArrayToken.Values.ToArray();
					result = (from item in values select (long)item).ToArray();
					return true;
				}
			}
			result = null;
			return false;
		}

	}
}
