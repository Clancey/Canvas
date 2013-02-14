using System;
using System.Collections.Generic;
using System.Linq;

using Cairo;

namespace Xamarin.Canvas.Cairo
{
	public class NullNodeRenderer : ICairoRenderer
	{
		#region ICairoRenderer implementation
		
		public void LayoutOutline (Node node, Context context)
		{
		}
		
		public void Render (Node node, Context context)
		{
		}
		
		public void PostRender (Node node, Context context)
		{	
		}
		
		public void ClipChildren (Node node, Context context)
		{
		}
		
		public bool Clip { get { return false; } }
		public bool Post { get { return false; } }
		
		#endregion
		
		#region IRenderer implementation
		
		public int Affinity (Node node)
		{
			return 0;
		}
		
		#endregion
	}
	
}
