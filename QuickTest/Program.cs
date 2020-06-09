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
			var filename = "poly-test.fbx";
			var testFile = Path.Combine(PathHelper.FilesPath, filename);

			var documentNode = FbxIO.Read(testFile, ErrorLevel.Strict);
			var geometryIds = documentNode.GetGeometryIds();

			foreach (var geometryId in geometryIds)
			{
				var vertexIndices = documentNode.GetVertexIndices(geometryId);
				var positions = documentNode.GetPositions(geometryId, vertexIndices);
				var tangents = documentNode.GetTangents(geometryId, vertexIndices);
				var binormals = documentNode.GetBinormals(geometryId, vertexIndices);
				var texCoords = documentNode.GetTexCoords(geometryId, vertexIndices);
				var materials = documentNode.GetMaterials(geometryId, vertexIndices);

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
