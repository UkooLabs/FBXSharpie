namespace UkooLabs.FbxSharpie.Tokens
{
	internal class CommentToken : Token
	{
		public readonly string Value;

		public CommentToken(string value) : base(TokenTypeEnum.Comment, ValueTypeEnum.None)
		{
			Value = value;
		}
	}
}
