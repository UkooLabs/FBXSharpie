using System;
using System.IO;
using Xunit;
using UkooLabs.FbxSharpie;
using System.Reflection;

namespace UkooLabs.FbxSharpie.Tests
{
    public class UnitTest
    {
        [Fact]
        public void TestLoadAsciiFbx()
        {
			var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			var testFile = Path.Combine(path, "Files", "mug-ascii.fbx");
			var documentNode = FbxIO.Read(testFile);
			var scaleFactor = documentNode.GetScaleFactor();
			var geometryIds = documentNode.GetGeometryIds();
			foreach (var geometryId in geometryIds)
			{
				var materialName = documentNode.GetMaterialName(geometryId);
				var diffuseColor = documentNode.GetDiffuseColor(geometryId);
				var vertexIndices = documentNode.GetVertexIndices(geometryId);
				var vertices = documentNode.GetVertices(geometryId);
				var normals = documentNode.GetNormals(geometryId);
				var tangents = documentNode.GetTangents(geometryId);
				var texCoords = documentNode.GetTexCoords(geometryId);
			}
		}

		[Fact]
		public void TestLoadBinaryFbx()
		{
			var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			var testFile = Path.Combine(path, "Files", "mug-binary.fbx");
			var documentNode = FbxIO.Read(testFile);
			var scaleFactor = documentNode.GetScaleFactor();
			var geometryIds = documentNode.GetGeometryIds();
			foreach (var geometryId in geometryIds)
			{
				var materialName = documentNode.GetMaterialName(geometryId);
				var diffuseColor = documentNode.GetDiffuseColor(geometryId);
				var vertexIndices = documentNode.GetVertexIndices(geometryId);
				var vertices = documentNode.GetVertices(geometryId);
				var normals = documentNode.GetNormals(geometryId);
				var tangents = documentNode.GetTangents(geometryId);
				var texCoords = documentNode.GetTexCoords(geometryId);
			}
		}
	}
}
