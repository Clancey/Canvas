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
	public class LabelNodeRenderer : NodeUIView
	{
		LabelNode node;
		UILabel label;
		
		public LabelNodeRenderer (LabelNode node)
			: base (node)
		{
			this.node = node;
			label = new UILabel ();
			AddSubview (label);
		}

		protected override void UpdateNativeWidget ()
		{
			label.Text = node.Text;
			label.TextColor = node.Color.ToUIColor ();
			label.BackgroundColor = UIColor.Clear;

			base.UpdateNativeWidget ();
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			label.Frame = new RectangleF (PointF.Empty, Frame.Size);
		}
	}
}
