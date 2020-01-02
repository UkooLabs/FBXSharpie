using System.IO;
using System.Text;

namespace UkooLabs.FbxSharpie.Tokens.Value
{
	internal class FloatToken : Token
	{
		public float Value { get; set; }

		internal override void WriteBinary(FbxVersion version, BinaryWriter binaryWriter)
		{
			binaryWriter.Write((byte)('F'));
			binaryWriter.Write(Value);
		}

		internal override void WriteAscii(FbxVersion version, LineStringBuilder lineStringBuilder, int indentLevel)
		{
			lineStringBuilder.Append(Value.ToString());
		}

		public override bool Equals(object obj)
		{
			if (obj is FloatToken id)
			{
				return Value == id.Value;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public FloatToken(float value) : base(TokenTypeEnum.Value, ValueTypeEnum.Float)
		{
			Value = value;
		}
	}
}
