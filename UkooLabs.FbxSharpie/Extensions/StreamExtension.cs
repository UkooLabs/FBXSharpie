using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UkooLabs.FbxSharpie.Extensions
{
	internal static class StreamExtension
	{
		private static char GetChar(Stream stream, FbxAsciiFileInfo fbxAsciiFileInfo)
		{
			stream.Read(fbxAsciiFileInfo.Buffer, 0, 1);
			fbxAsciiFileInfo.PeekedChar = (char)fbxAsciiFileInfo.Buffer[0];
			return fbxAsciiFileInfo.PeekedChar;
		}

		public static char PeekChar(this Stream stream, FbxAsciiFileInfo fbxAsciiFileInfo)
		{
			if (fbxAsciiFileInfo.PeekedChar != '\0')
			{
				return fbxAsciiFileInfo.PeekedChar;
			}
			return GetChar(stream, fbxAsciiFileInfo);
		}

		public static char ReadChar(this Stream stream, FbxAsciiFileInfo fbxAsciiFileInfo)
		{
			char c;
			if (fbxAsciiFileInfo.PeekedChar != '\0')
			{
				c = fbxAsciiFileInfo.PeekedChar;
				fbxAsciiFileInfo.PeekedChar = '\0';
				return c;
			}
			c = GetChar(stream, fbxAsciiFileInfo);
			fbxAsciiFileInfo.Column++;
			return c;
		}

		public static bool IsEndOfStream(this Stream stream) => stream.Position == stream.Length;

		public static bool TryParseCommentToken(this Stream stream, FbxAsciiFileInfo fbxAsciiFileInfo, out string comment)
		{
			var c = PeekChar(stream, fbxAsciiFileInfo);

			if (c != ';')
			{
				comment = null;
				return false;
			}

			var stringBuilder = new StringBuilder();
			while (!c.IsLineEnd() && !stream.IsEndOfStream())
			{
				stringBuilder.Append(stream.ReadChar(fbxAsciiFileInfo));
				c = stream.PeekChar(fbxAsciiFileInfo);
			}
			comment = stringBuilder.ToString();
			return true;
		}

		public static bool TryParseWhiteSpaceToken(this Stream stream, FbxAsciiFileInfo fbxAsciiFileInfo, out string whitespace)
		{
			var c = PeekChar(stream, fbxAsciiFileInfo);

			if (!(char.IsWhiteSpace(c) || c.IsLineEnd()))
			{
				whitespace = null;
				return false;
			}

			var stringBuilder = new StringBuilder();
			while ((char.IsWhiteSpace(c) || c.IsLineEnd()) && !stream.IsEndOfStream())
			{
				if (stream.ReadChar(fbxAsciiFileInfo) == '\n')
				{
					fbxAsciiFileInfo.Column = 0;
					fbxAsciiFileInfo.Line++;
				}
				c = stream.PeekChar(fbxAsciiFileInfo);
			}
			whitespace = stringBuilder.ToString();
			return true;
		}

		public static bool TryParseLiteralToken(this Stream stream, FbxAsciiFileInfo fbxAsciiFileInfo, out string literal)
		{
			var c = PeekChar(stream, fbxAsciiFileInfo);

			if (c != '"')
			{
				literal = null;
				return false;
			}

			stream.ReadChar(fbxAsciiFileInfo);

			var stringBuilder = new StringBuilder();
			while (stream.PeekChar(fbxAsciiFileInfo) != '"')
			{
				stringBuilder.Append(stream.ReadChar(fbxAsciiFileInfo));
				if (stream.IsEndOfStream())
				{
					throw new FbxException(fbxAsciiFileInfo, "Unexpected end of stream; expecting end quote");
				}
			}

			stream.ReadChar(fbxAsciiFileInfo);

			literal = stringBuilder.ToString();
			return true;
		}

		public static bool TryParseIdentifierToken(this Stream stream, FbxAsciiFileInfo fbxAsciiFileInfo, out string identifier)
		{
			var c = PeekChar(stream, fbxAsciiFileInfo);

			if (!c.IsIdentifierChar())
			{
				identifier = null;
				return false;
			}

			var stringBuilder = new StringBuilder();
			while (c.IsIdentifierChar() && !stream.IsEndOfStream())
			{
				stringBuilder.Append(stream.ReadChar(fbxAsciiFileInfo));
				c = stream.PeekChar(fbxAsciiFileInfo);
			}

			identifier = stringBuilder.ToString();
			return true;
		}

		public static bool TryParseOperatorToken(this Stream stream, FbxAsciiFileInfo fbxAsciiFileInfo, out char op)
		{
			var c = PeekChar(stream, fbxAsciiFileInfo);

			if (!"{}*:,".Contains(c.ToString()))
			{
				op = '\0';
				return false;
			}
			op = stream.ReadChar(fbxAsciiFileInfo);
			return true;
		}

		public static bool TryParseNumberToken(this Stream stream, FbxAsciiFileInfo fbxAsciiFileInfo, out object number)
		{
			var c = PeekChar(stream, fbxAsciiFileInfo);

			var isFirst = true;
			if (!c.IsDigit(isFirst))
			{
				number = null;
				return false;
			}

			var stringBuilder = new StringBuilder();
			while (c.IsDigit(isFirst) && !stream.IsEndOfStream())
			{
				stringBuilder.Append(stream.ReadChar(fbxAsciiFileInfo));
				isFirst = false;
				c = stream.PeekChar(fbxAsciiFileInfo);
			}

			var value = stringBuilder.ToString();
			if (!value.TryParseNumberToken(out number))
			{
				throw new FbxException(fbxAsciiFileInfo, $"Invalid number '{value}'");
			}

			return true;
		}
	}
}
