using System.IO;
using System.Text;

namespace UkooLabs.FbxSharpie.Tokens
{
	public class IdentifierToken : Token
	{
		public readonly string Value;

		internal override void WriteBinary(FbxVersion version, BinaryWriter binaryWriter)
		{
			var bytes = Encoding.ASCII.GetBytes(Value ?? string.Empty);
			if (bytes.Length > byte.MaxValue)
			{
				throw new FbxException(binaryWriter.BaseStream.Position, "Identifier value is too long");
			}
			binaryWriter.Write((byte)bytes.Length);
			if (bytes.Length > 0)
			{
				binaryWriter.Write(bytes);
			}
		}

		internal override void WriteAscii(FbxVersion version, LineStringBuilder stringBuilder, int indentLevel)
		{
			stringBuilder.Append($"{Value}:");
		}

		public override bool Equals(object obj)
		{
			if (obj is IdentifierToken id)
			{
				return Value == id.Value;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Value?.GetHashCode() ?? 0;
		}

		public IdentifierToken(string value) : base(TokenTypeEnum.Identifier, ValueTypeEnum.None)
		{
			Value = value;
		}
	}
}
