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
	public static class ColorExtensions
	{
		public static global::Android.Graphics.Color ToAndroid (this Color self)
		{
			return new global::Android.Graphics.Color ((byte)(byte.MaxValue * self.R),
			                                           (byte)(byte.MaxValue * self.G),
			                                           (byte)(byte.MaxValue * self.B),
			                                           (byte)(byte.MaxValue * self.A));
		}
	}
	
}
