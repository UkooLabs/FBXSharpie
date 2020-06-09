using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Numerics;

namespace UkooLabs.FbxSharpie.Tests.Helpers
{
	public static class VertexDraw
	{
		public static void Draw(int width, int height, Vector3[] vertices, string filePath)
		{
			var minX = float.MaxValue;
			var maxX = float.MinValue;
			var minY = float.MaxValue;
			var maxY = float.MinValue;

			foreach (var vertice in vertices)
			{
				minX = Math.Min(vertice.X, minX);
				maxX = Math.Max(vertice.X, maxX);
				minY = Math.Min(vertice.Y, minY);
				maxY = Math.Max(vertice.Y, maxY);
			}

			using (var image = new Image<Rgba32>(width, height))
			{
				for (int i = 0; i < vertices.Length; i += 3)
				{
					var point1 = new PointF(vertices[i].X, vertices[i].Y);
					point1.X -= minX;
					point1.Y -= minY;
					point1.X /= (maxX - minX);
					point1.Y /= (maxY - minY);
					point1.X *= width;
					point1.Y *= height;

					var point2 = new PointF(vertices[i + 1].X, vertices[i + 1].Y);
					point2.X -= minX;
					point2.Y -= minY;
					point2.X /= (maxX - minX);
					point2.Y /= (maxY - minY);
					point2.X *= width;
					point2.Y *= height;

					var point3 = new PointF(vertices[i + 2].X, vertices[i + 2].Y);
					point3.X -= minX;
					point3.Y -= minY;
					point3.X /= (maxX - minX);
					point3.Y /= (maxY - minY);
					point3.X *= width;
					point3.Y *= height;

					image.Mutate(i => i.DrawLines(Color.HotPink, 2, new[] { point1, point2, point3, point1 }));
				}
				image.Save(filePath);
			}
		}

	}
}
