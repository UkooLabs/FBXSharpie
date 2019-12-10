using System;
using System.Collections.Generic;

namespace UkooLabs.FbxSharpie
{
	/// <summary>
	/// Represents a node in an FBX file
	/// </summary>
	public class FbxNode : FbxNodeList
	{
        private readonly List<FbxValue> _properties = new List<FbxValue>();

		/// <summary>
		/// The node name, which is often a class type
		/// </summary>
		/// <remarks>
		/// The name must be smaller than 256 characters to be written to a binary stream
		/// </remarks>
		public string Name { get; set; }

        /// <summary>
        /// The list of properties associated with the node
        /// </summary>
        /// <remarks>
        /// Supported types are primitives (apart from byte and char),arrays of primitives, and strings
        /// </remarks>
        public FbxValue[] Properties => _properties.ToArray();

		public FbxValue GetPropertyWithName(string name)
		{
			foreach (var property in _properties)
			{
				if (!property.IsString)
				{
					continue;
				}
				var propertyName = property.AsString?.Split(new string[] { "::" }, StringSplitOptions.None)[0];
				if (string.Equals(propertyName, name, StringComparison.CurrentCultureIgnoreCase))
				{
					return property;
				}
			}
			return null;
		}

        public void AddProperty(FbxValue value)
        {
            _properties.Add(value);
        }

		public int[] PropertiesToIntArray()
		{
			var values = new List<int>();
			foreach (var property in Properties)
			{
				values.Add(property.GetAsInt());
			}
			return values.ToArray();
		}

		public float[] PropertiesToFloatArray()
		{
			var values = new List<float>();
			foreach (var property in Properties)
			{
				values.Add(property.GetAsFloat());
			}
			return values.ToArray();
		}

		/// <summary>
		/// The first property element
		/// </summary>
		public FbxValue Value
		{
			get { return Properties.Length < 1 ? null : Properties[0]; }
			set
			{
                if (Properties.Length < 1)
                {
                    _properties.Add(value);
                }
                else
                {
                    Properties[0] = value;
                }
			}
		}

		/// <summary>
		/// Whether the node is empty of data
		/// </summary>
		public bool IsEmpty => string.IsNullOrEmpty(Name) && Properties.Length == 0 && Nodes.Length == 0;
	}
}
