using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.ComponentModel;

using global::Android.Content.PM;

namespace Xamarin.Canvas.Android
{
	[Activity (Label = "Xamarin.Canvas.Android", MainLauncher = true, HardwareAccelerated = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class Activity1 : Activity, ISynchronizeInvoke
	{
		Canvas canvas;
		Controls.Coverflow coverflow;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			Xamarin.Motion.Tweener.Sync = this;
			canvas = new Canvas (this.BaseContext);
			canvas.SetBackground (new Color (0, 0, 1));

			coverflow = new Xamarin.Canvas.Controls.Coverflow (new [] {
				"cover1.jpg", "cover2.jpg", "cover3.jpg", "cover4.jpg", "cover5.jpg", "cover6.jpg", 
				"cover7.jpg", "cover8.jpg", "cover9.jpg", "cover10.jpg", "cover1.jpg", "cover2.jpg", "cover3.jpg",
				"cover4.jpg", "cover5.jpg", "cover6.jpg", "cover7.jpg", "cover8.jpg", "cover9.jpg", "cover10.jpg",
			});

			canvas.Root.Add (coverflow);
			canvas.Root.SizeChanged += (sender, e) => {
				coverflow.WidthRequest = canvas.Root.Width;
				coverflow.HeightRequest = canvas.Root.Height;
			};


			LinearLayout layout = new LinearLayout (BaseContext);
			SetContentView (layout);

			layout.AddView (canvas);

		}

		public override void OnConfigurationChanged (global::Android.Content.Res.Configuration newConfig)
		{
			// we're good
			base.OnConfigurationChanged (newConfig);
		}

		protected override void OnDestroy ()
		{
			canvas.Destroy ();
			base.OnDestroy ();
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


