using System;
using System.Collections.Generic;
using System.Linq;

using Cairo;

namespace Xamarin.Canvas.Cairo
{
	public class LabelNodeRenderer : AbstractCairoRenderer
	{
		Func<string, TextOptions, Engine, Size> GetExtents = CachedFunc.Make ((string s, TextOptions o, Engine e) => e.TextExtents (s, o));

		public override void LayoutOutline (Node node, Context context)
		{
			LabelNode label = node as LabelNode;
			Cairo.Engine engine = node.Canvas.Engine as Cairo.Engine;

			if (label.ClipInputToTextExtents) {
				var options = new TextOptions ();
				options.Color = label.Color.MultiplyAlpha (node.Opacity);
				options.MaxWidth = label.Width;

				var size = GetExtents (label.Text, options, engine);
				
				double x = label.Width - size.Width;
				x = (int) (x * label.XAlign);
				
				double y = label.Height - size.Height;
				y = (int) (y * label.YAlign);
				
				context.Rectangle (x, y, size.Width, size.Height);
			} else {
				context.Rectangle (0, 0, label.Width, label.Height);
			}
		}

		public override void Render (Node node, Context context)
		{
			LabelNode label = node as LabelNode;
			Cairo.Engine engine = node.Canvas.Engine as Cairo.Engine;

			var options = new TextOptions ();
			options.Color = label.Color.MultiplyAlpha (node.Opacity);
			options.MaxWidth = label.Width;

			var size = GetExtents (label.Text, options, engine);

			double x = label.Width - size.Width;
			x = (int) (x * label.XAlign);
			
			double y = label.Height - size.Height;
			y = (int) (y * label.YAlign);

			engine.RenderText (context, label.Text, options, new Point (x, y));
		}

		public override int Affinity (Node node)
		{
			return StandardAffinity (node, typeof (LabelNode));
		}
	}
}
