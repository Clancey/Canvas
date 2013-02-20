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
	public class Canvas : UIView, ICanvas, ICanvasEngine, ISynchronizeInvoke
	{
		RootNode root;

		public bool Supports3D { get { return true; } }

		public Canvas ()
		{
			Motion.Tweener.Sync = this;
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
			// FIXME Loading needs to be configurable by the user
			if (node is BoxNode)
				return new BoxNodeRenderer (node as BoxNode);
			if (node is LabelNode)
				return new LabelNodeRenderer (node as LabelNode);
			if (node is PolygonNode)
				return new PolygonNodeRenderer (node as PolygonNode);
			if (node is ImageNode)
				return new ImageNodeRenderer (node as ImageNode);
			if (node is ButtonNode)
				return new ButtonNodeRenderer (node as ButtonNode);

			return new NodeUIView (node);
		}

		void AddChild (Node node)
		{
			if (node.Renderer == null) {
				NodeUIView view = ViewForNode (node);
				node.Renderer = view;
				
				if (node.Parent != null) {
					NodeUIView parentView = node.Parent.Renderer as NodeUIView;
					parentView.AddSubview (view);
				} else {
					AddSubview (view);
				}
			}

			if (node.Children != null)
				foreach (Node child in node.Children)
					AddChild (child);
		}

		void RemoveChild (Node node)
		{

		}

		#region ICanvas implementation
		public void Destroy ()
		{
			
		}

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

		public Size ImageSize (string file)
		{
			var source = MonoTouch.ImageIO.CGImageSource.FromUrl (new NSUrl (file, false));
			var props = source.GetProperties (0);
			Size result = new Size (props.PixelWidth ?? 0, props.PixelHeight ?? 0);
			return result;
		}
		#endregion

		#region ISynchronizeInvoke implementation
		
		public IAsyncResult BeginInvoke (Delegate method, object[] args)
		{
			InvokeOnMainThread (() => method.DynamicInvoke (args));
			return null;
		}
		
		public object EndInvoke (IAsyncResult result)
		{
			return null;
		}
		
		public object Invoke (Delegate method, object[] args)
		{
			InvokeOnMainThread (() => method.DynamicInvoke (args));
			return null;
		}
		
		public bool InvokeRequired {
			get { return true; }
		}
		
		#endregion
	}
}

