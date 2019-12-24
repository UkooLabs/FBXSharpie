using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace UkooLabs.FbxSharpie.Tokens.ValueArray
{
	internal class ByteArrayToken : Token
	{
		public List<byte> Values { get; }

		internal override void WriteBinary(FbxVersion version, BinaryWriter binaryWriter)
		{
			binaryWriter.Write((byte)'R');
			binaryWriter.Write(Values.Count);
			binaryWriter.Write(Values.ToArray());	
		}

		internal override void WriteAscii(FbxVersion version, StringBuilder stringBuilder, int indentLevel, ref int lineStart)
		{
			var arrayLength = Values.Count;
			WriteAsciiArray(version, stringBuilder, arrayLength, indentLevel, ref lineStart, (itemWriter, currentLineStart) =>
			{
				bool pFirst = true;
				foreach (var value in Values)
				{
					var stringValue = value.ToString();
					if (!pFirst)
					{
						stringBuilder.Append(',');
					}
					if ((stringBuilder.Length - currentLineStart) + stringValue.Length >= Settings.MaxLineLength)
					{
						stringBuilder.Append('\n');
						currentLineStart = stringBuilder.Length;
					}
					stringBuilder.Append(stringValue);
					pFirst = false;
				}
				return currentLineStart;
			});
		}

		public ByteArrayToken() : base(TokenTypeEnum.ValueArray, ValueTypeEnum.Byte)
		{
			Values = new List<byte>();
		}

		public ByteArrayToken(byte[] values) : base(TokenTypeEnum.ValueArray, ValueTypeEnum.Byte)
		{
			Values = new List<byte>(values);
		}
	}
}
