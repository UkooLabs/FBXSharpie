using System.IO;

namespace UkooLabs.FbxSharpie.Tokens.Value
{
	public class BooleanToken : Token
	{
		public bool Value { get; set; }

		internal override void WriteBinary(FbxVersion version, BinaryWriter binaryWriter)
		{
			binaryWriter.Write((byte)('C'));
			binaryWriter.Write(Value ? 'T' : 'F');
		}

		internal override void WriteAscii(FbxVersion version, LineStringBuilder lineStringBuilder, int indentLevel)
		{
			lineStringBuilder.Append(Value ? "T" : "F");
		}

		public BooleanToken(bool value) : base(TokenType.Value, ValueType.Boolean)
		{
			Value = value;
		}
	}
}
