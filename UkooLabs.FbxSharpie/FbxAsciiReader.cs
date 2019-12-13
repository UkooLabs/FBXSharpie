using System;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using UkooLabs.FbxSharpie.Extensions;
using UkooLabs.FbxSharpie.Tokens;

namespace UkooLabs.FbxSharpie
{
	/// <summary>
	/// Reads FBX nodes from a text stream
	/// </summary>
	public class FbxAsciiReader
	{
		private readonly Stream _stream;
		private readonly ErrorLevel _errorLevel;
		private readonly FbxAsciiFileInfo _fbxAsciiFileInfo;

		/// <summary>
		/// Creates a new reader
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="errorLevel"></param>
		public FbxAsciiReader(Stream stream, ErrorLevel errorLevel = ErrorLevel.Checked)
		{
			_fbxAsciiFileInfo = new FbxAsciiFileInfo();
			_stream = stream ?? throw new ArgumentNullException(nameof(stream));
			_errorLevel = errorLevel;
		}

		/// <summary>
		/// The maximum array size that will be allocated
		/// </summary>
		/// <remarks>
		/// If you trust the source, you can expand this value as necessary.
		/// Malformed files could cause large amounts of memory to be allocated
		/// and slow or crash the system as a result.
		/// </remarks>
		public int MaxArrayLength { get; set; } = (1 << 24);

		private object prevTokenSingle;

		// Reads a single token, allows peeking
		// Can return 'null' for a comment or whitespace
		object ReadTokenSingle()
		{
			if (prevTokenSingle != null)
			{
				var ret = prevTokenSingle;
				prevTokenSingle = null;
				return ret;
			}

			if(_stream.IsEndOfStream())
			{
				return new EndOfStream();
			}
			if (_stream.TryParseWhiteSpaceToken(_fbxAsciiFileInfo, out var _))
			{
				return null;
			}
			if (_stream.TryParseCommentToken(_fbxAsciiFileInfo, out var _))
			{
				return null;
			}
			if (_stream.TryParseOperatorToken(_fbxAsciiFileInfo, out var op))
			{
				return op;
			}
			if (_stream.TryParseNumberToken(_fbxAsciiFileInfo, out var value))
			{
				return value;
			}
			if (_stream.TryParseLiteralToken(_fbxAsciiFileInfo, out var literal))
			{
				return literal;
			}
			if (_stream.TryParseIdentifierToken(_fbxAsciiFileInfo, out var identifier))
			{
				return new Identifier(identifier);
			}

			throw new FbxException(_fbxAsciiFileInfo, $"Unknown character {_stream.PeekChar(_fbxAsciiFileInfo)}");
		}

		private object prevToken;

		// Use a loop rather than recursion to prevent stack overflow
		// Here we can also merge string+colon into an identifier,
		// returning single-character bare strings (for C-type properties)
		object ReadToken()
		{
			object ret;
			if (prevToken != null)
			{
				ret = prevToken;
				prevToken = null;
				return ret;
			}
			do
			{
				ret = ReadTokenSingle();
			} while (ret == null);
			if (ret is Identifier id)
			{
				object colon;
				do
				{
					colon = ReadTokenSingle();
				} while (colon == null);
				if (!':'.Equals(colon))
				{
					if (id.Value.Length > 1)
					{
						throw new FbxException(_fbxAsciiFileInfo, "Unexpected '" + colon + "', expected ':' or a single-char literal");
					}
					ret = id.Value[0];
					prevTokenSingle = colon;
				}
			}
			return ret;
		}

		void ExpectToken(object token)
		{
			var t = ReadToken();
			if (!token.Equals(t))
			{
				throw new FbxException(_fbxAsciiFileInfo, "Unexpected '" + t + "', expected " + token);
			}
		}

		private enum ArrayType
		{
			Byte = 0,
			Int = 1,
			Long = 2,
			Float = 3,
			Double = 4,
		};

		Array ReadArray()
		{
			// Read array length and header
			var len = ReadToken();
			long l;
			if (len is long)
			{
				l = (long) len;
			}
			else if (len is int)
			{
				l = (int) len;
			}
			else if (len is byte)
			{
				l = (byte) len;
			}
			else
			{
				throw new FbxException(_fbxAsciiFileInfo, "Unexpected '" + len + "', expected an integer");
			}

			if (l < 0)
			{
				throw new FbxException(_fbxAsciiFileInfo, "Invalid array length " + l);
			}

			if (l > MaxArrayLength)
			{
				throw new FbxException(_fbxAsciiFileInfo, "Array length " + l + " higher than permitted maximum " + MaxArrayLength);
			}

			ExpectToken('{');
			ExpectToken(new Identifier("a"));
			var array = new double[l];

			// Read array elements
			bool expectComma = false;
			object token = ReadToken();
			var arrayType = ArrayType.Byte;
			long pos = 0;
			while (!'}'.Equals(token))
			{
				if (expectComma)
				{
					if (!','.Equals(token))
					{
						throw new FbxException(_fbxAsciiFileInfo, "Unexpected '" + token + "', expected ','");
					}
					expectComma = false;
					token = ReadToken();
					continue;
				}
				if (pos >= array.Length)
				{
					if (_errorLevel >= ErrorLevel.Checked)
					{
						throw new FbxException(_fbxAsciiFileInfo,
							"Too many elements in array");
					}
					token = ReadToken();
					continue;
				}

				// Add element to the array, checking for the maximum
				// size of any one element.
				// (I'm not sure if this is the 'correct' way to do it, but it's the only
				// logical one given the nature of the ASCII format)
				double d;
				if (token is byte)
				{
					d = (byte)token;
				}
				else if (token is int)
				{
					d = (int)token;
					if (arrayType < ArrayType.Int)
					{
						arrayType = ArrayType.Int;
					}
				}
				else if (token is long)
				{
					d = (long)token;
					if (arrayType < ArrayType.Long)
					{
						arrayType = ArrayType.Long;
					}
				}
				else if (token is float)
				{
					d = (float)token;
					// A long can't be accurately represented by a float
					arrayType = arrayType < ArrayType.Long
						? ArrayType.Float : ArrayType.Double;
				}
				else if (token is double)
				{
					d = (double)token;
					if (arrayType < ArrayType.Double)
					{
						arrayType = ArrayType.Double;
					}
				}
				else
				{
					throw new FbxException(_fbxAsciiFileInfo, "Unexpected '" + token + "', expected a number");
				}

				array[pos++] = d;
				expectComma = true;
				token = ReadToken();
			}
			if(pos < array.Length && _errorLevel >= ErrorLevel.Checked)
			{
				throw new FbxException(_fbxAsciiFileInfo,
					"Too few elements in array - expected " + (array.Length - pos) + " more");
			}

			// Convert the array to the smallest type we can see
			Array ret;
			switch (arrayType)
			{
				case ArrayType.Byte:
					var bArray = new byte[array.Length];
					for (int i = 0; i < bArray.Length; i++)
					{
						bArray[i] = (byte)array[i];
					}

					ret = bArray;
					break;
				case ArrayType.Int:
					var iArray = new int[array.Length];
					for (int i = 0; i < iArray.Length; i++)
					{
						iArray[i] = (int)array[i];
					}

					ret = iArray;
					break;
				case ArrayType.Long:
					var lArray = new long[array.Length];
					for (int i = 0; i < lArray.Length; i++)
					{
						lArray[i] = (long)array[i];
					}

					ret = lArray;
					break;
				case ArrayType.Float:
					var fArray = new float[array.Length];
					for (int i = 0; i < fArray.Length; i++)
					{
						fArray[i] = (long)array[i];
					}

					ret = fArray;
					break;
				default:
					ret = array;
					break;
			}
			return ret;
		}

		/// <summary>
		/// Reads the next node from the stream
		/// </summary>
		/// <returns>The read node, or <c>null</c></returns>
		public FbxNode ReadNode()
		{
			var first = ReadToken();
			if (!(first is Identifier id))
			{
				if (first is EndOfStream)
				{
					return null;
				}

				throw new FbxException(_fbxAsciiFileInfo,
					"Unexpected '" + first + "', expected an identifier");
			}
			var node = new FbxNode {Name = id.Value};

			// Read properties
			object token = ReadToken();
			bool expectComma = false;
			while (!'{'.Equals(token) && !(token is Identifier) && !'}'.Equals(token))
			{
				if (expectComma)
				{
					if (!','.Equals(token))
					{
						throw new FbxException(_fbxAsciiFileInfo, "Unexpected '" + token + "', expected a ','");
					}
					expectComma = false;
					token = ReadToken();
					continue;
				}
				if (token is char c)
				{
					switch (c)
					{
						case '*':
							token = ReadArray();
							break;
						case '}':
						case ':':
						case ',':
							throw new FbxException(_fbxAsciiFileInfo, "Unexpected '" + c + "' in property list");
					}
				}
				node.AddProperty(new FbxValue(token));
				expectComma = true; // The final comma before the open brace isn't required
				token = ReadToken();
			}
			// TODO: Merge property list into an array as necessary
			// Now we're either at an open brace, close brace or a new node
			if (token is Identifier || '}'.Equals(token))
			{
				prevToken = token;
				return node;
			}
			// The while loop can't end unless we're at an open brace, so we can continue right on
			object endBrace = ReadToken();
			while(!'}'.Equals(endBrace))
			{
				prevToken = endBrace; // If it's not an end brace, the next node will need it
				node.AddNode(ReadNode());
				endBrace = ReadToken();
			}
			if(node.Nodes.Length < 1) // If there's an open brace, we want that to be preserved
			{
				node.AddNode(null);
			}

			return node;
		}

		/// <summary>
		/// Reads a full document from the stream
		/// </summary>
		/// <returns>The complete document object</returns>
		public FbxDocument Read()
		{
			var ret = new FbxDocument();

			// Read version string
			const string versionString = @"; FBX (\d)\.(\d)\.(\d) project file";

			_stream.TryParseWhiteSpaceToken(_fbxAsciiFileInfo, out var _);

			bool hasVersionString = false;
			if (_stream.TryParseCommentToken(_fbxAsciiFileInfo, out var comment))
			{
				var match = Regex.Match(comment, versionString);
				hasVersionString = match.Success;
				if(hasVersionString)
				{
					ret.Version = (FbxVersion)(
						int.Parse(match.Groups[1].Value)*1000 +
						int.Parse(match.Groups[2].Value)*100 +
						int.Parse(match.Groups[3].Value)*10
					);
				}
			}

			if(!hasVersionString && _errorLevel >= ErrorLevel.Strict)
			{
				throw new FbxException(_fbxAsciiFileInfo, "Invalid version string; first line must match \"" + versionString + "\"");
			}

			FbxNode node;
			while((node = ReadNode()) != null)
			{
				ret.AddNode(node);
			}

			return ret;
		}
	}
}
