# FBX Sharpie

based upon FBXWriter by Hamish Milne

https://github.com/hamish-milne/FbxWriter

```csharp
using UkooLabs.FbxSharpie;

class FbxExample
{
	static void Main(string[] args)
	{
		var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		var testFile = Path.Combine(path, "mug-ascii.fbx.fbx");
		var documentNode = FbxIO.Read(testFile);
		var scaleFactor = documentNode.GetScaleFactor();
		var geometryIds = documentNode.GetGeometryIds();
		foreach (var geometryId in geometryIds)
		{
			var materialName = documentNode.GetMaterialName(geometryId);
			var diffuseColor = documentNode.GetDiffuseColor(geometryId);
			var vertexIndices = documentNode.GetVertexIndices(geometryId);
			var vertices = documentNode.GetVertices(gGetVertexIndiceseometryId);
			var normals = documentNode.GetNormals(geometryId);
			var tangents = documentNode.GetTangents(geometryId);
			var texCoords = documentNode.GetTexCoords(geometryId);
		}
	}
}
```
