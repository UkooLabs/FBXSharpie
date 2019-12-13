namespace UkooLabs.FbxSharpie.Extensions
{
	internal static class StringExtension
	{
		public static bool TryParseNumberToken(this string value, out object number)
		{
			if (value.Contains("."))
			{
				var splitValue = value.Split('.', 'e', 'E');
				if (splitValue.Length > 1 && splitValue[1].Length > 6)
				{
					if (!double.TryParse(value, out var d))
					{
						number = null;
						return false;
					}
					number = d;
					return true;
				}
				if (!float.TryParse(value, out var f))
				{
					number = null;
					return false;
				}
				number = f;
				return true;
			}
			if (!long.TryParse(value, out var l))
			{
				number = null;
				return false;
			}
			if (l >= byte.MinValue && l <= byte.MaxValue)
			{
				number = (byte)l;
				return true;
			}
			if (l >= int.MinValue && l <= int.MaxValue)
			{
				number = (int)l;
				return true;
			}
			number = l;
			return true;
		}

	}
}
