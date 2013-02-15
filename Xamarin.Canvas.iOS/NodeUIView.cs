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
			
			node.RedrawNeeded += (o, a) => UpdateNativeWidget ();
			node.SizeChanged += (o, a) => UpdateNativeWidget ();
			
			UpdateNativeWidget ();

			singleTouchTap = CreateTapRecognizer (1, 1, r => node.Tap (new TapEventArgs (1, UIStateToNodeState (r.State))));
			doubleTouchTap = CreateTapRecognizer (2, 1, r => node.Tap (new TapEventArgs (2, UIStateToNodeState (r.State))));
			tripleTouchTap = CreateTapRecognizer (3, 1, r => node.Tap (new TapEventArgs (3, UIStateToNodeState (r.State))));
			doubleTap = CreateTapRecognizer (1, 2, r => node.Tap (new TapEventArgs (2, UIStateToNodeState (r.State))));

			AddGestureRecognizer (singleTouchTap);
			AddGestureRecognizer (doubleTouchTap);
			AddGestureRecognizer (tripleTouchTap);
			AddGestureRecognizer (doubleTap);
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
			UITapGestureRecognizer result = new UITapGestureRecognizer (r => {
				action (r);
				Console.WriteLine ("Tap");
			});
			result.NumberOfTouchesRequired = (uint)numFingers;
			result.NumberOfTapsRequired = (uint)numTaps;
			return result;
		}
		
		void UpdateNativeWidget ()
		{
			Frame = new RectangleF ((float)node.X, (float)node.Y, (float)node.Width, (float)node.Height);
			
			CATransform3D transform = CATransform3D.Identity;
			transform.m34 = 1.0f / -2000f;
			transform = transform.Rotate ((float)node.Rotation * (float)Math.PI / 180.0f, 0.0f, 0.0f, 1.0f);
			Layer.Transform = transform;
			
			SetNeedsDisplay ();
		}
	}
	
}
