using System.IO;
using Xunit;
using System.Reflection;
using System.Numerics;

namespace UkooLabs.FbxSharpie.Tests
{
	public class UnitTest
    {
		[Theory]
		[InlineData("mug-ascii.fbx", false, 1.0d, true, true, true)]
		[InlineData("mug-binary.fbx", true, 2.54d, true, true, true)]
		[InlineData("cube-ascii-notangent.fbx", false, 1.0d, true, false, false)]
		[InlineData("cube-ascii-nouv.fbx", false, 1.0d, false, false, false)]
		[InlineData("cube-ascii-tangent.fbx", false, 1.0d, true, true, true)]
		[InlineData("cube-binary-notangent.fbx", true, 1.0d, true, false, false)]
		[InlineData("cube-binary-nouv.fbx", true, 1.0d, false, false, false)]
		[InlineData("cube-binary-tangent.fbx", true, 1.0d, true, true, true)]
		[InlineData("cube-ascii-fbx2006.fbx", false, 1.0d, true, true, true)]
		[InlineData("cube-ascii-fbx2009.fbx", false, 1.0d, true, true, true)]
		[InlineData("cube-ascii-fbx2010.fbx", false, 1.0d, true, true, true)]
		[InlineData("cube-ascii-fbx2011.fbx", false, 1.0d, true, true, true)]
		[InlineData("cube-ascii-fbx2012.fbx", false, 1.0d, true, true, true)]
		[InlineData("cube-ascii-fbx2013.fbx", false, 1.0d, true, true, true)]
		[InlineData("cube-ascii-fbx2014-15.fbx", false, 1.0d, true, true, true)]
		[InlineData("cube-ascii-fbx2016-17.fbx", false, 1.0d, true, true, true)]
		[InlineData("cube-ascii-fbx2018.fbx", false, 1.0d, true, true, true)]
		[InlineData("cube-ascii-fbx2019.fbx", false, 1.0d, true, true, true)]
		[InlineData("cube-binary-fbx2006.fbx", true, 1.0d, true, true, true)]
		[InlineData("cube-binary-fbx2009.fbx", true, 1.0d, true, true, true)]
		[InlineData("cube-binary-fbx2010.fbx", true, 1.0d, true, true, true)]
		[InlineData("cube-binary-fbx2011.fbx", true, 1.0d, true, true, true)]
		[InlineData("cube-binary-fbx2012.fbx", true, 1.0d, true, true, true)]
		[InlineData("cube-binary-fbx2013.fbx", true, 1.0d, true, true, true)]
		[InlineData("cube-binary-fbx2014-15.fbx", true, 1.0d, true, true, true)]
		[InlineData("cube-binary-fbx2016-17.fbx", true, 1.0d, true, true, true)]
		[InlineData("cube-binary-fbx2018.fbx", true, 1.0d, true, true, true)]
		[InlineData("cube-binary-fbx2019.fbx", true, 1.0d, true, true, true)]
		public void TestFbx(string filename, bool expectedIsBinary, double expectedScaleFacor, bool expectedHasTexCoord, bool expectedHasTangent, bool expectedHasBinormal)
		{
			var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			var testFile = Path.Combine(path, "Files", filename);
			Assert.True(expectedIsBinary == FbxIO.IsBinaryFbx(testFile), $"IsBinaryFbx expected {expectedIsBinary}");

			var documentNode = FbxIO.Read(testFile, ErrorLevel.Strict);
			var scaleFactor = documentNode.GetScaleFactor();
			Assert.True(expectedScaleFacor == scaleFactor, $"ScaleFactor expected {expectedScaleFacor}");

			var materialIds = documentNode.GetMaterialIds();

			var geometryIds = documentNode.GetGeometryIds();
			Assert.True(geometryIds.Length > 0);

			var fbxIndexer = new FbxIndexer();

			foreach (var geometryId in geometryIds)
			{
				var vertexIndices = documentNode.GetVertexIndices(geometryId);
				var positions = documentNode.GetPositions(geometryId, vertexIndices);
				var normals = documentNode.GetNormals(geometryId, vertexIndices);
				var tangents = documentNode.GetTangents(geometryId, vertexIndices);
				var binormals = documentNode.GetBinormals(geometryId, vertexIndices);
				var texCoords = documentNode.GetTexCoords(geometryId, vertexIndices);
				var materials = documentNode.GetMaterials(geometryId, vertexIndices);

				var hasNormals = documentNode.GetGeometryHasNormals(geometryId);

				var hasTexCoords = documentNode.GetGeometryHasTexCoords(geometryId);
				Assert.True(expectedHasTexCoord == hasTexCoords, $"HasTexCoord expected {expectedHasTexCoord}");

				var hasTangents = documentNode.GetGeometryHasTangents(geometryId);
				Assert.True(expectedHasTangent == hasTangents, $"HasTangent expected {expectedHasTangent}");

				var hasBinormals = documentNode.GetGeometryHasBinormals(geometryId);
				Assert.True(expectedHasBinormal == hasBinormals, $"HasBinormal expected {expectedHasBinormal}");

				var hasMaterials = documentNode.GetGeometryHasMaterials(geometryId);

				for (var i = 0; i < positions.Length; i++)
				{
					var vertex = new FbxVertex
					{
						Position = positions[i],
						Normal = hasNormals ? normals[i] : new Vector3(),
						Tangent = hasTangents ? tangents[i] : new Vector3(),
						Binormal = hasBinormals ? binormals[i] : new Vector3(),
						TexCoord = hasTexCoords ? texCoords[i] : new Vector2()
					};
					var materialId = hasMaterials ? materials[i] : 0;
					fbxIndexer.AddVertex(vertex, materialId);
				}
			}

			foreach (var materialId in materialIds)
			{
				var materialName = documentNode.GetMaterialName(materialId);
				var diffuseColor = documentNode.GetMaterialDiffuseColor(materialId);
				fbxIndexer.Index(materialId, out var indexedVertices, out var indexedIndices);
			}
		}

		[Theory]
		[InlineData("multi-material-ascii.fbx")]
		public void TestMaterialFbx(string filename)
		{
			var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			var testFile = Path.Combine(path, "Files", filename);

			var documentNode = FbxIO.Read(testFile, ErrorLevel.Strict);
			var scaleFactor = documentNode.GetScaleFactor();

			var materialIds = documentNode.GetMaterialIds();
			Assert.True(materialIds.Length > 0);

			foreach (var materialId in materialIds)
			{
				var materialName = documentNode.GetMaterialName(materialId);
				var diffuseColor = documentNode.GetMaterialDiffuseColor(materialId);
			}

			var geometryIds = documentNode.GetGeometryIds();
			Assert.True(geometryIds.Length > 0);

			foreach (var geometryId in geometryIds)
			{
				var vertexIndices = documentNode.GetVertexIndices(geometryId);
				var positions = documentNode.GetPositions(geometryId, vertexIndices);
				var tangents = documentNode.GetTangents(geometryId, vertexIndices);
				var binormals = documentNode.GetBinormals(geometryId, vertexIndices);
				var texCoords = documentNode.GetTexCoords(geometryId, vertexIndices);
				var materials = documentNode.GetMaterials(geometryId, vertexIndices);

				var hasTexCoords = documentNode.GetGeometryHasTexCoords(geometryId);
				var hasTangents = documentNode.GetGeometryHasTangents(geometryId);
				var hasBinormals = documentNode.GetGeometryHasBinormals(geometryId);
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
			Helpers.Compare.CompareAsciiFiles(filename);
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
			Helpers.Compare.CompareBinaryFiles(filename);
		}

		[Theory]
		[InlineData("mug-binary-compressed.fbx")]
		public void TestCompressedBinaryFbx(string filename)
		{
			Helpers.Compare.CompareBinaryFiles(filename);
		}
	}
}
