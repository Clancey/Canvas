using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Gtk;

namespace Xamarin.Canvas.Gtk
{
	public class GdkKeyEvent : IKeyEvent
	{
		public object NativeEvent { get; private set; }
		public bool MoveNext { get; private set; }
		public bool MovePrev { get; private set; }

		public GdkKeyEvent (Gdk.EventKey evnt)
		{
			NativeEvent = evnt;
			MoveNext = evnt.Key == Gdk.Key.Tab;
			MovePrev = evnt.Key == Gdk.Key.ISO_Left_Tab;
		}
	}

	public class GlibSyncInvoke : ISynchronizeInvoke
	{
		#region ISynchronizeInvoke implementation
		public IAsyncResult BeginInvoke (Delegate method, object[] args)
		{
			GLib.Timeout.Add (0, () => { 
				method.DynamicInvoke (args);
				return false;
			});
			return null;
		}

		public object EndInvoke (IAsyncResult result)
		{
			return null;
		}

		public object Invoke (Delegate method, object[] args)
		{
			GLib.Timeout.Add (0, () => { 
				method.DynamicInvoke (args);
				return false;
			});
			return null;
		}

		public bool InvokeRequired { get { return true; } }
		#endregion
	}

	public class Canvas : EventBox, ICanvas
	{
		Cairo.Engine engine;
		RootNode root;
		
		Node hoveredNode;
		Node HoveredNode {
			get {
				return hoveredNode;
			}
			set {
				if (value == hoveredNode)
					return;
				
				if (MouseGrabNode != null)
					return;
				
				var old = hoveredNode;
				hoveredNode = value;
				
				if (old != null)
					old.MouseOut ();
				
				
				if (hoveredNode != null)
					hoveredNode.MouseIn ();
			}
		}
		
		Gdk.Point DragOffset { get; set; }
		Gdk.Point DragStart { get; set; }
		bool dragging;
		
		Node mouseGrabNode;
		Node MouseGrabNode {
			get {
				return mouseGrabNode;
			}
			set {
				HoveredNode = value;
				mouseGrabNode = value;
			}
		}
		
		Node LastFocusedNode { get; set; }
		
		Node focusedNode;
		Node FocusedNode {
			get {
				return focusedNode;
			}
			set {
				if (value == focusedNode)
					return;
				
				var old = focusedNode;
				focusedNode = value;
				
				if (old != null)
					old.FocusOut ();
				
				if (focusedNode != null)
					focusedNode.FocusIn ();
			}
		}

		public ICanvasEngine Engine {
			get { return engine; }
		}

		public RootNode Root {
			get { return root; }
		}

		public Canvas ()
		{
			Motion.Tweener.Sync = new GlibSyncInvoke ();
			AppPaintable = true;
			VisibleWindow = false;
			CanFocus = true;

			engine = new Cairo.Engine (() => Gdk.CairoHelper.Create (GdkWindow), () => PangoContext);
			root = new RootNode ();
			root.Canvas = this;

			AddEvents ((int)(Gdk.EventMask.AllEventsMask));

			root.RedrawNeeded += (object sender, EventArgs e) => {
				QueueDraw ();
			};
		}

		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);

			root.SetSize (allocation.Width, allocation.Height);
		}

		public void SetCursor (CursorType type)
		{
			switch (type) {
			case CursorType.Normal:
				GdkWindow.Cursor = null;
				break;
			case CursorType.Hand:
				GdkWindow.Cursor = new Gdk.Cursor (Gdk.CursorType.Hand1);
				break;
			case CursorType.IBeam:
				GdkWindow.Cursor = new Gdk.Cursor (Gdk.CursorType.Xterm);
				break;
			}
		}

		public void FocusNode (Node node)
		{
			if (node.CanFocus)
				FocusedNode = node;
		}

		Gdk.Point menuPosition;
		public void ShowMenu (Node node, int rootX, int rootY, uint button)
		{
			if (node.MenuItems == null || !node.MenuItems.Any ())
				return;
			Menu menu = new Menu ();
			
			foreach (var item in node.MenuItems) {
				MenuItem menuItem = new MenuItem (item.Name);
				var tmp = item;
				menuItem.Activated += (sender, e) => {
					node.OnMenuEntryActivated (tmp);
				};
				menu.Append (menuItem);
				menuItem.Show ();
			}
			
			
			menuPosition = new Gdk.Point (rootX, rootY);
			menu.Popup (null, null, PositionMenu, button, Global.CurrentEventTime);
		}
		
		void PositionMenu (Menu menu, out int x, out int y, out bool pushIn)
		{
			x = menuPosition.X;
			y = menuPosition.Y;
			pushIn = false;	
		}

		protected override bool OnExposeEvent (Gdk.EventExpose evnt)
		{
			engine.RenderScene (Root);
			return true;
		}

		protected override bool OnMotionNotifyEvent (Gdk.EventMotion evnt)
		{
			double dx = evnt.X;
			double dy = evnt.Y;
			if (MouseGrabNode != null) {
				if (MouseGrabNode.Draggable && evnt.State.HasFlag (Gdk.ModifierType.Button1Mask)) {
					if (!dragging && (Math.Abs (DragStart.X - dx) > 5 || Math.Abs (DragStart.Y - dy) > 5))
						dragging = true;
					
					if (dragging) {
						var point = engine.TransformPoint (MouseGrabNode.Parent, dx, dy);
						MouseGrabNode.X = point.X - DragOffset.X;
						MouseGrabNode.Y = point.Y - DragOffset.Y;
						QueueDraw ();
					}
				} else {
					var point = engine.TransformPoint (MouseGrabNode, evnt.X, evnt.Y);
					MouseGrabNode.MouseMotion (point.X, point.Y, (ModifierType)evnt.State);
				}
			} else {
				HoveredNode = engine.InputNodeAt (root, evnt.X, evnt.Y);
				if (HoveredNode != null) {
					var point = engine.TransformPoint (HoveredNode, evnt.X, evnt.Y);
					HoveredNode.MouseMotion (point.X, point.Y, (ModifierType)evnt.State);
				}
			}
			
			return base.OnMotionNotifyEvent (evnt);
		}
		
		protected override bool OnEnterNotifyEvent (Gdk.EventCrossing evnt)
		{
			return base.OnEnterNotifyEvent (evnt);
		}
		
		protected override bool OnLeaveNotifyEvent (Gdk.EventCrossing evnt)
		{
			HoveredNode = null;
			return base.OnLeaveNotifyEvent (evnt);
		}

		protected override bool OnButtonPressEvent (Gdk.EventButton evnt)
		{
			int x = (int)evnt.X;
			int y = (int)evnt.Y;
			var element = engine.InputNodeAt (root, x, y); 
			
			HasFocus = true;
			
			MouseGrabNode = element;
			if (MouseGrabNode != null) {
				MouseGrabNode.CancelAnimations ();
				DragStart = new Gdk.Point (x, y);
				
				double dx = x;
				double dy = y;
				var point = engine.TransformPoint (MouseGrabNode.Parent, dx, dy);
				DragOffset = new Gdk.Point ((int) (point.X - MouseGrabNode.X), (int) (point.Y - MouseGrabNode.Y));
				
				var transformedPoint = engine.TransformPoint (MouseGrabNode, x, y);
				MouseGrabNode.ButtonPress (new ButtonEventArgs (transformedPoint.X, transformedPoint.Y, evnt.XRoot, evnt.YRoot, evnt.Button, (ModifierType)evnt.State));
			}
			
			return true;
		}
		
		protected override bool OnButtonReleaseEvent (Gdk.EventButton evnt)
		{
			int x = (int)evnt.X;
			int y = (int)evnt.Y;
			
			if (MouseGrabNode != null) {
				var element = engine.InputNodeAt (root, x, y);
				var point = engine.TransformPoint (MouseGrabNode, x, y);
				MouseGrabNode.ButtonRelease (new ButtonEventArgs (point.X, point.Y, evnt.XRoot, evnt.YRoot, evnt.Button, (ModifierType)evnt.State));
				
				if (element == MouseGrabNode && !dragging) {
					element.Activated ();
					if (element.CanFocus)
						FocusedNode = element;
				}
			}
			
			dragging = false;
			MouseGrabNode = null;
			return true;
		}
		
		protected override bool OnGrabBrokenEvent (Gdk.EventGrabBroken evnt)
		{
			if (MouseGrabNode != null) {
				MouseGrabNode.GrabBroken ();
				MouseGrabNode = null;
			}
			return base.OnGrabBrokenEvent (evnt);
		}
		
		protected override bool OnFocusInEvent (Gdk.EventFocus evnt)
		{
			if (LastFocusedNode != null)
				FocusedNode = LastFocusedNode;
			return base.OnFocusInEvent (evnt);
		}
		
		protected override bool OnFocusOutEvent (Gdk.EventFocus evnt)
		{
			if (FocusedNode != null)
				LastFocusedNode = FocusedNode;
			FocusedNode = null;
			return base.OnFocusOutEvent (evnt);
		}
		
		protected override bool OnKeyPressEvent (Gdk.EventKey evnt)
		{
			if (FocusedNode != null)
				FocusedNode.KeyPress (new GdkKeyEvent (evnt));
			return true;
		}
		
		protected override bool OnKeyReleaseEvent (Gdk.EventKey evnt)
		{
			if (FocusedNode != null)
				FocusedNode.KeyRelease (new GdkKeyEvent (evnt));
			return true;
		}
	}
}

