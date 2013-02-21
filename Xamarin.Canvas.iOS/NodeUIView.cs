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
	public class NodeTouchEvent : TouchEvent
	{
		Point point;
		TouchType type;
		Vec2 velocity;

		public override Point Point {
			get { return point; }
		}

		public override TouchType Type {
			get { return type; }
		}

		public override Vec2 Velocity {
			get { return velocity; }
		}

		public NodeTouchEvent (NodeUIView sender, NSSet touches, UIEvent evt, TouchType type)
		{
			this.type = type;

			UITouch touch = touches.AnyObject as UITouch;
			var touchLocation = touch.LocationInView (sender);
			point = new Point (touchLocation.X, touchLocation.Y);
		}
	}

	public class NodeUIView : UIView, ICanvasRenderer
	{
		Node node;
		UITapGestureRecognizer singleTouchTap, doubleTouchTap, tripleTouchTap;
		UITapGestureRecognizer doubleTap;

		public Node Node { get { return node; } }
		
		public NodeUIView (Node node)
		{
			this.node = node;
			this.BackgroundColor = new Color (0.2, 0.4, 0.6).ToUIColor ();
			
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

			node.Touch (new NodeTouchEvent (this, touches, evt, TouchType.Down));
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);

			node.Touch (new NodeTouchEvent (this, touches, evt, TouchType.Move));
		}

		public override void TouchesCancelled (NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled (touches, evt);
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);

			node.Touch (new NodeTouchEvent (this, touches, evt, TouchType.Up));
		}

		void UpdateChildrenOrder ()
		{
			foreach (var child in node.Children) {
				UIView nativeControl = child.Renderer as UIView;
				BringSubviewToFront (nativeControl);
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

		CALayer layer;
		bool inputTransparent;
		protected virtual void UpdateNativeWidget ()
		{
			if (layer == null) {
				layer = Layer;
			}

			if (inputTransparent != node.InputTransparent) {
				UserInteractionEnabled = !node.InputTransparent;
				inputTransparent = node.InputTransparent;
			}

			Frame = new RectangleF ((float)node.X, (float)node.Y, (float)node.Width, (float)node.Height);
			layer.AnchorPoint = new PointF ((float)node.AnchorX, (float)node.AnchorY);
			layer.Opacity = (float)node.Opacity;
			
			CATransform3D transform = CATransform3D.Identity;
			transform = transform.Scale ((float)node.Scale);
			transform.m34 = 1.0f / -400f;
			transform = transform.Rotate ((float)node.Rotation * (float)Math.PI / 180.0f, 0.0f, 0.0f, 1.0f);
			transform = transform.Rotate ((float)node.RotationX * (float)Math.PI / 180.0f, 1.0f, 0.0f, 0.0f);
			transform = transform.Rotate ((float)node.RotationY * (float)Math.PI / 180.0f, 0.0f, 1.0f, 0.0f);
			layer.Transform = transform;
			
			SetNeedsDisplay ();
		}
	}
	
}
