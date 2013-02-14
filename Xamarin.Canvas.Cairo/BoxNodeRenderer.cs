using System;
using System.Collections.Generic;
using System.Linq;

using Cairo;

namespace Xamarin.Canvas.Cairo
{
	public class BoxNodeRenderer : AbstractCairoRenderer
	{
		public override void LayoutOutline (Node node, Context context)
		{
			context.Rectangle (0, 0, node.Width, node.Height);
		}

		public override void Render (Node node, Context context)
		{
			BoxNode box = node as BoxNode;

			LayoutOutline (node, context);
			context.Color = box.Color.MultiplyAlpha (node.Opacity).ToCairo ();
			context.Fill ();
		}

		public override int Affinity (Node node)
		{
			return StandardAffinity (node, typeof (BoxNode));
		}

	}
}
