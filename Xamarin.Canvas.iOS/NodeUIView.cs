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
	public class NodeUIView : UIView, ICanvasRenderer
	{
		Node node;
		UITapGestureRecognizer singleTouchTap, doubleTouchTap, tripleTouchTap;
		UITapGestureRecognizer doubleTap;

		public Node Node { get { return node; } }
		
		public NodeUIView (Node node)
		{
			this.node = node;
			this.BackgroundColor = UIColor.Clear;
			
			node.RedrawNeeded += (o, a) => { if (o == node) UpdateNativeWidget (); };
			node.SizeChanged += (o, a) => UpdateNativeWidget ();

			singleTouchTap = CreateTapRecognizer (1, 1, r => node.Tap (new TapEventArgs (1, UIStateToNodeState (r.State))));
			doubleTouchTap = CreateTapRecognizer (2, 1, r => node.Tap (new TapEventArgs (2, UIStateToNodeState (r.State))));
			tripleTouchTap = CreateTapRecognizer (3, 1, r => node.Tap (new TapEventArgs (3, UIStateToNodeState (r.State))));
			doubleTap      = CreateTapRecognizer (1, 2, r => node.Tap (new TapEventArgs (2, UIStateToNodeState (r.State))));

			AddGestureRecognizer (singleTouchTap);
			AddGestureRecognizer (doubleTouchTap);
			AddGestureRecognizer (tripleTouchTap);
			AddGestureRecognizer (doubleTap);

			node.ChildrenReordered += (o, a) => UpdateChildrenOrder ();
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);
		}

		public override void TouchesCancelled (NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled (touches, evt);
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);
		}

		void UpdateChildrenOrder ()
		{
			foreach (var child in node.Children) {
				UIView nativeControl = child.Renderer as UIView;
				nativeControl.RemoveFromSuperview ();
				AddSubview (nativeControl);
			}
		}

		public override void LayoutSubviews ()
		{
			UpdateNativeWidget ();
		}

		GestureState UIStateToNodeState (UIGestureRecognizerState state)
		{
			switch (state) {
			case UIGestureRecognizerState.Began:
				return GestureState.Began;
			case UIGestureRecognizerState.Cancelled:
				return GestureState.Cancelled;
			case UIGestureRecognizerState.Changed:
				return GestureState.Update;
			case UIGestureRecognizerState.Ended:
				return GestureState.Ended;
			case UIGestureRecognizerState.Failed:
				return GestureState.Failed;
			case UIGestureRecognizerState.Possible:
				return GestureState.Possible;
			}
			return GestureState.Failed;
		}

		UITapGestureRecognizer CreateTapRecognizer (int numFingers, int numTaps, Action<UITapGestureRecognizer> action)
		{
			UITapGestureRecognizer result = new UITapGestureRecognizer (action);
			result.NumberOfTouchesRequired = (uint)numFingers;
			result.NumberOfTapsRequired = (uint)numTaps;
			return result;
		}

		Size lastSize;
		CALayer layer;
		bool inputTransparent;
		protected virtual void UpdateNativeWidget ()
		{
			if (layer == null) {
				layer = Layer;
				lastSize = new Size ();
			}

			if (inputTransparent != node.InputTransparent) {
				UserInteractionEnabled = !node.InputTransparent;
				inputTransparent = node.InputTransparent;
			}

			if (node.Width != lastSize.Width || node.Height != lastSize.Height) 
				Frame = new RectangleF (0, 0, (float)node.Width, (float)node.Height);
			lastSize = new Size (node.Width, node.Height);

			layer.AnchorPoint = new PointF ((float)node.AnchorX, (float)node.AnchorY);
			layer.Opacity = (float)node.Opacity;
			
			CATransform3D transform = CATransform3D.Identity;
			transform.m34 = 1.0f / -2000f;
			transform = transform.Translate ((float)node.X, (float)node.Y, 0);
			transform = transform.Scale ((float)node.Scale);
			transform = transform.Rotate ((float)node.Rotation * (float)Math.PI / 180.0f, 0.0f, 0.0f, 1.0f);
			layer.Transform = transform;
			
			SetNeedsDisplay ();
		}
	}
	
}
