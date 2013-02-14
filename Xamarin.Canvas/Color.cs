using System;

namespace Xamarin.Canvas
{
	public static class NumericExtensions
	{
		public static double Clamp (this double self, double min, double max)
		{
			return Math.Min (max, Math.Max (self, min));
		}
	}

	public struct Color
	{
		bool hslDirty;
		bool rgbDirty;
		
		double a;
		public double A {
			get { return a; }
			set {
				value = value.Clamp (0, 1);
				a = value; 
			}
		}
		
		double r;
		public double R {
			get {
				CheckDirty ();
				return r;
			}
			set {
				value = value.Clamp (0, 1);
				if (r == value)
					return;
				r = value;
				hslDirty = true;
			}
		}
		double g;
		public double G {
			get {
				CheckDirty ();
				return g;
			}
			set {
				value = value.Clamp (0, 1);
				if (g == value)
					return;
				g = value;
				hslDirty = true;
			}
		}
		double b;
		public double B {
			get {
				CheckDirty ();
				return b;
			}
			set {
				value = value.Clamp (0, 1);
				if (b == value)
					return;
				b = value;
				hslDirty = true;
			}
		}
		
		double hue;
		public double Hue {
			get {
				CheckDirty ();
				return hue;
			}
			set {
				value = value.Clamp (0, 1);
				if (hue == value)
					return;
				hue = value;
				rgbDirty = true;
			}
		}
		double saturation;
		public double Saturation {
			get {
				CheckDirty ();
				return saturation;
			}
			set {
				value = value.Clamp (0, 1);
				if (saturation == value)
					return;
				saturation = value;
				rgbDirty = true;
			}
		}
		double luminosity;
		public double Luminosity {
			get {
				CheckDirty ();
				return luminosity;
			}
			set {
				value = value.Clamp (0, 1);
				if (luminosity == value)
					return;
				luminosity = value;
				rgbDirty = true;
			}
		}
		
		public Color (double r, double g, double b, double a)
		{
			this.r = r.Clamp (0, 1);
			this.g = g.Clamp (0, 1);
			this.b = b.Clamp (0, 1);
			this.a = a.Clamp (0, 1);
			
			hue = luminosity = saturation = 0;
			
			hslDirty = true;
			rgbDirty = false;
		}
		
		public Color (double r, double g, double b)
		{
			this.r = r.Clamp (0, 1);
			this.g = g.Clamp (0, 1);
			this.b = b.Clamp (0, 1);
			this.a = 1;
			
			hue = luminosity = saturation = 0;
			
			hslDirty = true;
			rgbDirty = false;
		}

		public Color MultiplyAlpha (double alpha)
		{
			return new Color (r, g, b, a * alpha);
		}
		
		void CheckDirty ()
		{
			if (rgbDirty)
				UpdateRGBFromHSL ();
			else if (hslDirty)
				UpdateHSLFromRGB ();
			
			rgbDirty = false;
			hslDirty = false;
		}
		
		void UpdateRGBFromHSL ()
		{
			if (luminosity == 0) {
				r = g = b = 0;
				return;
			}
			
			if (saturation == 0) {
				r = g = b = luminosity;
			} else {
				double temp2 = luminosity <= 0.5 ? luminosity * (1.0 + saturation) : luminosity + saturation - (luminosity * saturation);
				double temp1 = 2.0 * luminosity - temp2;
				
				double[] t3 = new double[] { hue + 1.0 / 3.0, hue, hue - 1.0 / 3.0 };
				double[] clr= new double[] { 0, 0, 0 };
				for (int i = 0; i < 3; i++) {
					if (t3[i] < 0)
						t3[i] += 1.0;
					if (t3[i] > 1)
						t3[i] -= 1.0;
					if (6.0 * t3[i] < 1.0)
						clr[i] = temp1 + (temp2 - temp1) * t3[i] * 6.0;
					else if (2.0 * t3[i] < 1.0)
						clr[i] = temp2;
					else if (3.0 * t3[i] < 2.0)
						clr[i] = (temp1 + (temp2 - temp1) * ((2.0 / 3.0) - t3[i]) * 6.0);
					else
						clr[i] = temp1;
				}
				
				r = clr[0];
				g = clr[1];
				b = clr[2];
			}
		}
		
		void UpdateHSLFromRGB ()
		{
			double v = System.Math.Max (r, g);
			v = System.Math.Max (v, b);
			
			double m = System.Math.Min (r, g);
			m = System.Math.Min (m, b);
			
			double l = (m + v) / 2.0;
			if (l <= 0.0) {
				hue = saturation = luminosity = 0;
				return;
			}
			double vm = v - m;
			double s = vm;
			
			if (s > 0.0) {
				s /= (l <= 0.5) ? (v + m) : (2.0 - v - m);
			} else {
				hue = 0; saturation = 0; luminosity = l;
				return;
			}
			
			double r2 = (v - r) / vm;
			double g2 = (v - g) / vm;
			double b2 = (v - b) / vm;
			
			double h;
			if (r == v) {
				h = (g == m ? 5.0 + b2 : 1.0 - g2);
			} else if (g == v) {
				h = (b == m ? 1.0 + r2 : 3.0 - b2);
			} else {
				h = (r == m ? 3.0 + g2 : 5.0 - r2);
			}
			h /= 6.0;
			
			hue = h;
			saturation = s;
			luminosity = l;
		}
		
		public static bool operator == (Color color1, Color color2)
		{
			return color1.r == color2.r && color1.g == color2.g && color1.b == color2.b && color1.a == color2.a;
		}
		
		public static bool operator != (Color color1, Color color2)
		{
			return color1.r != color2.r || color1.g != color2.g || color1.b != color2.b || color1.a != color2.a;
		}
		
		public override int GetHashCode ()
		{
			return r.GetHashCode () + g.GetHashCode () + b.GetHashCode () + a.GetHashCode ();
		}
		
		public override bool Equals (object obj)
		{
			if (obj is Color) {
				var color = (Color)obj;
				return color.r == r && color.g == g && color.b == b && color.a == a;
			}
			return base.Equals (obj);
		}
		
		public override string ToString ()
		{
			return string.Format ("[Color: A={0}, R={1}, G={2}, B={3}, Hue={4}, Saturation={5}, Luminosity={6}]", A, R, G, B, Hue, Saturation, Luminosity);
		}
	}
}

