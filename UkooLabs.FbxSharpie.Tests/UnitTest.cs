using System.IO;
using Xunit;
using System.Reflection;

namespace UkooLabs.FbxSharpie.Tests
{
	public class UnitTest
    {
		[Theory]
		[InlineData("mug-ascii.fbx", false, 1.0d, true, true)]
		[InlineData("cube-ascii-notangent.fbx", false, 1.0d, false, false)]
		[InlineData("cube-ascii-nouv.fbx", false, 1.0d, false, false)]
		[InlineData("cube-ascii-tangent.fbx", false, 1.0d, true, true)]
		[InlineData("mug-binary.fbx", true, 2.54d, true, true)]
		[InlineData("cube-binary-notangent.fbx", true, 1.0d, false, false)]
		[InlineData("cube-binary-nouv.fbx", true, 1.0d, false, false)]
		[InlineData("cube-binary-tangent.fbx", true, 1.0d, true, true)]
		[InlineData("cube-ascii-fbx2006.fbx", false, 1.0d, true, true)]
		[InlineData("cube-ascii-fbx2009.fbx", false, 1.0d, true, true)]
		[InlineData("cube-ascii-fbx2010.fbx", false, 1.0d, true, true)]
		[InlineData("cube-ascii-fbx2011.fbx", false, 1.0d, true, true)]
		[InlineData("cube-ascii-fbx2012.fbx", false, 1.0d, true, true)]
		[InlineData("cube-ascii-fbx2013.fbx", false, 1.0d, true, true)]
		[InlineData("cube-ascii-fbx2014-15.fbx", false, 1.0d, true, true)]
		[InlineData("cube-ascii-fbx2016-17.fbx", false, 1.0d, true, true)]
		[InlineData("cube-ascii-fbx2018.fbx", false, 1.0d, true, true)]
		[InlineData("cube-ascii-fbx2019.fbx", false, 1.0d, true, true)]
		[InlineData("cube-binary-fbx2006.fbx", true, 1.0d, true, true)]
		[InlineData("cube-binary-fbx2009.fbx", true, 1.0d, true, true)]
		[InlineData("cube-binary-fbx2010.fbx", true, 1.0d, true, true)]
		[InlineData("cube-binary-fbx2011.fbx", true, 1.0d, true, true)]
		[InlineData("cube-binary-fbx2012.fbx", true, 1.0d, true, true)]
		[InlineData("cube-binary-fbx2013.fbx", true, 1.0d, true, true)]
		[InlineData("cube-binary-fbx2014-15.fbx", true, 1.0d, true, true)]
		[InlineData("cube-binary-fbx2016-17.fbx", true, 1.0d, true, true)]
		[InlineData("cube-binary-fbx2018.fbx", true, 1.0d, true, true)]
		[InlineData("cube-binary-fbx2019.fbx", true, 1.0d, true, true)]
		public void TestFbx(string filename, bool expectedIsBinary,  double expectedScaleFacor, bool expectedHasTangent, bool expectedHasBinormal)
		{
			var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			var testFile = Path.Combine(path, "Files", filename);
			Assert.True(expectedIsBinary == FbxIO.IsBinaryFbx(testFile), $"IsBinaryFbx expected {expectedIsBinary}");

			var documentNode = FbxIO.Read(testFile, ErrorLevel.Strict);
			var scaleFactor = documentNode.GetScaleFactor();
			Assert.True(expectedScaleFacor == scaleFactor, $"ScaleFactor expected {expectedScaleFacor}");

			var geometryIds = documentNode.GetGeometryIds();
			foreach (var geometryId in geometryIds)
			{
				var hasTangent = documentNode.GetGeometryHasTangents(geometryId);
				Assert.True(expectedHasTangent == hasTangent, $"HasTangent expected {expectedHasTangent}");

				var hasBinormal = documentNode.GetGeometryHasBinormals(geometryId);
				Assert.True(expectedHasBinormal == hasBinormal, $"HasBinormal expected {expectedHasBinormal}");

				var vertexIndices = documentNode.GetVertexIndices(geometryId);
				var vertices = documentNode.GetVertices(geometryId);
				var normals = documentNode.GetNormals(geometryId);
				var texCoords = documentNode.GetTexCoords(geometryId);
				if (documentNode.GetGeometryHasTangents(geometryId))
				{
					var tangents = documentNode.GetTangents(geometryId);
				}
				if (documentNode.GetGeometryHasBinormals(geometryId))
				{
					var binormals = documentNode.GetBinormals(geometryId);
				}
				if (documentNode.GetGeometryHasMaterial(geometryId))
				{
					var materialName = documentNode.GetMaterialName(geometryId);
					var materialDiffuseColor = documentNode.GetMaterialDiffuseColor(geometryId);
				}
			}
		}

		private void CompareBinaryFiles(string filename)
		{
			var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			var testFile = Path.Combine(path, "Files", filename);
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

		private string FilterLine(string line)
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

		private void CompareAsciiFiles(string filename)
		{
			var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			var testFile = Path.Combine(path, "Files", filename);
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

		[Theory]
		[InlineData("cube-ascii-fbx2006.fbx")]
		[InlineData("cube-ascii-fbx2009.fbx")]
		[InlineData("cube-ascii-fbx2010.fbx")]
		[InlineData("cube-ascii-fbx2011.fbx")]
		[InlineData("cube-ascii-fbx2012.fbx")]
		[InlineData("cube-ascii-fbx2013.fbx")]
		[InlineData("cube-ascii-fbx2014-15.fbx")]
		[InlineData("cube-ascii-fbx2016-17.fbx")]
		[InlineData("cube-ascii-fbx2018.fbx")]
		[InlineData("cube-ascii-fbx2019.fbx")]
		public void TestCompareAsciiFbx(string filename)
		{
			CompareAsciiFiles(filename);
		}

		[Theory]
		[InlineData("cube-binary-fbx2006.fbx")]
		[InlineData("cube-binary-fbx2009.fbx")]
		[InlineData("cube-binary-fbx2010.fbx")]
		[InlineData("cube-binary-fbx2011.fbx")]
		[InlineData("cube-binary-fbx2012.fbx")]
		[InlineData("cube-binary-fbx2013.fbx")]
		[InlineData("cube-binary-fbx2014-15.fbx")]
		[InlineData("cube-binary-fbx2016-17.fbx")]
		[InlineData("cube-binary-fbx2018.fbx")]
		[InlineData("cube-binary-fbx2019.fbx")]
		public void TestCompareBinaryFbx(string filename)
		{
			CompareBinaryFiles(filename);
		}
	}
}
