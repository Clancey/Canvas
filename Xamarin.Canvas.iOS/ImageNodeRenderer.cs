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
			imageView.Opaque = node.IsOpaque;
			SetView (imageView);		
		}
	}
	
}
