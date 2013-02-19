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

	public class NativeViewRenderer : NodeUIView
	{
		Node node;
		UIView view;
		
		public NativeViewRenderer (Node node)
			: base (node)
		{
			this.node = node;
		}

		protected virtual void SetView (UIView view)
		{
			this.view = view;
			AddSubview (view);
		}
		
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			if (view != null)
				view.Frame = new RectangleF (0, 0, (float) node.Width, (float) node.Height);
		}
	}
	
}
