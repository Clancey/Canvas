using System;
using System.Collections.Generic;
using System.Linq;

using Cairo;

namespace Xamarin.Canvas.Cairo
{
	public class ImageNodeRenderer : AbstractCairoRenderer
	{
		public override void LayoutOutline (Node node, Context context)
		{
			ImageNode image = node as ImageNode;
			
			ImageSurface surfaceCache = image.Data as ImageSurface;
			if (surfaceCache == null) {
				// will improve with CGLayer surfaces
				surfaceCache = new ImageSurface (image.File);
			}

			context.Rectangle (0, 0, image.Width, image.Height);
		}

		public override void Render (Node node, Context context)
		{
			ImageNode image = node as ImageNode;

			ImageSurface surfaceCache = image.Data as ImageSurface;
			if (surfaceCache == null) {
				surfaceCache = new ImageSurface (image.File);
			}
			int x = (int)((image.Width - surfaceCache.Width) * image.XAlign);
			int y = (int)((image.Height - surfaceCache.Height) * image.YAlign);
			context.SetSourceSurface (surfaceCache, x, y);
			double opacity = image.Opacity;
			if (opacity == 1)
				context.Paint ();
			else
				context.PaintWithAlpha (image.Opacity);
		}

		public override int Affinity (Node node)
		{
			return StandardAffinity (node, typeof (ImageNode));
		}
	}
}
