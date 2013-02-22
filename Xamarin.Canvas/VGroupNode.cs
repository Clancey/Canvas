using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Xamarin.Canvas
{
	public class VGroupNode : GroupNode
	{
		protected override void OnSizeAllocated (double width, double height)
		{
			double y = 0;
			foreach (var child in Children) {
				child.X = 0;
				child.Y = y;
				child.SetSize (width, child.PreferedHeight);
				y += child.Width;
			}
		}
		
		protected override void OnChildPreferedSizeChanged (object sender, EventArgs e)
		{
			var width = Children.Max (c => c.PreferedWidth);
			var height = Children.Sum (c => c.PreferedHeight);
			
			SetPreferedSize (width, height);
		}
	}
	
}
