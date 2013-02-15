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
	public static class ColorExtensions
	{
		public static CGColor ToCGColor (this Color color)
		{
			return new CGColor ((float)color.R, (float)color.G, (float)color.B, (float)color.A);
		}

		public static UIColor ToUIColor (this Color color)
		{
			return new UIColor ((float)color.R, (float)color.G, (float)color.B, (float)color.A);
		}
	}
	
}
