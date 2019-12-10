using System;
using System.Numerics;

namespace UkooLabs.FbxSharpie
{
	public struct FbxVertex : IEquatable<FbxVertex>
	{
		public const uint SizeInBytes = 56;

		public Vector3 Position;
		public Vector2 TexCoord;
		public Vector3 Normal;
		public Vector3 Tangent;
		public Vector3 Binormal;

		public FbxVertex(Vector3 position, Vector2 texCoord, Vector3 normal, Vector3 tangent, Vector3 binormal)
		{
			Position = position;
			TexCoord = texCoord;
			Normal = normal;
			Tangent = tangent;
			Binormal = binormal;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is FbxVertex))
			{
				return false;
			}
			return Equals((FbxVertex)obj);
		}

		public bool Equals(FbxVertex other)
		{
			return Position == other.Position && TexCoord == other.TexCoord && Normal == other.Normal && Tangent == other.Tangent && Binormal == other.Binormal;
		}

		private static int CombineHashCodes(int h1, int h2)
		{
			return (((h1 << 5) + h1) ^ h2);
		}

		public override int GetHashCode()
		{
			int hash = Position.GetHashCode();
			hash = CombineHashCodes(hash, TexCoord.GetHashCode());
			hash = CombineHashCodes(hash, Normal.GetHashCode());
			hash = CombineHashCodes(hash, Tangent.GetHashCode());
			hash = CombineHashCodes(hash, Binormal.GetHashCode());
			return hash;
		}

		public static bool operator ==(FbxVertex left, FbxVertex right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(FbxVertex left, FbxVertex right)
		{
			return !(left == right);
		}
	}
}
