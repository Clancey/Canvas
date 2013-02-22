using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Xamarin.Canvas
{

	public class LabelNode : Node
	{
		public double XAlign { get; set; }
		public double YAlign { get; set; }
		
		public bool ClipInputToTextExtents { get; set; }
		
		public Color Color { get; set; }

		string text;
		public string Text {
			get {
				return text;
			}
			set {
				if (text == value)
					return;
				text = value;
				UpdateSize ();
				QueueDraw ();
			}
		}
		
		public LabelNode (string label) : this ()
		{
			Text = label;
		}
		
		public LabelNode ()
		{
			Color = new Color (0, 0, 0);	
			CanvasSet += (sender, e) => UpdateSize ();
		}
		
		void UpdateSize ()
		{
			if (Canvas == null)
				return;

			var pixelSize = Canvas.Engine.TextExtents (Text, new TextOptions ());
			SetPreferedSize (pixelSize.Width, pixelSize.Height);
		}
	}
}
