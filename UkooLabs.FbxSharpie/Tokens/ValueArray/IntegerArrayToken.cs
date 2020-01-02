using System.IO;

namespace UkooLabs.FbxSharpie.Tokens.ValueArray
{
	internal class IntegerArrayToken : Token
	{
		public int[] Values { get; set; }

		internal override void WriteBinary(FbxVersion version, BinaryWriter binaryWriter)
		{
			var count = Values.Length;
			binaryWriter.Write((byte)'i');
			binaryWriter.Write(count);
			var uncompressedSize = count * sizeof(int);
			WriteBinaryArray(binaryWriter, uncompressedSize, (itemWriter) => {
				foreach (var value in Values)
				{
					itemWriter.Write(value);
				}
			});
		}

		internal override void WriteAscii(FbxVersion version, LineStringBuilder lineStringBuilder, int indentLevel)
		{
			var arrayLength = Values.Length;
			WriteAsciiArray(version, lineStringBuilder, arrayLength, indentLevel, (itemWriter) =>
			{
				for (var i = 0; i < Values.Length; i++)
				{
					if (i > 0)
					{
						lineStringBuilder.Append(",");
					}
					lineStringBuilder.Append(Values[i].ToString());
				}
			});
		}

		public IntegerArrayToken(int[] values) : base(TokenTypeEnum.ValueArray, ValueTypeEnum.Integer)
		{
			Values = values;
		}
	}
}
