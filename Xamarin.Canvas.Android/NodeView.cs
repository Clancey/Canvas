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

	public class NodeView : ViewGroup
	{
		Node node;

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

		public override bool OnTouchEvent (MotionEvent e)
		{
			var x = e.GetX ();
			var y = e.GetY ();
			if (e.Action == MotionEventActions.Move) {
				return node.Touch (new TouchEvent (x, y, TouchType.Move));
			} else if (e.Action == MotionEventActions.Down) {
				return node.Touch (new TouchEvent (x, y, TouchType.Down));
			} else if (e.Action == MotionEventActions.Up) {
				return node.Touch (new TouchEvent (x, y, TouchType.Up));
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
			node.Tap (new TapEventArgs (1, GestureState.Ended));
		}

		protected virtual void UpdateNativeView ()
		{
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
