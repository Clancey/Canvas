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
	public class ImageNodeRenderer: NodeUIView
	{
		ImageNode node;
		UIImageView image;
		
		public ImageNodeRenderer (ImageNode node)
			: base (node)
		{
			this.node = node;
			image = new UIImageView (new UIImage (node.File));
			AddSubview (image);
			Console.WriteLine ("Create Image Node");
		}
		
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			image.Frame = new RectangleF (0, 0, (float) node.Width, (float) node.Height);
		}
	}
	
}
