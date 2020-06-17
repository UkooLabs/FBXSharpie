using System;
using System.IO;
using UkooLabs.FbxSharpie;
using UkooLabs.FbxSharpie.Tests.Helpers;

namespace QuickTest
{
	class Program
    {
		public static void Test()
		{
			var filename = "plane-multi-uvs.fbx";
			var testFile = Path.Combine(PathHelper.FilesPath, filename);

			var documentNode = FbxIO.Read(testFile, ErrorLevel.Strict);
			var geometryIds = documentNode.GetGeometryIds();

			foreach (var geometryId in geometryIds)
			{
				var vertexIndices = documentNode.GetVertexIndices(geometryId);
				var positions = documentNode.GetPositions(geometryId, vertexIndices);

				var normalLayerIndices = documentNode.GetLayerIndices(geometryId, FbxLayerElementType.Normal);
				var tangentLayerIndices = documentNode.GetLayerIndices(geometryId, FbxLayerElementType.Tangent);
				var binormalLayerIndices = documentNode.GetLayerIndices(geometryId, FbxLayerElementType.Binormal);
				var texCoordLayerIndices = documentNode.GetLayerIndices(geometryId, FbxLayerElementType.TexCoord);
				var materialLayerIndices = documentNode.GetLayerIndices(geometryId, FbxLayerElementType.Material);

				var normals = documentNode.GetNormals(geometryId, vertexIndices, normalLayerIndices[0]);
				var tangents = documentNode.GetTangents(geometryId, vertexIndices, tangentLayerIndices[0]);
				var binormals = documentNode.GetBinormals(geometryId, vertexIndices, binormalLayerIndices[0]);
				var texCoords = documentNode.GetTexCoords(geometryId, vertexIndices, texCoordLayerIndices[0]);
				var materials = documentNode.GetMaterials(geometryId, vertexIndices, materialLayerIndices[0]);

				VertexDraw.Draw(1024, 1024, positions, @"D:\TestPoly.png");
			}
		}

		static void Main(string[] args)
        {
			Test();

			Console.WriteLine("Hello World!");
        }
    }
}
