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

	public class BoxNodeRenderer : NodeUIView
	{
		BoxNode node;

		public BoxNodeRenderer (BoxNode node)
			: base (node)
		{
			this.node = node;
		}

		public override void Draw (RectangleF rect)
		{	
			var context = UIGraphics.GetCurrentContext ();
			context.SetFillColor (node.Color.ToCGColor ());
			context.FillRect (rect);
		}
	}

}
