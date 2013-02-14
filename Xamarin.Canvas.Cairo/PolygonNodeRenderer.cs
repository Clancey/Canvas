using System;
using System.Collections.Generic;
using System.Linq;

using Cairo;

namespace Xamarin.Canvas.Cairo
{
	public class PolygonNodeRenderer : AbstractCairoRenderer
	{
		public override void LayoutOutline (Node node, Context context)
		{
			PolygonNode poly = node as PolygonNode;
			// polygons have 3 sides
			if (poly.Verticies.Count <= 2)
				return;
			
			var renderVerts = poly.Verticies.Select (v => new Point (v.X * node.Width, v.Y * node.Height)).ToList ();
			context.MoveTo (renderVerts.First ().ToCairo ());
			renderVerts.ForEach (v => context.LineTo (v.ToCairo ()));
		}

		public override void Render (Node node, Context context)
		{
			PolygonNode poly = node as PolygonNode;

			LayoutOutline (poly, context);
			context.Color = poly.Color.MultiplyAlpha (poly.Opacity).ToCairo ();
			context.Fill ();
		}

		public override int Affinity (Node node)
		{
			return StandardAffinity (node, typeof (PolygonNode));
		}
	}
	
}
