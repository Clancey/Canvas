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
			
			SetPreferedSize (width, height);
		}
	}
}

