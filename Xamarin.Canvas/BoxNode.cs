using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Xamarin.Canvas
{
	public class BoxNode : Node
	{
		public Color Color { get; private set; }
		
		public BoxNode (Color color, int width = 1, int height = 1)
		{
			Color = color;
			AnchorX = width / 2.0;
			AnchorY = height / 2.0;
			
			SetPreferedSize (width, height);
		}
	}
}

