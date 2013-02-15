using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;
using MonoTouch.CoreAnimation;
using System.ComponentModel;
using MonoTouch.Foundation;

namespace Xamarin.Canvas.iOS
{
	public class Canvas : UIView, ICanvas, ICanvasEngine
	{
		RootNode root;

		public bool Supports3D { get { return true; } }

		public Canvas ()
		{
			Motion.Tweener.Sync = new UISyncInvoke ();
			root = new RootNode ();
			root.ChildAdded += (o, e) => AddChild (o as Node);
			root.ChildRemoved += (o, e) => RemoveChild (o as Node);
			root.Canvas = this;

			AddChild (root);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			root.SetSize (Frame.Width, Frame.Height);
		}

		NodeUIView ViewForNode (Node node)
		{
			if (node is BoxNode)
				return new BoxNodeRenderer (node as BoxNode);
			if (node is LabelNode)
				return new LabelNodeRenderer (node as LabelNode);

			return new NodeUIView (node);
		}

		void AddChild (Node node)
		{
			NodeUIView view = ViewForNode (node);
			node.Renderer = view;

			if (node.Parent != null) {
				NodeUIView parentView = node.Parent.Renderer as NodeUIView;
				parentView.AddSubview (view);
			} else {
				AddSubview (view);
			}
		}

		void RemoveChild (Node node)
		{

		}

		#region ICanvas implementation
		public void SetCursor (CursorType type)
		{
			// no cursors on iOS
		}

		public void FocusNode (Node node)
		{
		}

		public void ShowMenu (Node node, int rootX, int rootY, uint button)
		{
		}

		public ICanvasEngine Engine { get { return this; } }

		public RootNode Root { get { return root; } }
		#endregion

		#region ICanvasEngine implementation
		public void RenderScene (Node rootNode)
		{
			(rootNode.Renderer as UIView).SetNeedsDisplay ();
		}

		public Node InputNodeAt (Node rootNode, double x, double y)
		{
			return null;
		}

		public void SetBackground (Color color)
		{
			BackgroundColor = color.ToUIColor ();
		}

		public Size TextExtents (string text, TextOptions options)
		{
			NSString str = new NSString (text);
			var size = str.StringSize (UIFont.SystemFontOfSize (UIFont.LabelFontSize));
			return new Size (size.Width, size.Height);
		}
		#endregion
	}
}

