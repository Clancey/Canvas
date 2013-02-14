using System;
using System.Collections.Generic;
using System.Linq;

using Cairo;

namespace Xamarin.Canvas.Cairo
{
	public static class ColorExtensions
	{
		public static global::Cairo.Color ToCairo (this Xamarin.Canvas.Color color)
		{
			return new global::Cairo.Color (color.R, color.G, color.B, color.A);
		}
	}
	
}
