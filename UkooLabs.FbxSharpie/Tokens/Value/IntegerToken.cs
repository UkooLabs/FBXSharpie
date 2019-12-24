using System.IO;
using System.Text;

namespace UkooLabs.FbxSharpie.Tokens.Value
{
	internal class IntegerToken : Token
	{
		public int Value { get; set; }

		internal override void WriteBinary(FbxVersion version, BinaryWriter binaryWriter)
		{
			binaryWriter.Write((byte)('I'));
			binaryWriter.Write(Value);
		}

		internal override void WriteAscii(FbxVersion version, StringBuilder stringBuilder, int indentLevel, ref int lineStart)
		{
			stringBuilder.Append(Value);
		}

		public override bool Equals(object obj)
		{
			if (obj is IntegerToken id)
			{
				return Value == id.Value;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public IntegerToken(int value) : base(TokenTypeEnum.Value, ValueTypeEnum.Integer)
		{
			Value = value;
		}
	}
}
