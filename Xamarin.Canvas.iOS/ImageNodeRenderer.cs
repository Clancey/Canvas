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


	public class ImageNodeRenderer: NativeViewRenderer
	{
		ImageNode node;
		
		public ImageNodeRenderer (ImageNode node)
			: base (node)
		{
			this.node = node;
			var imageView = new UIImageView (new UIImage (node.File));
			imageView.Layer.ShouldRasterize = true;
			imageView.Opaque = node.IsOpaque;
			SetView (imageView);

			if (node.Hints.Shadow) {
				imageView.Layer.ShadowOpacity = 0.5f;
				imageView.Layer.ShadowOffset = new SizeF (0, 0);
			}
		}
	}
	
}
