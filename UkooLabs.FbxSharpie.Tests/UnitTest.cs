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

	}
}
