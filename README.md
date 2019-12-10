# FBX Sharpie

based upon FBXWriter by Hamish Milne

https://github.com/hamish-milne/FbxWriter

Current features...

- Read and Write FBX binary files
- Read and Write FBX ASCII files
- Auto format detection
- Store and manipulate raw FBX object data
- Higher level processing of FBX nodes (In progress)
- Supports FBX versions 2006, 2009, 2010, 2011, 2012, 2013, 2014-15, 2016-17, 2018, 2019
- Ability to extract triangulated verex, tangent, normal, binormal data by geometry
- Ability to extract material diffuse color and name

Todo...

- Re-indexing of vertex data
- Optimize and organize code
- Improve how data is tokenized and stored
- Expand functionality to include camera's, lighting, animation etc

[![NuGet](https://img.shields.io/nuget/v/UkooLabs.FbxSharpie.svg?style=flat)](https://www.nuget.org/packages/UkooLabs.FbxSharpie/)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=UkooLabs_FBXSharpie&metric=alert_status)](https://sonarcloud.io/dashboard?id=UkooLabs_FBXSharpie) [![Join the chat at https://gitter.im/UkooLabs/FBXSharpie](https://badges.gitter.im/UkooLabs/FBXSharpie.svg)](https://gitter.im/UkooLabs/FBXSharpie?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

```csharp
using UkooLabs.FbxSharpie;

class FbxExample
{
	static void Main(string[] args)
	{
		var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		var testFile = Path.Combine(path, "file.fbx");

		var isBinary = FbxIO.IsBinaryFbx(testFile);
		var documentNode = FbxIO.Read(testFile);

		// Scale factor usually 1 or 2.54
		var scaleFactor = documentNode.GetScaleFactor();

		var materialIds = documentNode.GetMaterialIds();
		var geometryIds = documentNode.GetGeometryIds();

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
			var hasTangents = documentNode.GetGeometryHasTangents(geometryId);
			var hasBinormals = documentNode.GetGeometryHasBinormals(geometryId);
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

		// Example to re-index geometry based on each material
		foreach (var materialId in materialIds)
		{
			var materialName = documentNode.GetMaterialName(materialId);
			var diffuseColor = documentNode.GetMaterialDiffuseColor(materialId);
			fbxIndexer.Index(materialId, out var indexedVertices, out var indexedIndices);
		}
	}
}
```
