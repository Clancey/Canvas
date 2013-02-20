using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.ComponentModel;

namespace Xamarin.Canvas.Android
{

	public class Canvas : ViewGroup, ICanvas, ICanvasEngine
	{
		RootNode root;

		public Canvas (Context context) : base (context)
		{
			root = new RootNode ();
			root.ChildAdded += (sender, e) => AddChild (sender as Node);
			root.Canvas = this;
			AddChild (root);
		}

		NodeView ViewForNode (Node node)
		{
			if (node is BoxNode)
				return new BoxNodeView (this.Context, node as BoxNode);
			if (node is ImageNode)
				return new ImageNodeView (this.Context, node as ImageNode);
			return new NodeView (this.Context, node);
		}

		void AddChild (Node node)
		{
			if (node.Renderer == null) {
				NodeView view = ViewForNode (node);
				node.Renderer = view;
				
				if (node.Parent != null) {
					NodeView parentView = node.Parent.Renderer as NodeView;
					parentView.AddView (view);
				} else {
					AddView (view);
				}
			}
			
			if (node.Children != null)
				foreach (Node child in node.Children)
					AddChild (child);
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

		public ICanvasEngine Engine { get { return this; } }
		public RootNode Root { get { return root; } }
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
			SetBackgroundColor (color.ToAndroid ());
		}

		public Size TextExtents (string text, TextOptions options)
		{
			var paint = new global::Android.Graphics.Paint ();
			global::Android.Graphics.Rect rect = new global::Android.Graphics.Rect ();
			paint.GetTextBounds (text, 0, text.Length, rect);
			return new Size (rect.Width (), rect.Height ());
		}

		public Size ImageSize (string file)
		{
			var bitmap = Resources.GetBitmap (file);
			return new Size (bitmap.Width, bitmap.Height);
		}

		protected override void OnLayout (bool changed, int l, int t, int r, int b)
		{
			root.SetSize (r - l, b - t);

			for (int i = 0; i < this.ChildCount; i++) {
				View view = GetChildAt (i);
				if (view is NodeView) {
					(view as NodeView).LayoutByNode ();
				}
			}
		}

		public bool Supports3D { get { return true; } }
		#endregion

	}
	
}
