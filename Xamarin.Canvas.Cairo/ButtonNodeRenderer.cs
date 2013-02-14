using System;
using System.Collections.Generic;
using System.Linq;

using Cairo;

namespace Xamarin.Canvas.Cairo
{
	
	public class ButtonNodeRenderer : AbstractCairoRenderer
	{
		public override void Render (Node node, Context context)
		{
			ButtonNode button = node as ButtonNode;

			context.RoundedRectangle (0.5, 0.5, button.Width - 1, button.Height - 1, button.Rounding);
			if (button.Relief) {
				using (var lg = new global::Cairo.LinearGradient (0, 0, 0, button.Height)) {
					CreateGradient (lg, button.State, button.Opacity);
					context.Pattern = lg;
					context.FillPreserve ();
				}
				
				context.LineWidth = 1;
				context.Color = new Color (0.8, 0.8, 0.8, button.Opacity).ToCairo ();
				context.Stroke ();
			}
		}

		public override void LayoutOutline (Node node, Context context)
		{
			context.Rectangle (0, 0, node.Width, node.Height);
		}

		public override void ClipChildren (Node node, Context context)
		{
			ButtonNode button = node as ButtonNode;
			context.RoundedRectangle (0.5, 0.5, button.Width - 1, button.Height - 1, button.Rounding);
			context.Clip ();
		}

		public override bool Clip { get { return true; } }

		public override int Affinity (Node node)
		{
			return StandardAffinity (node, typeof(ButtonNode));
		}

		static void CreateGradient (global::Cairo.LinearGradient lg, NodeState state, double opacity)
		{
			if (state.HasFlag (NodeState.Pressed)) {
				lg.AddColorStop (0, new Color (0.9, 0.9, 0.9, opacity).ToCairo ());
				lg.AddColorStop (1, new Color (1, 1, 1, opacity).ToCairo ());
			} else if (state.HasFlag (NodeState.Prelight)) {
				lg.AddColorStop (0, new Color (1, 1, 1, opacity).ToCairo ());
				lg.AddColorStop (1, new Color (0.95, 0.95, 0.95, opacity).ToCairo ());
			} else {
				lg.AddColorStop (0, new Color (1, 1, 1, opacity).ToCairo ());
				lg.AddColorStop (1, new Color (0.9, 0.9, 0.9, opacity).ToCairo ());
			}
			
		}
	}
}
