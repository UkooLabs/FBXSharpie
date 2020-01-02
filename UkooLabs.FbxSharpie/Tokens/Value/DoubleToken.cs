using System.IO;
using System.Text;

namespace UkooLabs.FbxSharpie.Tokens.Value
{
	internal class DoubleToken : Token
	{
		public double Value { get; set; }

		internal override void WriteBinary(FbxVersion version, BinaryWriter binaryWriter)
		{
			binaryWriter.Write((byte)('D'));
			binaryWriter.Write(Value);
		}

		internal override void WriteAscii(FbxVersion version, LineStringBuilder stringBuilder, int indentLevel)
		{
			stringBuilder.Append(Value.ToString());
		}

		public override bool Equals(object obj)
		{
			if (obj is DoubleToken id)
			{
				return Value == id.Value;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public DoubleToken(double value) : base(TokenTypeEnum.Value, ValueTypeEnum.Double)
		{
			Value = value;
		}
	}
}
