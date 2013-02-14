using System;
using System.Collections.Generic;
using Gtk;

using Xamarin.Canvas;
using Xamarin.Canvas.Gtk;
using Xamarin.Motion;

namespace Xamarin.Canvas.Example
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow win = new MainWindow ();

			Gtk.Canvas canvas = new Gtk.Canvas ();
			canvas.Engine.SetBackground (new Color (0.5, 0.5, 0.5));
			win.Add (canvas);

			BoxNode box = new BoxNode (new Color (1, 1, 1), 100, 100);
			box.Draggable = true;
			canvas.Root.Add (box);

			PolygonNode poly = new PolygonNode (50, 50);
			poly.Color = new Color (0, 0, 1);
			poly.SetVerticies (new [] {
				new Point (0, 1),
				new Point (0, 0),
				new Point (1, 0.5),
			});
			poly.Draggable = true;
			canvas.Root.Add (poly);

			LabelNode label = new LabelNode ("Label Node Test");
			label.Color = new Color (1, 0, 0);
			label.Draggable = true;
			label.ClipInputToTextExtents = true;
			canvas.Root.Add (label);

			Random r = new Random ();
			box.ClickEvent += (sender, e) => {
				box.RotateTo (r.NextDouble () * 5);
			};

			ButtonNode button = new ButtonNode (new LabelNode ("Button"));
			canvas.Root.Add (button);
			button.MoveTo (300, 100);

			win.SetSizeRequest (800, 200);
			win.ShowAll ();

			Application.Run ();
		}
	}
}
