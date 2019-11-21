# FBX Sharpie

based upon FBXWriter by Hamish Milne

https://github.com/hamish-milne/FbxWriter

[![NuGet](https://img.shields.io/nuget/v/UkooLabs.FbxSharpie.svg?style=flat)](https://www.nuget.org/packages/UkooLabs.FbxSharpie/)

```csharp
using UkooLabs.FbxSharpie;

class FbxExample
{
	static void Main(string[] args)
	{
		var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		var testFile = Path.Combine(path, "mug-ascii.fbx");
		var documentNode = FbxIO.Read(testFile);
		var scaleFactor = documentNode.GetScaleFactor();
		var geometryIds = documentNode.GetGeometryIds();
		foreach (var geometryId in geometryIds)
		{
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
```
