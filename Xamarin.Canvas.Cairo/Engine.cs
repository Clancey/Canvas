using System;
using System.Collections.Generic;
using System.Linq;

using Cairo;

namespace Xamarin.Canvas.Cairo
{

	public class Engine : Canvas.ICanvasEngine
	{
		List<ICairoRenderer> renderers;
		Color background;
		Func<Context> contextInitializer;
		Func<Pango.Context> pangoInitializer;

		public bool Supports3D { get { return false; } }

		public Engine (Func<Context> contextInitializer, Func<Pango.Context> pangoInitializer)
		{
			this.contextInitializer = contextInitializer;
			this.pangoInitializer = pangoInitializer;
			renderers = new List<ICairoRenderer> ();

			InsertStandardRenderers ();
		}

		void InsertStandardRenderers ()
		{
			InsertRenderer (new BoxNodeRenderer ());
			InsertRenderer (new NullNodeRenderer ());
			InsertRenderer (new PolygonNodeRenderer ());
			InsertRenderer (new LabelNodeRenderer ());
			InsertRenderer (new ImageNodeRenderer ());
			InsertRenderer (new ButtonNodeRenderer ());
		}

		public Point TransformPoint (Node node, double x, double y)
		{
			node.GetInverseTransform ().TransformPoint (ref x, ref y);
			return new Point (x, y);
		}

		#region Text Handling
		public Size ImageSize (string file)
		{
			ImageSurface image = new ImageSurface (file);
			Size result = new Size (image.Width, image.Height);
			(image as IDisposable).Dispose ();
			return result;
		}

		public Size TextExtents (string text, TextOptions options)
		{
			int w, h;
			using (var layout = new Pango.Layout (pangoInitializer ())) {
				layout.SetText (text);
				layout.GetPixelSize (out w, out h);
			}
			return new Size (w, h);
		}

		public void RenderText (Context context, string text, TextOptions options, Point position)
		{
			using (var layout = new Pango.Layout (pangoInitializer ())) {
				layout.SetText (text);

				layout.Width = Pango.Units.FromPixels ((int)options.MaxWidth);
			
				context.MoveTo (position.X, position.Y);
				context.Color = options.Color.ToCairo ();
				Pango.CairoHelper.ShowLayout (context, layout);
			}
		}
		#endregion

		public Node InputNodeAt (Node root, double x, double y)
		{
			if (root.InputTransparent)
				return null;
			
			var children = root.Children;
			
			if (children != null) {
				// Manual loop to avoid excessive creation of iterators
				for (int i = children.Count - 1; i >= 0; i--) {
					var result = InputNodeAt (children[i], x, y);
					if (result != null)
						return result;
				}
			}

			ICairoRenderer renderer = root.Renderer as ICairoRenderer;
			if (renderer == null)
				return null;

			using (var context = contextInitializer ()) {
				context.Save ();
				renderer.LayoutOutline (root, context);
				var point = TransformPoint (root, x, y);
				
				if (context.InFill (point.X, point.Y)) {
					context.NewPath ();
					context.Restore ();
					return root;
				}
				context.NewPath ();
				context.Restore ();
			}
			return null;
		}

		public void SetBackground (Color color)
		{
			background = color;
		}

		public void RenderScene (Node rootNode)
		{
			using (var context = contextInitializer ()) {
				context.Operator = Operator.Source;
				context.Color = background.ToCairo ();
				context.Paint ();
				context.Operator = Operator.Over;
				
				RenderNode (rootNode, context);
			}
		}

		void RenderNode (Node current, Context context)
		{
			if (current.Renderer == null)
				AssignRenderers (current);
			
			var cairoRenderer = current.Renderer as ICairoRenderer;
			if (cairoRenderer == null)
				return;

			context.Save ();
			context.Transform (current.GetTransform ());
			cairoRenderer.Render (current, context);

			context.Save ();

			if (cairoRenderer.Clip)
				cairoRenderer.ClipChildren (current, context);

			if (current.Children != null)
				current.Children.ForEach (n => RenderNode (n, context));

			context.Restore ();

			if (cairoRenderer.Post)
				cairoRenderer.PostRender (current, context);

			context.Restore ();
		}

		public bool InsertRenderer (ICairoRenderer renderer)
		{
			var cairoRenderer = renderer;

			if (cairoRenderer == null)
				return false;

			renderers.Add (cairoRenderer);
			return true;
		}

		void AssignRenderers (Node rootNode)
		{
			new [] { rootNode }.Concat (rootNode.AllChildren ()).ForEach (n => n.Renderer = renderers.OrderByDescending (r => r.Affinity (n)).FirstOrDefault ());
		}
	}
}

