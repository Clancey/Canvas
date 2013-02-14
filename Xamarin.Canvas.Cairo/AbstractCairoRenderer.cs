using System;
using System.Collections.Generic;
using System.Linq;

using Cairo;

namespace Xamarin.Canvas.Cairo
{

	public abstract class AbstractCairoRenderer : ICairoRenderer
	{
		#region ICairoRenderer implementation
		public virtual void LayoutOutline (Node node, Context context) {}
		public virtual void Render (Node node, Context context) {}
		public virtual void PostRender (Node node, Context context) {}
		public virtual void ClipChildren (Node node, Context context) {}

		public virtual bool Clip { get { return false; } }

		public virtual bool Post { get { return false; } }
		#endregion

		#region IRenderer implementation
		public virtual int Affinity (Node node)
		{
			return -1;
		}
		#endregion

		protected int StandardAffinity (Node node, Type type)
		{
			Type nodeType = node.GetType ();
			if (nodeType.IsSubclassOf (type))
				return 1;
			if (nodeType.IsAssignableFrom (type))
				return 2;
			return -1;
		}
	}
}
