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

	public class ImageNodeView : NodeView
	{
		ImageNode image;
		ImageView view;

		public ImageNodeView (Context context, ImageNode image)
			: base (context, image)
		{
			this.image = image;
			this.view = new ImageView (context);

			var bitmap = Resources.GetBitmap(image.File);
			view.SetImageBitmap (bitmap);
			AddView (view);
		}

		protected override void OnLayout (bool changed, int l, int t, int r, int b)
		{
			base.OnLayout (changed, l, t, r, b);
			view.Layout (0, 0, r - l, b - t);
		}
	}
	
}
