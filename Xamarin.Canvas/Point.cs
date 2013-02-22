using System;

namespace Xamarin.Canvas
{
	public struct Point
	{
		public double X;
		public double Y;

		public Point (double x, double y)
		{
			X = x;
			Y = y;
		}

		public double Distance (Point other)
		{
			return Math.Sqrt (Math.Pow (X - other.X, 2) + Math.Pow (Y - other.Y, 2));
		}
	}

	public struct Vec2
	{
		public double X;
		public double Y;

		public Vec2 (double x, double y)
		{
			X = x;
			Y = y;
		}
	}
}
