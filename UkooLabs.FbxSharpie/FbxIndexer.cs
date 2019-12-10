using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace UkooLabs.FbxSharpie
{
	public class FbxIndexer
	{
		private readonly List<FbxVertex> _vertices; 

		public FbxIndexer()
		{
			_vertices = new List<FbxVertex>();
		}

		public void AddVertex(FbxVertex vertex)
		{
			_vertices.Add(vertex);
		}

		public void Index(out FbxVertex[] vertices, out int[] indices)
		{
			var tempVertices = new List<FbxVertex>();
			var tempIndices = new List<int>();
			foreach (var vertex in _vertices)
			{
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
