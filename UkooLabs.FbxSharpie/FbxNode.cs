using System;
using System.Collections.Generic;
using UkooLabs.FbxSharpie.Tokens;
using UkooLabs.FbxSharpie.Tokens.Value;

namespace UkooLabs.FbxSharpie
{
	/// <summary>
	/// Represents a node in an FBX file
	/// </summary>
	public class FbxNode : FbxNodeList
	{
        private readonly List<Token> _properties = new List<Token>();

		/// <summary>
		/// The node name, which is often a class type
		/// </summary>
		/// <remarks>
		/// The name must be smaller than 256 characters to be written to a binary stream
		/// </remarks>
		public IdentifierToken Identifier { get; }

        /// <summary>
        /// The list of properties associated with the node
        /// </summary>
        /// <remarks>
        /// Supported types are primitives (apart from byte and char),arrays of primitives, and strings
        /// </remarks>
        public Token[] Properties => _properties.ToArray();

		public FbxNode(IdentifierToken identifier)
		{
			Identifier = identifier;
		}

		public Token GetPropertyWithName(string name)
		{
			foreach (var property in _properties)
			{
				if (property.TokenType != TokenType.String)
				{
					continue;
				}
				var stringToken = (StringToken)property;
				var propertyName = stringToken.Value?.Split(new string[] { "::" }, StringSplitOptions.None)[0];
				if (string.Equals(propertyName, name, StringComparison.CurrentCultureIgnoreCase))
				{
					return property;
				}
			}
			return null;
		}

        public void AddProperty(Token value)
        {
            _properties.Add(value);
        }

		public int[] PropertiesToIntArray()
		{
			var values = new List<int>();
			foreach (var property in Properties)
			{
				if (property.TokenType != TokenType.Value && property.ValueType != Tokens.ValueType.None)
				{
					continue;
				}
				if (property.ValueType == Tokens.ValueType.Boolean && property is BooleanToken booleanToken)
				{
					values.Add(booleanToken.Value ? 1 : 0);
				}
				else if (property.ValueType == Tokens.ValueType.Integer && property is IntegerToken integerToken)
				{
					values.Add(integerToken.Value);
				}
				else if (property.ValueType == Tokens.ValueType.Long && property is LongToken longToken)
				{
					values.Add((int)longToken.Value);
				}
				else if (property.ValueType == Tokens.ValueType.Float && property is FloatToken floatToken)
				{
					values.Add((int)floatToken.Value);
				}
				else if (property.ValueType == Tokens.ValueType.Double && property is DoubleToken doubleToken)
				{
					values.Add((int)doubleToken.Value);
				}
			}
			return values.ToArray();
		}

		public float[] PropertiesToFloatArray()
		{
			var values = new List<float>();
			foreach (var property in Properties)
			{
				if (property.TokenType != TokenType.Value && property.ValueType != Tokens.ValueType.None)
				{
					continue;
				}
				if (property.ValueType == Tokens.ValueType.Boolean && property is BooleanToken booleanToken)
				{
					values.Add(booleanToken.Value ? 1 : 0);
				}
				else if (property.ValueType == Tokens.ValueType.Integer && property is IntegerToken integerToken)
				{
					values.Add(integerToken.Value);
				}
				else if (property.ValueType == Tokens.ValueType.Long && property is LongToken longToken)
				{
					values.Add(longToken.Value);
				}
				else if (property.ValueType == Tokens.ValueType.Float && property is FloatToken floatToken)
				{
					values.Add(floatToken.Value);
				}
				else if (property.ValueType == Tokens.ValueType.Double && property is DoubleToken doubleToken)
				{
					values.Add((float)doubleToken.Value);
				}
			}
			return values.ToArray();
		}

		/// <summary>
		/// The first property element
		/// </summary>
		public Token Value
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
		public bool IsEmpty => string.IsNullOrEmpty(Identifier.Value) && Properties.Length == 0 && Nodes.Length == 0;
	}
}
