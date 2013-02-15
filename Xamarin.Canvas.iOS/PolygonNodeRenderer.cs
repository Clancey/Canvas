using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;
using MonoTouch.CoreAnimation;
using System.ComponentModel;
using MonoTouch.Foundation;

namespace Xamarin.Canvas.iOS
{
	public class PolygonNodeRenderer : NodeUIView
	{
		PolygonNode node;
		
		public PolygonNodeRenderer (PolygonNode node)
			: base (node)
		{
			this.node = node;
		}
		
		public override void Draw (RectangleF rect)
		{	
			if (node.Verticies.Count < 3)
				return;

			var context = UIGraphics.GetCurrentContext ();
			context.MoveTo ((float)node.Verticies.First ().X, (float)node.Verticies.First ().Y);
			node.Verticies.Skip (1).ForEach (v => context.AddLineToPoint ((float)v.X, (float)v.Y));
			context.SetFillColor (node.Color.ToCGColor ());
			context.FillPath ();
		}
	}

}
