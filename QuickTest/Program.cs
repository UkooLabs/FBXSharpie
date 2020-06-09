using System;
using System.IO;
using System.Numerics;
using System.Reflection;
using UkooLabs.FbxSharpie;
using UkooLabs.FbxSharpie.Tests.Helpers;

namespace QuickTest
{
    class Program
    {

		//https://github.com/schmidtgit/ComputationalGeometry
		//https://github.com/nem0/OpenFBX/blob/master/src/ofbx.cpp

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


				VertexDraw.Draw(1024, 1024, positions, vertexIndices, @"D:\TestPoly.png");

			}
		}


		static void Main(string[] args)
        {
			Test();

			Console.WriteLine("Hello World!");
        }
    }
}
