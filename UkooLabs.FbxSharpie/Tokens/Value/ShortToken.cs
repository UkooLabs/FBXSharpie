using System.IO;
using System.Text;

namespace UkooLabs.FbxSharpie.Tokens.Value
{
	internal class ShortToken : Token
	{
		public short Value { get; set; }

		internal override void WriteBinary(FbxVersion version, BinaryWriter binaryWriter)
		{
			binaryWriter.Write((byte)('Y'));
			binaryWriter.Write(Value);
		}

		internal override void WriteAscii(FbxVersion version, LineStringBuilder stringBuilder, int indentLevel)
		{
			stringBuilder.Append(Value.ToString());
		}

		public override bool Equals(object obj)
		{
			if (obj is LongToken id)
			{
				return Value == id.Value;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Value.GetHashCode();
		}

		public ShortToken(short value) : base(TokenTypeEnum.Value, ValueTypeEnum.Short)
		{
			Value = value;
		}
	}
}
