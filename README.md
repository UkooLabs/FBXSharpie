# FBX Sharpie

based upon FBXWriter by Hamish Milne

https://github.com/hamish-milne/FbxWriter

Current features...

- Read FBX binary files
- Read FBX ASCII files
- Write FBX binary files
- Write FBX ASCII files
- Auto format detection
- Store and manipulate raw FBX object data
- Higher level processing of FBX nodes (In progress)
- Supports FBX versions 2006, 2009, 2010, 2011, 2012, 2013, 2014-15, 2016-17, 2018, 2019
- Ability to extract triangulated verex, tangent, normal, binormal data by geometry
- Ability to extract material diffuse color and name

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
				var binormals = documentNode.GetBinormals(geometryId);
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
