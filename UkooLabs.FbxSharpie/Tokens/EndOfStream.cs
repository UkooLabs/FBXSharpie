using System;
using System.Collections.Generic;
using System.Text;

namespace UkooLabs.FbxSharpie.Tokens
{
	internal class EndOfStream : IToken
	{
		public override string ToString()
		{
			return "EndOfStream";
		}
	}
}
