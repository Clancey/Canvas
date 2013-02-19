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
	public static class ColorExtensions
	{
		public static global::Android.Graphics.Color ToAndroid (this Color self)
		{
			return new global::Android.Graphics.Color ((byte)(byte.MaxValue * self.R),
			                                           (byte)(byte.MaxValue * self.G),
			                                           (byte)(byte.MaxValue * self.B),
			                                           (byte)(byte.MaxValue * self.A));
		}
	}

	public class ImageNodeView : NodeView
	{
		ImageNode image;
		ImageView view;

		public ImageNodeView (Context context, ImageNode image)
			: base (context, image)
		{
			this.image = image;
			this.view = new ImageView (context);

			var bitmap = Resources.GetBitmap(image.File);
			view.SetImageBitmap (bitmap);
			AddView (view);
		}

		protected override void OnLayout (bool changed, int l, int t, int r, int b)
		{
			base.OnLayout (changed, l, t, r, b);
			view.Layout (0, 0, r - l, b - t);
		}
	}

	public class BoxNodeView : NodeView
	{
		BoxNode box;

		public BoxNodeView (Context context, BoxNode box)
			: base (context, box)
		{
			this.box = box;
		}

		protected override void UpdateNativeView ()
		{
			base.UpdateNativeView ();
			this.SetBackgroundColor (box.Color.ToAndroid ());
		}
	}

	public class NodeView : ViewGroup
	{
		Node node;

		public NodeView (Context context, Node node) : base (context)
		{
			SetBackgroundColor (new Color (0, 0, 0, 0).ToAndroid ());
			this.node = node;
			node.RedrawNeeded += (sender, e) => {
				if (sender == node)
					UpdateNativeView (); 
			};
			node.SizeChanged += (sender, e) => UpdateNativeView ();

			Click += HandleClick;
		}

		void HandleClick (object sender, EventArgs e)
		{
			node.Tap (new TapEventArgs (1, GestureState.Ended));
		}

		protected virtual void UpdateNativeView ()
		{
			SetX ((float)node.X);
			SetY ((float)node.Y);
			Alpha = (float)node.Opacity;
			Rotation = (float)node.Rotation;
			ScaleX = (float)node.Scale;
			ScaleY = (float)node.Scale;
		}

		public void LayoutByNode ()
		{
			UpdateNativeView ();
			Layout ((int)node.X, (int)node.Y, (int)(node.X + node.Width), (int)(node.Y + node.Height));
		}

		protected override void OnLayout (bool changed, int l, int t, int r, int b)
		{
			for (int i = 0; i < this.ChildCount; i++) {
				NodeView view = GetChildAt (i) as NodeView;
				if (view != null) {
					view.LayoutByNode ();
				}
			}
		}
	}

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

	[Activity (Label = "Xamarin.Canvas.Android", MainLauncher = true)]
	public class Activity1 : Activity, ISynchronizeInvoke
	{
		Canvas canvas;

		protected override void OnCreate (Bundle bundle)
		{
			Xamarin.Motion.Tweener.Sync = this;
			base.OnCreate (bundle);
			canvas = new Canvas (this.BaseContext);
			canvas.SetBackground (new Color (0, 0, 1));

			BoxNode box = new BoxNode (new Color (1, 0, 0), 100, 100);
			box.X = 50;
			box.Y = 50;
			canvas.Root.Add (box);

			ImageNode image = new ImageNode ("cover1.jpg");
			image.X = 150;
			image.Y = 150;
			canvas.Root.Add (image);

			image.ActivatedEvent += (sender, e) => image.RelRotateTo (50);

			// Set our view from the "main" layout resource
			LinearLayout layout = new LinearLayout (BaseContext);
			SetContentView (layout);

			layout.AddView (canvas);

		}

		#region ISynchronizeInvoke implementation
		
		IAsyncResult ISynchronizeInvoke.BeginInvoke (Delegate method, object[] args)
		{
			RunOnUiThread (() => method.DynamicInvoke (args));
			return null;
		}
		
		object ISynchronizeInvoke.EndInvoke (IAsyncResult result)
		{
			return null;
		}
		
		object ISynchronizeInvoke.Invoke (Delegate method, object[] args)
		{
			RunOnUiThread (() => method.DynamicInvoke (args));
			return null;
		}
		
		bool ISynchronizeInvoke.InvokeRequired {
			get { return true; }
		}
		
		#endregion
	}
}


