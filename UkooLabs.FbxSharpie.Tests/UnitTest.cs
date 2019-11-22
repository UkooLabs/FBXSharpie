using System;
using System.IO;
using Xunit;
using UkooLabs.FbxSharpie;
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

			var documentNode = FbxIO.Read(testFile);
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
					var tangents = documentNode.GetBinormals(geometryId);
				}
				if (documentNode.GetGeometryHasMaterial(geometryId))
				{
					var materialName = documentNode.GetMaterialName(geometryId);
					var materialDiffuseColor = documentNode.GetMaterialDiffuseColor(geometryId);
				}
			}
		}

		private void CompareFiles(string filename)
		{
			var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			var testFile = Path.Combine(path, "Files", filename);
			var originalData = File.ReadAllBytes(testFile);
			var isBinary = FbxIO.IsBinaryFbx(testFile);
			var documentNode = FbxIO.Read(testFile);
			using (var newStream = new MemoryStream())
			{
				if (isBinary)
				{
					FbxIO.WriteBinary(documentNode, newStream);
				}
				else
				{
					FbxIO.WriteAscii(documentNode, newStream);
				}
				var newData = newStream.ToArray();

				if (originalData.Length != newData.Length)
				{
					var resultPath = Path.Combine(path, "FailedCompares");
					Directory.CreateDirectory(resultPath);
					File.WriteAllBytes(Path.Combine(resultPath, $"{Path.GetFileNameWithoutExtension(testFile)}-orig{Path.GetExtension(testFile)}"), originalData);
					File.WriteAllBytes(Path.Combine(resultPath, $"{Path.GetFileNameWithoutExtension(testFile)}-new{Path.GetExtension(testFile)}"), newData);
					Assert.True(false, $"Length expected {originalData.Length} but was {newData.Length}");
				}

				for (var i = 0; i < originalData.Length; i++)
				{
					Assert.True(originalData[i] == newData[i], $"Value expected {originalData[i]} but was {newData[i]}");
				}
			}
		}

		//[Theory]
		//[InlineData("cube-ascii-fbx2006.fbx")]
		//[InlineData("cube-ascii-fbx2009.fbx")]
		//[InlineData("cube-ascii-fbx2010.fbx")]
		//[InlineData("cube-ascii-fbx2011.fbx")]
		//[InlineData("cube-ascii-fbx2012.fbx")]
		//[InlineData("cube-ascii-fbx2013.fbx")]
		//[InlineData("cube-ascii-fbx2014-15.fbx")]
		//[InlineData("cube-ascii-fbx2016-17.fbx")]
		//[InlineData("cube-ascii-fbx2018.fbx")]
		//[InlineData("cube-ascii-fbx2019.fbx")]
		//public void TestIdenticalAsciiFbx(string filename)
		//{
		//	CompareFiles(filename);
		//}

		//[Theory]
		//[InlineData("cube-binary-fbx2006.fbx")]
		//[InlineData("cube-binary-fbx2009.fbx")]
		//[InlineData("cube-binary-fbx2010.fbx")]
		//[InlineData("cube-binary-fbx2011.fbx")]
		//[InlineData("cube-binary-fbx2012.fbx")]
		//[InlineData("cube-binary-fbx2013.fbx")]
		//[InlineData("cube-binary-fbx2014-15.fbx")]
		//[InlineData("cube-binary-fbx2016-17.fbx")]
		//[InlineData("cube-binary-fbx2018.fbx")]
		//[InlineData("cube-binary-fbx2019.fbx")]
		//public void TestIdenticalBinaryFbx(string filename)
		//{
		//	CompareFiles(filename);
		//}

		//[Fact]
		//public void test()
		//{
		//	var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		//	var testFile = Path.Combine(path, "Files", @"cube-ascii-tangent.fbx");
		//	var documentNode = FbxIO.Read(testFile);
		//	FbxIO.WriteBinary(documentNode, @"d:\ukoo.fbx");
		//	//https://github.com/jskorepa/fbx/blob/master/src/fbxdocument.cpp
		//}
	}
}
