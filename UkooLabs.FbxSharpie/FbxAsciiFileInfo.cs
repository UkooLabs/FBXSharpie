using System;
using System.Collections.Generic;
using System.Text;

namespace UkooLabs.FbxSharpie
{
	public class FbxAsciiFileInfo
	{
		public int Line { get; set; }

		public int Column { get; set; }

		public byte[] Buffer { get; }

		public char PeekedChar { get; set; }

		public FbxAsciiFileInfo()
		{
			Line = 1;
			Column = 0;
			Buffer = new byte[1];
		}
	}
}
