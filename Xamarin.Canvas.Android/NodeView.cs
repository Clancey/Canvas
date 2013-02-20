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

			Click += HandleClick;
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
			ScaleX = (float)node.Scale;
			ScaleY = (float)node.Scale;
		}

		public void LayoutByNode ()
		{
			UpdateNativeView ();
			Layout ((int)node.X, (int)node.Y, (int)(node.X + node.Width), (int)(node.Y + node.Height));
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
