using System.IO;
using Xunit;
using UkooLabs.FbxSharpie.Tokens.Value;

namespace UkooLabs.FbxSharpie.Tests
{
	public class CodeCoverageTests
    {
		[Fact]
		public void TestBooleanTokenBinary()
		{
			using var memoryStream = new MemoryStream();
			using var binaryWriter = new BinaryWriter(memoryStream);
			var token = new BooleanToken(false);
			token.WriteBinary(FbxVersion.v6_0, binaryWriter);
			Assert.Equal(new byte[] { (byte)'C', (byte)'F' }, memoryStream.ToArray());
		}

		[Fact]
		public void TestBooleanTokenAscii()
		{
			var stringBuilder = new LineStringBuilder();
			var token = new BooleanToken(false);
			token.WriteAscii(FbxVersion.v6_0, stringBuilder, 0);
			Assert.Equal("F", stringBuilder.ToString());
		}
	}
}
