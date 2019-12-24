using System;
using System.Collections.Generic;
using System.Text;

namespace UkooLabs.FbxSharpie
{
	public static class Settings
	{
		public static int CompressionThreshold { get; set; } = 1024;

		/// <summary>
		/// The maximum line length in characters when outputting arrays
		/// </summary>
		/// <remarks>
		/// Lines might end up being a few characters longer than this, visibly and otherwise,
		/// so don't rely on it as a hard limit in code!
		/// </remarks>
		public static int MaxLineLength { get; set; } = 260;
	}
}
