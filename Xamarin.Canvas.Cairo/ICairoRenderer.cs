using System;
using System.Collections.Generic;
using System.Linq;

using Cairo;

namespace Xamarin.Canvas.Cairo
{
	public interface ICairoRenderer : IRenderer
	{
		bool Clip { get; }
		bool Post { get; }

		void LayoutOutline (Node node, Context context);
		void Render        (Node node, Context context);
		void PostRender    (Node node, Context context);
		void ClipChildren  (Node node, Context context);
	}

}
