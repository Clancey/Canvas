using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Xamarin.Canvas
{
	public class ImageNode : Node
	{
		public string File { get; private set; }
		public object Data { get; set; }

		public double XAlign { get; set; }
		public double YAlign { get; set; }
		
		public ImageNode (string file)
		{
			File = file;
			XAlign = 0.5;
			YAlign = 0.5;
		}
	}
}
