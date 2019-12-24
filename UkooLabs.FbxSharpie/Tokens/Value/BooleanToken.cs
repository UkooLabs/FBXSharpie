using System.IO;
using System.Text;

namespace UkooLabs.FbxSharpie.Tokens.Value
{
	internal class BooleanToken : Token
	{
		public bool Value { get; set; }

		internal override void WriteBinary(FbxVersion version, BinaryWriter binaryWriter)
		{
			binaryWriter.Write((byte)('C'));
			binaryWriter.Write(Value ? 'T' : 'F');
		}

		internal override void WriteAscii(FbxVersion version, StringBuilder stringBuilder, int indentLevel, ref int lineStart)
		{
			stringBuilder.Append(Value ? 'T' : 'F');
		}

		public BooleanToken(bool value) : base(TokenTypeEnum.Value, ValueTypeEnum.Boolean)
		{
			Value = value;
		}
	}
}
