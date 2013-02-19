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
	public class ButtonNodeRenderer : NativeViewRenderer
	{
		ButtonNode node;

		public ButtonNodeRenderer (ButtonNode node)
			: base (node)
		{
			this.node = node;
			SetView (new UIButton (UIButtonType.RoundedRect));
		}
	}


	public class ImageNodeRenderer: NativeViewRenderer
	{
		ImageNode node;
		
		public ImageNodeRenderer (ImageNode node)
			: base (node)
		{
			this.node = node;
			SetView (new UIImageView (new UIImage (node.File)));
		}
	}
	
}
