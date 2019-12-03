using System.Linq;
using System.Collections.Generic;

namespace UkooLabs.FbxSharpie
{
	/// <summary>
	/// Base class for nodes and documents
	/// </summary>
	public abstract class FbxNodeList
	{
        private readonly List<FbxNode> _nodes = new List<FbxNode>();

        /// <summary>
        /// The list of child/nested nodes
        /// </summary>
        /// <remarks>
        /// A list with one or more null elements is treated differently than an empty list,
        /// and represented differently in all FBX output files.
        /// </remarks>
        public FbxNode[] Nodes => _nodes.ToArray();

        /// <summary>
        /// Add node to node array
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(FbxNode node)
        {
            _nodes.Add(node);
        }

        /// <summary>
        /// Gets a named child node
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The child node, or null</returns>
        public FbxNode this[string name] { get { return Nodes.FirstOrDefault(n => n != null && n.Name == name); } }

		/// <summary>
		/// Gets a child node, using a '/' separated path
		/// </summary>
		/// <param name="path"></param>
		/// <returns>The child node, or null</returns>
		public FbxNode GetRelative(string path)
		{
			var tokens = path.Split('/');
			FbxNodeList n = this;
			foreach (var t in tokens)
			{
				if (t == "")
				{
					continue;
				}

				n = n[t];
				if (n == null)
				{
					break;
				}
			}
			return n as FbxNode;
		}
	}
}
