namespace UkooLabs.FbxSharpie.Tokens
{
	internal class Identifier : IToken
	{
		public readonly string Value;

		public override bool Equals(object obj)
		{
			if (obj is Identifier id)
			{
				return Value == id.Value;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return Value?.GetHashCode() ?? 0;
		}

		public Identifier(string value)
		{
			Value = value;
		}

		public override string ToString()
		{
			return Value + ":";
		}
	}
}
