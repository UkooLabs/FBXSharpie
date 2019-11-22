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
            return connections.Any(c => c != null && c.Properties[1].AsLong == value1 && c.Properties[2].AsLong == value2);
        }

        private int NormalizeIndex(int index)
        {
            return index < 0 ? (index + 1) * -1 : index;
        }

        private FbxNode GetMaterialNodeForGeometry(long geometryId)
        {
            var models = GetFbxNodes("Model", this);
            var materials = GetFbxNodes("Material", this);
            foreach (var model in models)
            {
                var modelId = model.Value.AsLong;
                if (HasConnection(geometryId, modelId))
                {
                    foreach (var material in materials)
                    {
                        var materialId = material.Value.AsLong;
                        if (HasConnection(materialId, modelId))
                        {
                            return material;
                        }
                    }
                }
            }
            return null;
        }

        private FbxNode GetGeometry(long geometryId)
        {
            var geometryNodes = GetFbxNodes("Geometry", this);
            foreach (var geometryNode in geometryNodes)
            {
                if (geometryNode.Value.AsLong != geometryId)
                {
                    continue;
                }
                return geometryNode;
            }
            return null;
        }

        public int[] GetVertexIndices(long geometryId)
        {
            var geometryNode = GetGeometry(geometryId);
            var vertexIndices = geometryNode.GetRelative("PolygonVertexIndex").Value.AsIntArray;
            var quadMode = vertexIndices[2] >= 0;
            var result = new List<int>();
            if (quadMode)
            {
                for (var i = 0; i < vertexIndices.Length; i += 4)
                {
                    result.Add(NormalizeIndex(vertexIndices[i]));
                    result.Add(NormalizeIndex(vertexIndices[i + 1]));
                    result.Add(NormalizeIndex(vertexIndices[i + 3]));
                    result.Add(NormalizeIndex(vertexIndices[i + 3]));
                    result.Add(NormalizeIndex(vertexIndices[i + 1]));
                    result.Add(NormalizeIndex(vertexIndices[i + 2]));
                }
            }
            else
            {
                for (var i = 0; i < vertexIndices.Length; i++)
                {
                    result.Add(NormalizeIndex(vertexIndices[i]));
                }
            }
            return result.ToArray();
        }

        public Vector3[] GetVertices(long geometryId)
        {
            var geometryNode = GetGeometry(geometryId);
            var vertices = geometryNode.GetRelative("Vertices").Value.AsDoubleArray;
            var result = new List<Vector3>();
            for (var i = 0; i < vertices.Length; i += 3)
            {
                result.Add(new Vector3((float)vertices[i], (float)vertices[i + 1], (float)vertices[i + 2]));
            }
            return result.ToArray();
        }

        public Vector3[] GetNormals(long geometryId)
        {
            var geometryNode = GetGeometry(geometryId);
			var result = new List<Vector3>();
			var normalNode = geometryNode.GetRelative("LayerElementNormal/Normals");
			if (normalNode != null)
			{
				var normals = normalNode.Value.AsDoubleArray;
				for (var i = 0; i < normals.Length; i += 3)
				{
					result.Add(new Vector3((float)normals[i], (float)normals[i + 1], (float)normals[i + 2]));
				}
			}
            return result.ToArray();
        }

		public bool GetGeometryHasTangents(long geometryId)
		{
			var geometryNode = GetGeometry(geometryId);
			return geometryNode.GetRelative("LayerElementTangent/Tangents") != null;
		}

		public Vector3[] GetTangents(long geometryId)
        {
            var geometryNode = GetGeometry(geometryId);
			var result = new List<Vector3>();
			var tangentNode = geometryNode.GetRelative("LayerElementTangent/Tangents");
			if (tangentNode != null)
			{
				var tangents = tangentNode.Value.AsDoubleArray;
				for (var i = 0; i < tangents.Length; i += 3)
				{
					result.Add(new Vector3((float)tangents[i], (float)tangents[i + 1], (float)tangents[i + 2]));
				}
			}
            return result.ToArray();
        }

		public bool GetGeometryHasBinormals(long geometryId)
		{
			var geometryNode = GetGeometry(geometryId);
			return geometryNode.GetRelative("LayerElementBinormal/Binormals") != null;
		}

		public Vector3[] GetBinormals(long geometryId)
		{
			var geometryNode = GetGeometry(geometryId);
			var result = new List<Vector3>();
			var binormalNode = geometryNode.GetRelative("LayerElementBinormal/Binormals");
			if (binormalNode != null)
			{
				var binormals = binormalNode.Value.AsDoubleArray;
				for (var i = 0; i < binormals.Length; i += 3)
				{
					result.Add(new Vector3((float)binormals[i], (float)binormals[i + 1], (float)binormals[i + 2]));
				}
			}
			return result.ToArray();
		}

		public Vector2[] GetTexCoords(long geometryId)
        {
            var geometryNode = GetGeometry(geometryId);
			var result = new List<Vector2>();
			var texCoordNode = geometryNode.GetRelative("LayerElementUV/UV");
			if (texCoordNode != null)
			{
				var texCoords = texCoordNode.Value.AsDoubleArray;
				for (var i = 0; i < texCoords.Length; i += 2)
				{
					result.Add(new Vector2((float)texCoords[i], (float)texCoords[i + 1]));
				}
			}
            return result.ToArray();
        }

		public bool GetGeometryHasMaterial(long geometryId)
		{
			return GetMaterialNodeForGeometry(geometryId) != null;
		}

        public string GetMaterialName(long geometryId)
        {
            var materialNode = GetMaterialNodeForGeometry(geometryId);
			if (materialNode == null)
			{
				return null;
			}
			return  materialNode.Properties[1].AsString.Split(new string[] { "::" }, StringSplitOptions.None)[1];
        }

        public Vector4 GetMaterialDiffuseColor(long geometryId)
        {
            var materialNode = GetMaterialNodeForGeometry(geometryId);
			if (materialNode == null)
			{
				return new Vector4(1.0f);
			}
            var materialProperties = materialNode.GetRelative(PropertiesName);
            var diffuseProperty = GetNodeWithValue(materialProperties.Nodes, "DiffuseColor");
			var alpha = diffuseProperty.Properties.Length > 7 ? diffuseProperty.Properties[7].AsFloat : 1.0f;
			return new Vector4(diffuseProperty.Properties[4].AsFloat, diffuseProperty.Properties[5].AsFloat, diffuseProperty.Properties[6].AsFloat, alpha);
        }

        public long[] GetGeometryIds()
        {
            var geometryNodes = GetFbxNodes("Geometry", this);
            var result = new List<long>();
            foreach (var geometryNode in geometryNodes)
            {
                result.Add(geometryNode.Value.AsLong);
            }
            return result.ToArray();
        }

        public double GetScaleFactor()
        {
            var properties = Version >= FbxVersion.v7_0 ? GetRelative($"GlobalSettings/{PropertiesName}") : GetRelative($"Objects/GlobalSettings/{PropertiesName}");
            var unitScaleFactor = GetNodeWithValue(properties.Nodes, "UnitScaleFactor");
            return unitScaleFactor.Properties[Version >= FbxVersion.v7_0 ? 4 : 3].AsDouble;
        }
    }
}
