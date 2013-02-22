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
	public class LabelNodeView : NodeView
	{
		TextView view;
		LabelNode label;

		public LabelNodeView (Context context, LabelNode label)
			: base (context, label)
		{
			this.label = label;
			this.view = new TextView (context);
			view.Text = label.Text;
			AddView (view);
		}

		protected override void UpdateNativeView ()
		{
			view.Text = label.Text;
			view.SetTextColor (label.Color.ToAndroid ());

			base.UpdateNativeView ();
		}

		protected override void OnLayout (bool changed, int l, int t, int r, int b)
		{
			base.OnLayout (changed, l, t, r, b);
			view.Layout (0, 0, r - l, b - t);
		}
	}
	
}
