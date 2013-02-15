using System;
using System.Collections.Generic;
using System.Linq;

using Cairo;

namespace Xamarin.Canvas.Cairo
{
	public static class NodeExtensions
	{
		public static Matrix GetTransform (this Node node)
		{
			double anchorX = node.AnchorX * node.Width;
			double anchorY = node.AnchorY * node.Height;

			Matrix transform = new Matrix ();
			transform.Translate (node.X + anchorX, node.Y + anchorY);
			transform.Rotate (node.Rotation);
			transform.Scale (node.Scale, node.Scale);
			transform.Translate (-anchorX, -anchorY);
			
			return transform;
		}

		public static Matrix GetInverseTransform (this Node node)
		{
			Matrix inverse = node.GetTransform ();
			
			var parent = node.Parent;
			while (parent != null) {
				inverse.Multiply (parent.GetTransform ());
				parent = parent.Parent;
			}
			
			inverse.Invert ();
			return inverse;
		}
	}
	
}
