namespace UkooLabs.FbxSharpie.Tokens
{
	internal class CommentToken : Token
	{
		public readonly string Value;

		public CommentToken(string value) : base(TokenType.Comment, ValueType.None)
		{
			Value = value;
		}
	}
}
