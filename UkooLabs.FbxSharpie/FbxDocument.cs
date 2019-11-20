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
            var normals = geometryNode.GetRelative("LayerElementNormal/Normals").Value.AsDoubleArray;
            var result = new List<Vector3>();
            for (var i = 0; i < normals.Length; i += 3)
            {
                result.Add(new Vector3((float)normals[i], (float)normals[i + 1], (float)normals[i + 2]));
            }
            return result.ToArray();
        }

        public Vector3[] GetTangents(long geometryId)
        {
            var geometryNode = GetGeometry(geometryId);
            var tangents = geometryNode.GetRelative("LayerElementTangent/Tangents").Value.AsDoubleArray;
            var result = new List<Vector3>();
            for (var i = 0; i < tangents.Length; i += 3)
            {
                result.Add(new Vector3((float)tangents[i], (float)tangents[i + 1], (float)tangents[i + 2]));
            }
            return result.ToArray();
        }

        public Vector2[] GetTexCoords(long geometryId)
        {
            var geometryNode = GetGeometry(geometryId);
            var texCoords = geometryNode.GetRelative("LayerElementUV/UV").Value.AsDoubleArray;
            var result = new List<Vector2>();
            for (var i = 0; i < texCoords.Length; i += 2)
            {
                result.Add(new Vector2((float)texCoords[i], (float)texCoords[i + 1]));
            }
            return result.ToArray();
        }

        public string GetMaterialName(long geometryId)
        {
            var materialNode = GetMaterialNodeForGeometry(geometryId);
            return materialNode.Properties[1].AsString.Split(new string[] { "::" }, StringSplitOptions.None)[1];
        }

        public Vector3 GetDiffuseColor(long geometryId)
        {
            var materialNode = GetMaterialNodeForGeometry(geometryId);
            var materialProperties = materialNode.GetRelative("Properties70");
            var diffuseProperty = GetNodeWithValue(materialProperties.Nodes, "DiffuseColor");
            return new Vector3(diffuseProperty.Properties[4].AsFloat, diffuseProperty.Properties[5].AsFloat, diffuseProperty.Properties[6].AsFloat);
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
            var properties = GetRelative("GlobalSettings/Properties70");
            var unitScaleFactor = GetNodeWithValue(properties.Nodes, "UnitScaleFactor");
            return unitScaleFactor.Properties[4].AsDouble;
        }
    }
}
