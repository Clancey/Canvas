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

	public class BoxNodeView : NodeView
	{
		BoxNode box;

		public BoxNodeView (Context context, BoxNode box)
			: base (context, box)
		{
			this.box = box;
		}

		protected override void UpdateNativeView ()
		{
			base.UpdateNativeView ();
			this.SetBackgroundColor (box.Color.ToAndroid ());
		}
	}
	
}
