using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace UkooLabs.FbxSharpie
{
	public class FbxIndexer
	{
		private readonly List<long> _materialIds;
		private readonly List<FbxVertex> _vertices; 

		public FbxIndexer()
		{
			_materialIds = new List<long>();
			_vertices = new List<FbxVertex>();
		}

		public void AddVertex(FbxVertex vertex)
		{
			_materialIds.Add(0);
			_vertices.Add(vertex);
		}

		public void AddVertex(FbxVertex vertex, long materialId)
		{
			_materialIds.Add(materialId);
			_vertices.Add(vertex);
		}

		public void Index(out FbxVertex[] vertices, out int[] indices)
		{
			Index(out vertices,out indices);
		}

		public void Index(long materialId, out FbxVertex[] vertices, out int[] indices)
		{
			var tempVertices = new List<FbxVertex>();
			var tempIndices = new List<int>();
			for (var i = 0; i < _vertices.Count; i++)
			{
				if (materialId != _materialIds[i])
				{
					continue;
				}
				var vertex = _vertices[i];
				if (tempVertices.Contains(vertex))
				{
					var index = tempVertices.IndexOf(vertex);
					tempIndices.Add(index);
				}
				else
				{
					tempIndices.Add(tempVertices.Count);
					tempVertices.Add(vertex);
				}
			}
			vertices = tempVertices.ToArray();
			indices = tempIndices.ToArray();
		}
	}
}
