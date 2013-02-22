using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.ComponentModel;

namespace Xamarin.Canvas.Android
{
	public class NodeTouchEvent : TouchEvent
	{
		TouchType type;
		Point point;
		VelocityTracker velocity;

		public override Point Point {
			get { return point; }
		}

		public override TouchType Type {
			get { return type; }
		}

		public override Vec2 Velocity {
			get {
				velocity.ComputeCurrentVelocity (1);
				return new Vec2 (velocity.XVelocity, velocity.YVelocity);
			}
		}

		public NodeTouchEvent (MotionEvent motion, VelocityTracker velocity)
		{
			this.velocity = velocity;
			velocity.AddMovement (motion);

			point = new Point (motion.GetX (), motion.GetY ());

			if (motion.Action == MotionEventActions.Move) {
				type = TouchType.Move;
			} else if (motion.Action == MotionEventActions.Down) {
				type = TouchType.Down;
			} else if (motion.Action == MotionEventActions.Up) {
				type = TouchType.Up;
			}
		}
	}

	public class NodeView : ViewGroup
	{
		Node node;
		VelocityTracker velocity;

		public NodeView (Context context, Node node) : base (context)
		{
			SetBackgroundColor (new Color (0, 0, 0, 0).ToAndroid ());
			this.node = node;
			node.RedrawNeeded += (sender, e) => {
				if (sender == node)
					UpdateNativeView (); 
			};
			node.SizeChanged += (sender, e) => UpdateNativeView ();
			node.ChildrenReordered += HandleChildrenReordered;

			Click += HandleClick;
		}

		public override bool OnInterceptTouchEvent (MotionEvent e)
		{
			if (node.TouchEvents) {
				if (e.Action == MotionEventActions.Move || e.Action == MotionEventActions.Down || e.Action == MotionEventActions.Up) {
					if (e.Action == MotionEventActions.Down || velocity == null)
						velocity = VelocityTracker.Obtain ();
					
					node.Touch (new NodeTouchEvent (e, velocity));
					
					if (e.Action == MotionEventActions.Up)
						velocity = null;
				}
			}
			return false;
		}

		Point down, up;
		public override bool OnTouchEvent (MotionEvent e)
		{
			if (e.Action == MotionEventActions.Down) {
				down = new Point (e.RawX, e.RawY);
			} else if (e.Action == MotionEventActions.Up) {
				up = new Point (e.RawX, e.RawY);
			}

			if (node.TouchEvents) {
				if (e.Action == MotionEventActions.Move || e.Action == MotionEventActions.Down || e.Action == MotionEventActions.Up) {
					if (e.Action == MotionEventActions.Down || velocity == null)
						velocity = VelocityTracker.Obtain ();

					var result = node.Touch (new NodeTouchEvent (e, velocity));

					if (e.Action == MotionEventActions.Up)
						velocity = null;

					return result;
				}
			}


			return base.OnTouchEvent (e);
		}

		void HandleChildrenReordered (object sender, EventArgs e)
		{
			ChildrenDrawingOrderEnabled = true;
			Invalidate ();
		}

		protected override int GetChildDrawingOrder (int childCount, int i)
		{
			if (node.Children == null)
				return i;

			var result = IndexOfChild (node.Children [i].Renderer as NodeView);
			return result;
		}

		void HandleClick (object sender, EventArgs e)
		{
			if (up.Distance (down) < 100)
				node.Tap (new TapEventArgs (1, GestureState.Ended));
		}

		protected virtual void UpdateNativeView ()
		{
			if (node.AnchorX != 0.5)
				PivotX = (float)(node.AnchorX * node.Width);
			if (node.AnchorY != 0.5)
				PivotY = (float)(node.AnchorY * node.Height);

			SetX ((float)node.X);
			SetY ((float)node.Y);
			Alpha = (float)node.Opacity;
			Rotation = (float)node.Rotation;
			RotationX = (float)node.RotationX;
			RotationY = (float)node.RotationY;
			ScaleX = (float)node.Scale;
			ScaleY = (float)node.Scale;
		}

		public void LayoutByNode ()
		{
			UpdateNativeView ();
			Layout ((int)node.X, (int)node.Y, (int)(node.X + node.Width), (int)(node.Y + node.Height));
			OnLayout (true, Left, Top, Right, Bottom);
		}

		protected override void OnLayout (bool changed, int l, int t, int r, int b)
		{
			for (int i = 0; i < this.ChildCount; i++) {
				NodeView view = GetChildAt (i) as NodeView;
				if (view != null) {
					view.LayoutByNode ();
				}
			}
		}
	}
	
}
