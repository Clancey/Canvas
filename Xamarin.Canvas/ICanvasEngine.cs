using System;
using System.Collections.Generic;

namespace Xamarin.Canvas
{
	public struct TextOptions
	{
		public double MaxWidth;
		public Color Color;
	}

	public interface ICanvasEngine
	{
		bool Supports3D { get; }

		void RenderScene (Node rootNode);

		Node InputNodeAt (Node rootNode, double x, double y);

		void SetBackground (Color color);

		Size TextExtents (string text, TextOptions options);

		Size ImageSize (string file);
	}
}

