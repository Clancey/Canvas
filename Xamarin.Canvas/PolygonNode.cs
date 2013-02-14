using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Xamarin.Canvas
{
	public class PolygonNode : Node
	{
		List<Point> verticies;
		
		public Color Color { get; set; }

		public ReadOnlyCollection<Point> Verticies {
			get { return verticies.AsReadOnly (); }
		}
		
		public PolygonNode (double width, double height)
		{
			SetPreferedSize (width, height);
			Color = new Color (0, 0, 0);
			verticies = new List<Point> ();
		}
		
		public void SetVerticies (IEnumerable<Point> verticies)
		{
			this.verticies.Clear ();
			this.verticies.AddRange (verticies);
			Normalize ();
		}
		
		void Normalize ()
		{
			var max = verticies.Aggregate ((agg, next) => agg = new Point (Math.Max (agg.X, next.X), Math.Max (agg.Y, next.Y)));
			var min = verticies.Aggregate ((agg, next) => agg = new Point (Math.Min (agg.X, next.X), Math.Min (agg.Y, next.Y)));
			
			verticies.ForEach (vert => {
				vert.X = (vert.X + min.Y) / max.Y;
				vert.Y = (vert.Y + min.Y) / max.Y;
			});
		}
	}
}
