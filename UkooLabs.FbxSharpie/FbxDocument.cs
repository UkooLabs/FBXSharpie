using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace UkooLabs.FbxSharpie
{
	/// <summary>
	/// A top-level FBX node
	/// </summary>
	public class FbxDocument : FbxNodeList
	{
		/// <summary>
		/// Describes the format and data of the document
		/// </summary>
		/// <remarks>
		/// It isn't recommended that you change this value directly, because
		/// it won't change any of the document's data which can be version-specific.
		/// Most FBX importers can cope with any version.
		/// </remarks>
		public FbxVersion Version { get; set; } = FbxVersion.v7_4;

		private string PropertiesName => Version >= FbxVersion.v7_0 ? "Properties70" : "Properties60";

		private string GeometryName => Version >= FbxVersion.v7_0 ? "Geometry" : "Model";

		private FbxNode GetNodeWithValue(FbxNode[] nodes, object value)
        {
            foreach (var node in nodes)
            {
                if (node == null)
                {
                    continue;
                }
                if (node.Value.AsObject.Equals(value))
                {
                    return node;
                }
            }
            return null;
        }

        private FbxNode[] GetFbxNodes(string name, FbxNodeList fbxNodeList)
        {
            var nodeList = new List<FbxNode>();
            foreach (var node in fbxNodeList.Nodes)
            {
                if (node == null)
                {
                    continue;
                }
                if (node.Name == name)
                {
                    nodeList.Add(node);
                }
                nodeList.AddRange(GetFbxNodes(name, node));
            }
            return nodeList.ToArray();
        }

        private bool HasConnection(long value1, long value2)
        {
            var connections = GetRelative("Connections").Nodes;
            return connections.Any(c => c != null && c.Properties[1].GetAsLong() == value1 && c.Properties[2].GetAsLong() == value2);
        }

        private int NormalizeIndex(int index)
        {
            return index < 0 ? (index + 1) * -1 : index;
        }

        private FbxNode[] GetMaterialNodesForGeometry(long geometryId)
        {
			var materialNodes = new List<FbxNode>();
            var models = GetFbxNodes("Model", this);
            var materials = GetFbxNodes("Material", this);
            foreach (var model in models)
            {
                var modelId = model.Value.GetAsLong();
                if (HasConnection(geometryId, modelId))
                {
                    foreach (var material in materials)
                    {
                        var materialId = material.Value.GetAsLong();
                        if (HasConnection(materialId, modelId))
                        {
                            materialNodes.Add(material);
                        }
                    }
                }
            }
            return materialNodes.ToArray();
        }





		public int[] GetVertexIndices(long geometryId)
		{
			var geometryNode = GetGeometry(geometryId);
			var polygonVertexIndexNode = geometryNode.GetRelative("PolygonVertexIndex");
			var vertexIndices = Version >= FbxVersion.v7_0 ? polygonVertexIndexNode.Value.GetAsIntArray() : polygonVertexIndexNode.PropertiesToIntArray();

			var result = new List<int>();

			var i = 0;
			while (i < vertexIndices.Length)
			{
				var quadMode = vertexIndices[i + 2] >= 0;
				if (quadMode)
				{
					result.Add(NormalizeIndex(vertexIndices[i]));
					result.Add(NormalizeIndex(vertexIndices[i + 1]));
					result.Add(NormalizeIndex(vertexIndices[i + 3]));
					result.Add(NormalizeIndex(vertexIndices[i + 3]));
					result.Add(NormalizeIndex(vertexIndices[i + 1]));
					result.Add(NormalizeIndex(vertexIndices[i + 2]));
					i += 4;
				}
				else
				{
					result.Add(NormalizeIndex(vertexIndices[i]));
					result.Add(NormalizeIndex(vertexIndices[i + 1]));
					result.Add(NormalizeIndex(vertexIndices[i + 2]));
					i += 3;
				}
			}
			return result.ToArray();
		}

		private FbxNode GetGeometry(long geometryId)
        {
            var geometryNodes = GetFbxNodes(GeometryName, this);
			return geometryNodes[geometryId];
        }

		private FbxNode GetMaterial(long materialId)
		{
			var materialNodes = GetFbxNodes("Material", this);
			return materialNodes[materialId];
		}





		private bool IsDirect(string value)
		{
			return string.Equals(value, "Direct", StringComparison.CurrentCultureIgnoreCase);
		}

		private bool IsIndex(string value)
		{
			return string.Equals(value, "Index", StringComparison.CurrentCultureIgnoreCase);
		}

		private bool IsIndexToDirect(string value)
		{
			return string.Equals(value, "IndexToDirect", StringComparison.CurrentCultureIgnoreCase);
		}

		private bool IsAllSame(string value)
		{
			return string.Equals(value, "AllSame", StringComparison.CurrentCultureIgnoreCase);
		}

		private bool IsByVertice(string value)
		{
			return string.Equals(value, "ByVertice", StringComparison.CurrentCultureIgnoreCase);
		}

		private bool IsByControlPoint(string value)
		{
			return string.Equals(value, "ByControlPoint", StringComparison.CurrentCultureIgnoreCase);
		}

		private bool IsByPolygonVertex(string value)
		{
			return string.Equals(value, "ByPolygonVertex", StringComparison.CurrentCultureIgnoreCase);
		}

		private bool IsByPolygon(string value)
		{
			return string.Equals(value, "ByPolygon", StringComparison.CurrentCultureIgnoreCase);
		}


		public Vector3 ToVector3(float[] values, int index)
		{
			var id = index * 3;
			return new Vector3(values[id], values[id + 1], values[id + 2]);
		}

		public Vector2 ToVector2(float[] values, int index)
		{
			var id = index * 2;
			return new Vector2(values[id], values[id + 1]);
		}

		private int ParseVertexIndex(int[] layerIndices, string mappingNode, string referenceMode, int controlPointIndex, int vertexindex)
		{
			if (IsByControlPoint(mappingNode))
			{
				if (IsDirect(referenceMode))
				{
					return controlPointIndex;
				}
				else if (IsIndex(referenceMode) || IsIndexToDirect(referenceMode))
				{
					return layerIndices[controlPointIndex];
				}
			}
			else if (IsByPolygonVertex(mappingNode) || IsByVertice(mappingNode))
			{
				if (IsDirect(referenceMode))
				{
					return vertexindex;
				}
				else if (IsIndex(referenceMode) || IsIndexToDirect(referenceMode))
				{
					return layerIndices[vertexindex];
				}
			}
			else if (IsByPolygon(mappingNode))
			{
				return vertexindex;
			}
			else if (IsAllSame(mappingNode))
			{
				return 0;
			}

			throw new NotSupportedException();
		}

		public Vector3 ParseVertexAsVector3(float[] layerValues, int[] layerIndices, string mappingMode, string referenceMode, int controlPointIndex, int vertexindex)
		{
			var index = ParseVertexIndex(layerIndices, mappingMode, referenceMode, controlPointIndex, vertexindex);
			return ToVector3(layerValues, index);
		}

		public Vector2 ParseVertexAsVector2(float[] layerValues, int[] layerIndices, string mappingMode, string referenceMode, int controlPointIndex, int vertexindex)
		{
			var index = ParseVertexIndex(layerIndices, mappingMode, referenceMode, controlPointIndex, vertexindex);
			return ToVector2(layerValues, index);
		}

		public int ParseVertexAsInt(int[] layerValues, int[] layerIndices, string mappingMode, string referenceMode, int controlPointIndex, int vertexindex)
		{
			var index = ParseVertexIndex(layerIndices, mappingMode, referenceMode, controlPointIndex, vertexindex);
			return layerValues[index];
		}

		public Vector3[] GetPositions(long geometryId, int[] vertexIndices)
		{
			var geometryNode = GetGeometry(geometryId);
			var verticesNode = geometryNode.GetRelative("Vertices");
			var vertices = Version >= FbxVersion.v7_0 ? verticesNode.Value.GetAsFloatArray() : verticesNode.PropertiesToFloatArray();
			var result = new List<Vector3>();
			for (var i = 0; i < vertexIndices.Length; i++)
			{
				result.Add(ToVector3(vertices, vertexIndices[i]));
			}
			return result.ToArray();
		}

		private void GetLayerFloatValues(long geometryId, string layerElement, string layerName, string layerIndexName, out float[] layerValues, out int[] layerIndices, out string mappingMode, out string referenceMode)
		{
			var geometryNode = GetGeometry(geometryId);
			var layerNode = geometryNode?.GetRelative(layerElement);
			var layerTypeNode = layerNode?.GetRelative(layerName);
			layerValues = Version >= FbxVersion.v7_0 ? layerTypeNode?.Value.GetAsFloatArray() : layerTypeNode?.PropertiesToFloatArray();
			var layerIndicesNode = layerNode?.GetRelative(layerIndexName);
			layerIndices = Version >= FbxVersion.v7_0 ? layerIndicesNode?.Value.GetAsIntArray() : layerIndicesNode?.PropertiesToIntArray();
			mappingMode = layerNode?.GetRelative("MappingInformationType")?.Value.AsString;
			referenceMode = layerNode?.GetRelative("ReferenceInformationType")?.Value.AsString;
		}

		private void GetLayerIntValues(long geometryId, string layerElement, string layerName, string layerIndexName, out int[] layerValues, out int[] layerIndices, out string mappingMode, out string referenceMode)
		{
			var geometryNode = GetGeometry(geometryId);
			var layerNode = geometryNode?.GetRelative(layerElement);
			var layerTypeNode = layerNode?.GetRelative(layerName);
			layerValues = Version >= FbxVersion.v7_0 ? layerTypeNode?.Value.GetAsIntArray() : layerTypeNode?.PropertiesToIntArray();
			var layerIndicesNode = layerNode?.GetRelative(layerIndexName);
			layerIndices = Version >= FbxVersion.v7_0 ? layerIndicesNode?.Value.GetAsIntArray() : layerIndicesNode?.PropertiesToIntArray();
			mappingMode = layerNode?.GetRelative("MappingInformationType")?.Value.AsString;
			referenceMode = layerNode?.GetRelative("ReferenceInformationType")?.Value.AsString;
		}

		public Vector3[] GetNormals(long geometryId, int[] vertexIndices)
		{
			var normals = new List<Vector3>();
			if (!GetGeometryHasNormals(geometryId))
			{
				return normals.ToArray();
			}
			var vertexIndex = 0;

			GetLayerFloatValues(geometryId, "LayerElementNormal", "Normals", "NormalsIndex", out var layerValues, out var layerIndices, out string mappingMode, out string referenceMode);

			for (var i = 0; i < 3; i++)
			{
				for (var polyIndex = 0; polyIndex < vertexIndices.Length; polyIndex += 3)
				{
					var controlPointIndex = vertexIndices[polyIndex + i];
					normals.Add(ParseVertexAsVector3(layerValues, layerIndices, mappingMode, referenceMode, controlPointIndex, vertexIndex));
				}
				vertexIndex++;
			}
			return normals.ToArray();
		}

		public Vector3[] GetTangents(long geometryId, int[] vertexIndices)
		{
			var tangents = new List<Vector3>();
			if (!GetGeometryHasTangents(geometryId))
			{
				return tangents.ToArray();
			}
			var vertexIndex = 0;

			GetLayerFloatValues(geometryId, "LayerElementTangent", "Tangents", "TangentsIndex", out var layerValues, out var layerIndices, out string mappingMode, out string referenceMode);

			for (var i = 0; i < 3; i++)
			{
				for (var polyIndex = 0; polyIndex < vertexIndices.Length; polyIndex += 3)
				{
					var controlPointIndex = vertexIndices[polyIndex + i];
					tangents.Add(ParseVertexAsVector3(layerValues, layerIndices, mappingMode, referenceMode, controlPointIndex, vertexIndex));
				}
				vertexIndex++;
			}
			return tangents.ToArray();
		}

		public Vector3[] GetBinormals(long geometryId, int[] vertexIndices)
		{
			var binormals = new List<Vector3>();
			if (!GetGeometryHasTangents(geometryId))
			{
				return binormals.ToArray();
			}
			var vertexIndex = 0;

			GetLayerFloatValues(geometryId, "LayerElementBinormal", "Binormals", "BinormalsIndex", out var layerValues, out var layerIndices, out string mappingMode, out string referenceMode);

			for (var i = 0; i < 3; i++)
			{
				for (var polyIndex = 0; polyIndex < vertexIndices.Length; polyIndex += 3)
				{
					var controlPointIndex = vertexIndices[polyIndex + i];
					binormals.Add(ParseVertexAsVector3(layerValues, layerIndices, mappingMode, referenceMode, controlPointIndex, vertexIndex));
				}
				vertexIndex++;
			}
			return binormals.ToArray();
		}

		public Vector2[] GetTexCoords(long geometryId, int[] vertexIndices)
		{
			var texCoords = new List<Vector2>();
			if (!GetGeometryHasTexCoords(geometryId))
			{
				return texCoords.ToArray();
			}
			var vertexIndex = 0;

			GetLayerFloatValues(geometryId, "LayerElementUV", "UV", "UVIndex", out var layerValues, out var layerIndices, out string mappingMode, out string referenceMode);

			for (var i = 0; i < 3; i++)
			{
				for (var polyIndex = 0; polyIndex < vertexIndices.Length; polyIndex += 3)
				{
					var controlPointIndex = vertexIndices[polyIndex + i];
					texCoords.Add(ParseVertexAsVector2(layerValues, layerIndices, mappingMode, referenceMode, controlPointIndex, vertexIndex));
				}
				vertexIndex++;
			}
			return texCoords.ToArray();
		}

		public int[] GetMaterials(long geometryId, int[] vertexIndices)
		{
			var materials = new List<int>();
			if (!GetGeometryHasMaterials(geometryId))
			{
				return materials.ToArray();
			}

			var vertexIndex = 0;

			GetLayerIntValues(geometryId, "LayerElementMaterial", "Materials", "MaterialsIndex", out var layerValues, out var layerIndices, out string mappingMode, out string referenceMode);

			for (var i = 0; i < 3; i++)
			{
				for (var polyIndex = 0; polyIndex < vertexIndices.Length; polyIndex += 3)
				{
					var controlPointIndex = vertexIndices[polyIndex + i];
					materials.Add(ParseVertexAsInt(layerValues, layerIndices, mappingMode, referenceMode, controlPointIndex, vertexIndex));
				}
				vertexIndex++;
			}
			return materials.ToArray();
		}

		//https://github.com/nem0/OpenFBX/blob/master/src/ofbx.cpp
		//https://github.com/assimp/assimp/blob/78ec42fc17f4c04de04ac195f0fce3bea93a7995/code/FBX/FBXExportNode.cpp


		public bool GetGeometryHasMaterials(long geometryId)
		{
			var geometryNode = GetGeometry(geometryId);
			return geometryNode.GetRelative("LayerElementMaterial/Materials") != null;
		}

		public bool GetGeometryHasNormals(long geometryId)
		{
			var geometryNode = GetGeometry(geometryId);
			return geometryNode.GetRelative("LayerElementNormal/Normals") != null;
		}

		public bool GetGeometryHasTangents(long geometryId)
		{
			var geometryNode = GetGeometry(geometryId);
			return geometryNode.GetRelative("LayerElementTangent/Tangents") != null;
		}

		public bool GetGeometryHasBinormals(long geometryId)
		{
			var geometryNode = GetGeometry(geometryId);
			return geometryNode.GetRelative("LayerElementBinormal/Binormals") != null;
		}

		public bool GetGeometryHasTexCoords(long geometryId)
		{
			var geometryNode = GetGeometry(geometryId);
			return geometryNode.GetRelative("LayerElementUV/UV") != null;
		}

		public string GetMaterialName(long materialId)
		{
			var materialNode = GetMaterial(materialId);
			var property = materialNode.GetPropertyWithName("Material");
			return property.AsString.Split(new string[] { "::" }, StringSplitOptions.None)[1];
		}

		public Vector4 GetMaterialDiffuseColor(long materialId)
		{
			var materialNode = GetMaterial(materialId);
			var materialProperties = materialNode.GetRelative(PropertiesName);
			var diffuseProperty = GetNodeWithValue(materialProperties.Nodes, "DiffuseColor");
			if (Version >= FbxVersion.v7_0)
			{
				var alpha = diffuseProperty.Properties.Length > 7 ? diffuseProperty.Properties[7].GetAsFloat() : 1.0f;
				return new Vector4(diffuseProperty.Properties[4].GetAsFloat(), diffuseProperty.Properties[5].GetAsFloat(), diffuseProperty.Properties[6].GetAsFloat(), alpha);
			}
			else
			{
				var alpha = diffuseProperty.Properties.Length > 6 ? diffuseProperty.Properties[6].GetAsFloat() : 1.0f;
				return new Vector4(diffuseProperty.Properties[3].GetAsFloat(), diffuseProperty.Properties[4].GetAsFloat(), diffuseProperty.Properties[5].GetAsFloat(), alpha);
			}
		}

        public long[] GetGeometryIds()
        {
			var geometryNodes = GetFbxNodes(GeometryName, this);
            var result = new List<long>();
            foreach (var geometryNode in geometryNodes)
            {
				result.Add(result.Count);
            }
            return result.ToArray();
        }

		public long[] GetMaterialIds()
		{
			var materialNodes = GetFbxNodes("Material", this);
			var result = new List<long>();
			foreach (var materialNode in materialNodes)
			{
				result.Add(result.Count);
			}
			return result.ToArray();
		}

		public double GetScaleFactor()
        {
            var properties = Version >= FbxVersion.v7_0 ? GetRelative($"GlobalSettings/{PropertiesName}") : GetRelative($"Objects/GlobalSettings/{PropertiesName}");
            var unitScaleFactor = GetNodeWithValue(properties.Nodes, "UnitScaleFactor");
            return unitScaleFactor.Properties[Version >= FbxVersion.v7_0 ? 4 : 3].GetAsDouble();
        }
    }
}
