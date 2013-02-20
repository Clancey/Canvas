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
	[Activity (Label = "Xamarin.Canvas.Android", MainLauncher = true)]
	public class Activity1 : Activity, ISynchronizeInvoke
	{
		Canvas canvas;

		protected override void OnCreate (Bundle bundle)
		{
			Xamarin.Motion.Tweener.Sync = this;
			base.OnCreate (bundle);
			canvas = new Canvas (this.BaseContext);
			canvas.SetBackground (new Color (0, 0, 1));

			BoxNode box = new BoxNode (new Color (1, 0, 0), 100, 100);
			box.X = 50;
			box.Y = 50;
			canvas.Root.Add (box);

			ImageNode image = new ImageNode ("cover1.jpg");
			image.X = 150;
			image.Y = 150;
			canvas.Root.Add (image);

			image.ActivatedEvent += (sender, e) => image.RelRotateTo (50);

			// Set our view from the "main" layout resource
			LinearLayout layout = new LinearLayout (BaseContext);
			SetContentView (layout);

			layout.AddView (canvas);

		}

		#region ISynchronizeInvoke implementation
		
		IAsyncResult ISynchronizeInvoke.BeginInvoke (Delegate method, object[] args)
		{
			RunOnUiThread (() => method.DynamicInvoke (args));
			return null;
		}
		
		object ISynchronizeInvoke.EndInvoke (IAsyncResult result)
		{
			return null;
		}
		
		object ISynchronizeInvoke.Invoke (Delegate method, object[] args)
		{
			RunOnUiThread (() => method.DynamicInvoke (args));
			return null;
		}
		
		bool ISynchronizeInvoke.InvokeRequired {
			get { return true; }
		}
		
		#endregion
	}
}


