using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UkooLabs.FbxSharpie.Tests.Helpers
{
	public static class PathHelper
	{
		public static string GetSolutionPath()
		{
			var path = AppDomain.CurrentDomain.BaseDirectory;
			while (!File.Exists(Path.Combine(path, "UkooLabs.FbxSharpie.sln")))
			{
				path = Path.GetFullPath(Path.Combine(path, ".."));
			}
			return path;
		}

		public static string SolutionPath => PathHelper.GetSolutionPath();

		public static string FilesPath => Path.Combine(SolutionPath, "Files");
	}
}
