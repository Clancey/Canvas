using System;
using System.Collections.Generic;
using System.Linq;

using Cairo;

namespace Xamarin.Canvas.Cairo
{

	public static class CairoExtensions
	{	
		public static void RoundedRectangle(this global::Cairo.Context cr, double x, double y, double w, double h, double r)
		{
			cr.MoveTo(x + r, y);
			cr.Arc(x + w - r, y + r, r, Math.PI * 1.5, Math.PI * 2);
			cr.Arc(x + w - r, y + h - r, r, 0, Math.PI * 0.5);
			cr.Arc(x + r, y + h - r, r, Math.PI * 0.5, Math.PI);
			cr.Arc(x + r, y + r, r, Math.PI, Math.PI * 1.5);
		}
	}
	
}
