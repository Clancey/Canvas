using System;
using System.Collections.Generic;
using System.Linq;

using Cairo;

namespace Xamarin.Canvas.Cairo
{
	public static class PointExtensions
	{
		public static PointD ToCairo (this Point point)
		{
			return new PointD (point.X, point.Y);
		}
	}
	
}
