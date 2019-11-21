using System;
using System.IO;

namespace UkooLabs.FbxSharpie
{
	/// <summary>
	/// Static read and write methods
	/// </summary>
	// IO is an acronym
	// ReSharper disable once InconsistentNaming
	public static class FbxIO
	{

		public static bool IsBinaryFbx(string path)
		{
			using (var stream = new FileStream(path, FileMode.Open))
			{
				return IsBinaryFbx(stream);
			}
		}

		public static bool IsBinaryFbx(Stream stream)
		{
			var position = stream.Position;
			var isBinary = FbxBinaryReader.ReadHeader(stream);
			stream.Position = position;
			return isBinary;
		}

		/// <summary>
		/// Reads a ASCII or binary FBX file
		/// </summary>
		/// <param name="path"></param>
		/// <returns>The top level document node</returns>
		public static FbxDocument Read(string path)
		{
			using (var stream = new FileStream(path, FileMode.Open))
			{
				return Read(stream);
			}
		}

		/// <summary>
		/// Reads a ASCII or binary FBX file
		/// </summary>
		/// <param name="stream"></param>
		/// <returns>The top level document node</returns>
		public static FbxDocument Read(Stream stream)
		{
			if (IsBinaryFbx(stream))
			{
				return new FbxBinaryReader(stream).Read();
			}
			return new FbxAsciiReader(stream).Read();
		}

		/// <summary>
		/// Reads a binary FBX file
		/// </summary>
		/// <param name="path"></param>
		/// <returns>The top level document node</returns>
		public static FbxDocument ReadBinary(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException(nameof(path));
			}
			using (var stream = new FileStream(path, FileMode.Open))
			{
				return ReadBinary(stream);
			}
		}

		/// <summary>
		/// Reads a binary FBX file
		/// </summary>
		/// <param name="stream"></param>
		/// <returns>The top level document node</returns>
		public static FbxDocument ReadBinary(Stream stream)
		{
			if (stream == null) 
			{
				throw new ArgumentNullException(nameof(stream));
			}
			var reader = new FbxBinaryReader(stream);
			return reader.Read();
		}

		/// <summary>
		/// Reads an ASCII FBX file
		/// </summary>
		/// <param name="path"></param>
		/// <returns>The top level document node</returns>
		public static FbxDocument ReadAscii(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException(nameof(path));
			}
			using (var stream = new FileStream(path, FileMode.Open))
			{
				return ReadAscii(stream);
			}
		}

		/// <summary>
		/// Reads an ASCII FBX file
		/// </summary>
		/// <param name="stream"></param>
		/// <returns>The top level document node</returns>
		public static FbxDocument ReadAscii(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}
			var reader = new FbxAsciiReader(stream);
			return reader.Read();
		}

		/// <summary>
		/// Writes an FBX document
		/// </summary>
		/// <param name="document">The top level document node</param>
		/// <param name="path"></param>
		public static void WriteBinary(FbxDocument document, string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException(nameof(path));
			}
			using (var stream = new FileStream(path, FileMode.Create))
			{
				WriteBinary(document, stream);
			}
		}

		/// <summary>
		/// Writes an FBX document
		/// </summary>
		/// <param name="document">The top level document node</param>
		/// <param name="stream"></param>
		public static void WriteBinary(FbxDocument document, Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}
			var writer = new FbxBinaryWriter(stream);
			writer.Write(document);
		}

		/// <summary>
		/// Writes an FBX document
		/// </summary>
		/// <param name="document">The top level document node</param>
		/// <param name="path"></param>
		public static void WriteAscii(FbxDocument document, string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException(nameof(path));
			}
			using (var stream = new FileStream(path, FileMode.Create))
			{
				WriteAscii(document, stream);
			}
		}

		/// <summary>
		/// Writes an FBX document
		/// </summary>
		/// <param name="document">The top level document node</param>
		/// <param name="stream"></param>
		public static void WriteAscii(FbxDocument document, Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException(nameof(stream));
			}
			var writer = new FbxAsciiWriter(stream);
			writer.Write(document);
		}
	}
}
