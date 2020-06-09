using System.IO;
using System.Reflection;
using Xunit;

namespace UkooLabs.FbxSharpie.Tests.Helpers
{
	public static class Compare
	{
		public static void CompareBinaryFiles(string filename)
		{
			var testFile = Path.Combine(PathHelper.FilesPath, filename);
			var originalData = File.ReadAllBytes(testFile);
			var isBinary = FbxIO.IsBinaryFbx(testFile);
			Assert.True(isBinary);
			var documentNode = FbxIO.Read(testFile);
			using (var newStream = new MemoryStream())
			{
				FbxIO.WriteBinary(documentNode, newStream);
				var newData = newStream.ToArray();

				Assert.True(newData.Length <= originalData.Length, $"Unexpected size comparisson");

				var identical = true;
				for (var i = 0; i < newData.Length; i++)
				{
					if (originalData[i] != newData[i])
					{
						identical = false;
						break;
					}
				}

				Assert.True(identical, $"Files data did not match as expected");
			}
		}

		private static string FilterLine(string line)
		{
			if (line.StartsWith("; FBX"))
			{
				return line;
			}
			if (line.StartsWith("; "))
			{
				return string.Empty;
			}
			return line.Replace(" ", "").Replace("\t", "");
		}

		public static void CompareAsciiFiles(string filename)
		{
			var testFile = Path.Combine(PathHelper.FilesPath, filename);
			var isBinary = FbxIO.IsBinaryFbx(testFile);
			Assert.False(isBinary);
			var documentNode = FbxIO.Read(testFile);
			using (var tempStream = new MemoryStream())
			{
				FbxIO.WriteAscii(documentNode, tempStream);
				tempStream.Position = 0;

				var originalBuffer = string.Empty;
				using (StreamReader originalStream = new StreamReader(testFile))
				{
					while (originalStream.EndOfStream)
					{
						originalBuffer += FilterLine(originalStream.ReadLine());
					}
				}

				var newBuffer = string.Empty;
				using (StreamReader newStream = new StreamReader(tempStream))
				{
					while (newStream.EndOfStream)
					{
						newBuffer += FilterLine(newStream.ReadLine());
					}
				}

				Assert.True(originalBuffer.Length == newBuffer.Length, $"Unexpected size comparisson");

				var identical = true;
				for (var i = 0; i < newBuffer.Length; i++)
				{
					if (originalBuffer[i] != newBuffer[i])
					{
						identical = false;
						break;
					}
				}

				Assert.True(identical, $"Files data did not match as expected");
			}
		}
	}
}
