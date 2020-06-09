namespace UkooLabs.FbxSharpie.Tokens
{
	public class CommentToken : Token
	{
		public readonly string Value;

		public CommentToken(string value) : base(TokenType.Comment, ValueType.None)
		{
			Value = value;
		}
	}
}
