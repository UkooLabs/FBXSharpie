using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace UkooLabs.FbxSharpie.Tokens.ValueArray
{
	internal class BooleanArrayToken : Token
	{

		public List<bool> Values { get; }

		internal override void WriteBinary(FbxVersion version, BinaryWriter binaryWriter)
		{
			var count = Values.Count;
			binaryWriter.Write((byte)'b');
			binaryWriter.Write(count);
			var uncompressedSize = count * sizeof(byte);
			WriteBinaryArray(binaryWriter, uncompressedSize, (itemWriter) => {
				foreach (var value in Values)
				{
					itemWriter.Write(value);
				}
			});
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

		public BooleanArrayToken() : base(TokenTypeEnum.ValueArray, ValueTypeEnum.Boolean)
		{
			Values = new List<bool>();
		}

		public BooleanArrayToken(bool[] values) : base(TokenTypeEnum.ValueArray, ValueTypeEnum.Boolean)
		{
			Values = new List<bool>(values);
		}
	}
}
