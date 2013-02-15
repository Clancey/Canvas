using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreAnimation;
using System.ComponentModel;

namespace Xamarin.Canvas.Mac
{

	public class Canvas : NSView, ICanvas, ICanvasEngine
	{
		RootNode root;
		List<object> renderers;
		NSButton testButton;
		UISyncInvoke uisync;

		public bool Supports3D { get { return false; } }

		public Canvas ()
		{
			root = new RootNode ();
			renderers = new List<object> ();
			uisync = new UISyncInvoke ();
			Motion.Tweener.Sync = uisync;

			testButton = new NSButton (new System.Drawing.RectangleF (100, 100, 100, 50));
			testButton.BezelStyle = NSBezelStyle.Rounded;

			testButton.FrameCenterRotation = 40;

			AddSubview (testButton);
		}

		public override void DrawRect (System.Drawing.RectangleF dirtyRect)
		{
			base.DrawRect (dirtyRect);
			var context = NSGraphicsContext.CurrentContext.GraphicsPort;

			context.SetFillColor (new MonoMac.CoreGraphics.CGColor (0, 0, 0));
			context.FillRect (dirtyRect);

		}

		#region ICanvas implementation

		public void SetCursor (CursorType type)
		{

		}

		public void FocusNode (Node node)
		{

		}

		public void ShowMenu (Node node, int rootX, int rootY, uint button)
		{

		}

		public ICanvasEngine Engine {
			get {
				return null;
			}
		}

		public RootNode Root {
			get {
				return root;
			}
		}

		#endregion

		#region ICanvasEngine implementation

		public void RenderScene (Node rootNode)
		{

		}

		public Node InputNodeAt (Node rootNode, double x, double y)
		{
			return null;
		}

		public void SetBackground (Color color)
		{

		}

		public Size TextExtents (string text, TextOptions options)
		{
			return new Size ();
		}

		public Size ImageSize (string file)
		{
			return new Size ();
		}

		#endregion
	}
}

