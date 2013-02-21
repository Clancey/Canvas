using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Xamarin.Motion;

namespace Xamarin.Canvas
{
	
	public enum NodeState {
		Prelight = 1,
		Pressed = 1 << 1,
	}

	public enum TouchType
	{
		Down,
		Up,
		Move
	}

	public abstract class TouchEvent
	{
		public abstract Point Point { get; }
		public abstract Vec2 Velocity { get; }
		public abstract TouchType Type { get; }

		public TouchEvent ()
		{
		}
	}

	public class TouchEventArgs : EventArgs
	{
		public TouchEvent Event { get; private set; }
		public bool Result { get; set; }

		public TouchEventArgs (TouchEvent evnt)
		{
			Event = evnt;
		}
	}

	public enum ModifierType
	{
		ShiftMask = 1 << 0, 
		LockMask = 1 << 1, 
		ControlMask = 1 << 2, 
		Mod1Mask = 1 << 3, 
		Mod2Mask = 1 << 4, 
		Mod3Mask = 1 << 5, 
		Mod4Mask = 1 << 6, 
		Mod5Mask = 1 << 7, 
		Button1Mask = 1 << 8, 
		Button2Mask = 1 << 9, 
		Button3Mask = 1 << 10, 
		Button4Mask = 1 << 11, 
		Button5Mask = 1 << 12, 
	}

	public enum GestureState
	{
		Began,
		Update,
		Ended,
		Failed,
		Cancelled,
		Possible
	}

	public class TapEventArgs : EventArgs
	{
		public int NumPresses { get; private set; }
		public GestureState State { get; private set; }

		public TapEventArgs (int presses, GestureState state)
		{
			NumPresses = presses;
			State = state;
		}
	}

	public class ButtonEventArgs : EventArgs
	{
		public double X { get; private set; }
		public double Y { get; private set; }
		public double XRoot { get; private set; }
		public double YRoot { get; private set; }
		public uint Button { get; private set; }
		public ModifierType State { get; private set; }
		
		public ButtonEventArgs (double x, double y, double rootX, double rootY, uint button, ModifierType state) 
		{
			X = x;
			Y = y;
			XRoot = rootX;
			YRoot = rootY;
			Button = button;
			State = state;
		}
	}
	
	public struct MenuEntry
	{
		public string Name;
		public object Data;
		
		public MenuEntry (string name, object data) 
		{
			Name = name;
			Data = data;
		}
	}
	
	public interface IContinuation<T>
	{
		void ContinueWith (Action<T> action);
	}
	
	public class Continuation<T> : IContinuation<T>
	{
		List<Action<T>> actions;
		
		public Continuation ()
		{
			actions = new List<Action<T>> ();
		}
		
		public void ContinueWith (Action<T> action)
		{
			actions.Add (action);
		}
		
		public void Invoke (T val)
		{
			foreach (var action in actions)
				action (val);
		}
	}

	internal class NodeDataStore
	{
		public double X;
		public double Y;
		public double Width;
		public double Height;
		public double Scale;
		public double Rotation;
		public double RotationX;
		public double RotationY;
		public double Opacity;

		public virtual Animation Tween (NodeDataStore target)
		{
			var anim = new Animation ();
			if (X != target.X) anim.Insert (0, 1, new Animation (f => X = f, X, target.X));
			if (Y != target.Y) anim.Insert (0, 1, new Animation (f => Y = f, Y, target.Y));
			if (Width != target.Width) anim.Insert (0, 1, new Animation (f => Width = f, Width, target.Width));
			if (Height != target.Height) anim.Insert (0, 1, new Animation (f => Height = f, Height, target.Height));
			if (Scale != target.Scale) anim.Insert (0, 1, new Animation (f => Scale = f, Scale, target.Scale));
			if (Rotation != target.Rotation) anim.Insert (0, 1, new Animation (f => Rotation = f, Rotation, target.Rotation));
			if (RotationX != target.RotationX) anim.Insert (0, 1, new Animation (f => RotationX = f, RotationX, target.RotationX));
			if (RotationY != target.RotationY) anim.Insert (0, 1, new Animation (f => RotationY = f, RotationY, target.RotationY));
			if (Opacity != target.Opacity) anim.Insert (0, 1, new Animation (f => Opacity = f, Opacity, target.Opacity));
			return anim;
		}
	}
	
	public class Node : IComparable<Node>, Animatable
	{
		public virtual CursorType Cursor { get {
				return CursorType.Normal;
			}
		}
		
		public bool NoChainOpacity { get; set; }
		public bool InputTransparent { get; set; }
		public bool Sensative { get; set; }
		public bool Draggable { get; set; }
		public bool CanFocus { get; set; }
		public bool HasFocus { get; private set; }
		public List<MenuEntry> MenuItems { get; set; }

		public object Renderer { get; set; }
		
		NodeState state;
		public NodeState State {
			get {
				return state;
			}
			private set {
				state = value;
				QueueDraw ();
			}
		}
		
		double x;
		public double X {
			get { return x; }
			set { x = value; }
		}
		
		double y;
		public double Y {
			get { return y; }
			set { y = value; }
		}
		
		double anchorX;
		public double AnchorX {
			get { return anchorX; }
			set { anchorX = value; }
		}
		
		double anchorY;
		public double AnchorY {
			get { return anchorY; }
			set { anchorY = value; }
		}
		
		public double Width { get; private set; }
		public double Height { get; private set; }
		
		double rotation;
		public double Rotation {
			get {
				return rotation;
			}
			set {
				rotation = value;
			}
		}

		double rotationX;
		public double RotationX {
			get {
				return rotationX;
			}
			set {
				rotationX = value;
			}
		}

		double rotationY;
		public double RotationY {
			get {
				return rotationY;
			}
			set {
				rotationY = value;
			}
		}
		
		double scale;
		public double Scale {
			get {
				return scale;
			}
			set {
				scale = value;
			}
		}
		
		public double Depth { get; set; }
		
		double opacity;
		public double Opacity {
			get {
				double result = opacity;
				if (Parent != null && !NoChainOpacity)
					result *= Parent.Opacity;
				
				return result;
			}
			set {
				opacity = Math.Max (0, Math.Min (1, value));
			}
		}
		
		/// <summary>
		/// User set width override for an element
		/// </summary>
		double widthRequest;
		public double WidthRequest {
			get {
				return widthRequest;
			}
			set {
				widthRequest = value;
				if (PreferedSizeChanged != null)
					PreferedSizeChanged (this, EventArgs.Empty);
			}
		}
		
		/// <summary>
		/// User set height override for an element
		/// </summary>
		double heightRequest;
		public double HeightRequest {
			get {
				return heightRequest;
			}
			set {
				heightRequest = value;
				if (PreferedSizeChanged != null)
					PreferedSizeChanged (this, EventArgs.Empty);
			}
		}
		
		/// <summary>
		/// Points prefered allocation accounting for user set width request
		/// </summary>
		double preferedWidth;
		public double PreferedWidth {
			get {
				return WidthRequest == -1 ? preferedWidth : WidthRequest;
			}
			private set {
				preferedWidth = value;
			}
		}
		
		/// <summary>
		/// Nodes prefered allocationsaccounting for user set height request
		/// </summary>
		double preferedHeight;
		public double PreferedHeight {
			get {
				return HeightRequest == -1 ? preferedHeight : HeightRequest;
			}
			private set {
				preferedHeight = value;
			}
		}
		
		public event EventHandler ActivatedEvent;
		public event EventHandler CanvasSet;

		public event EventHandler ChildrenReordered;

		public event EventHandler RedrawNeeded;

		public event EventHandler<MenuItemActivatedArgs> MenuItemActivatedEvent;
		
		public event EventHandler<ButtonEventArgs> ButtonPressEvent;
		public event EventHandler<ButtonEventArgs> ButtonReleaseEvent;

		public event EventHandler<TouchEventArgs> TouchEvent;
		
		public event EventHandler SizeChanged;
		public event EventHandler PreferedSizeChanged;

		public event EventHandler ChildAdded;
		public event EventHandler ChildRemoved;
		
		public virtual ReadOnlyCollection<Node> Children {
			get {
				return null;
			}
		}
		
		public Node Parent { get; set; }
		
		ICanvas canvas;
		public ICanvas Canvas {
			get {
				if (canvas == null && Parent != null)
					return Parent.Canvas;
				return canvas;
			}
			set {
				if (canvas == value)
					return;
				canvas = value;
				if (CanvasSet != null)
					CanvasSet (this, EventArgs.Empty);
				AllChildren ().ForEach (n => n.Canvas = canvas);
			}
		}
		
		public Node ()
		{
			opacity = 1;
			Scale = 1;
			WidthRequest = HeightRequest = -1;
			Width = Height = -1;
			CanFocus = true;
			Sensative = true;

			AnchorX = 0.5;
			AnchorY = 0.5;
		}

		protected void SendChildAdded (Node child)
		{
			if (ChildAdded != null)
				ChildAdded (child, EventArgs.Empty);

			if (Parent != null)
				Parent.SendChildAdded (child);
		}

		protected void SendChildRemoved (Node child)
		{
			if (ChildRemoved != null)
				ChildRemoved (child, EventArgs.Empty);
			
			if (Parent != null)
				Parent.SendChildRemoved (child);
		}
		
		public void MouseIn ()
		{
			if (Canvas != null && Cursor != CursorType.Normal) {
				Canvas.SetCursor (Cursor);
			}
			State |= NodeState.Prelight;
			OnMouseIn ();
		}
		
		public void MouseOut ()
		{
			if (Canvas != null && Cursor != CursorType.Normal) {
				Canvas.SetCursor (CursorType.Normal);
			}
			State &= ~NodeState.Prelight;
			OnMouseOut ();
		}
		
		public void MouseMotion (double x, double y, ModifierType state)
		{
			OnMouseMotion (x, y, state);
		}
		
		public void ButtonPress (ButtonEventArgs args) 
		{
			State |= NodeState.Pressed;
			OnButtonPress (args);
			
			if (ButtonPressEvent != null) {
				ButtonPressEvent (this, args);
			}
		}
		
		public void ButtonRelease (ButtonEventArgs args) 
		{
			State &= ~NodeState.Pressed;
			OnButtonRelease (args);
			
			if (ButtonReleaseEvent != null) {
				ButtonReleaseEvent (this, args);
			}
		}
		
		public void GrabBroken ()
		{
			State &= ~NodeState.Pressed;
		}
		
		public void FocusIn ()
		{
			HasFocus = true;
			OnFocusIn ();
		}
		
		public void FocusOut ()
		{
			HasFocus = false;
			OnFocusOut ();
		}
		
		public void Activated ()
		{
			OnActivated ();
			if (ActivatedEvent != null)
				ActivatedEvent (this, EventArgs.Empty);
		}
		
		protected virtual void MoveChildFocus (bool reverse)
		{
			if (Children == null)
				return;
			
			var children = Children.AsEnumerable ();
			if (reverse) {
				children = children.Reverse ();
			}
			Node next = children.Where (c => c.CanFocus).SkipWhile (c => !c.HasFocus).Skip (1).FirstOrDefault ();
			if (next != null) {
				if (Canvas != null)
					Canvas.FocusNode (next);
			} else {
				if (Parent != null)
					Parent.MoveChildFocus (reverse);
			}
		}
		
		public void KeyPress (IKeyEvent evnt)
		{
			if (evnt.MoveNext) {
				if (Parent != null)
					MoveChildFocus (false);
			} else if (evnt.MovePrev) {
				if (Parent != null)
					MoveChildFocus (true);
			}
			OnKeyPress (evnt);
		}
		
		public void KeyRelease (IKeyEvent evnt)
		{
			OnKeyRelease (evnt);
		}

		public bool Touch (TouchEvent evnt)
		{
			bool result = OnTouch (evnt);

			var args = new TouchEventArgs (evnt);
			if (TouchEvent != null)
				TouchEvent (this, args);

			return args.Result || result;
		}

		public void Tap (TapEventArgs args)
		{
			OnTap (args);

			if (args.NumPresses == 1 && args.State == GestureState.Ended)
				Activated ();
		}

		public void DoubleTap (TapEventArgs args)
		{
			OnDoubleTap (args);
		}
		
		private Point GetPoint(double t, Point p0, Point p1, Point p2, Point p3)
		{
			double cx = 3 * (p1.X - p0.X);
			double cy = 3 * (p1.Y - p0.Y);
			
			double bx = 3 * (p2.X - p1.X) - cx;
			double by = 3 * (p2.Y - p1.Y) - cy;
			
			double ax = p3.X - p0.X - cx - bx;
			double ay = p3.Y - p0.Y - cy - by;
			
			double Cube = t * t * t;
			double Square = t * t;
			
			double resX = (ax * Cube) + (bx * Square) + (cx * t) + p0.X;
			double resY = (ay * Cube) + (by * Square) + (cy * t) + p0.Y;
			
			return new Point(resX, resY);
		}
		
		public IContinuation<bool> CurveTo (double x1, double y1, double x2, double y2, double x3, double y3, uint length = 250, Func<double, double> easing = null)
		{
			if (easing == null)
				easing = Easing.Linear;
			
			Continuation<bool> result = new Continuation<bool> ();
			
			Point start = new Point (X, Y);
			Point p1 = new Point (x1, y1);
			Point p2 = new Point (x2, y2);
			Point end = new Point (x3, y3);
			new Animation (f => {
				var position = GetPoint (f, start, p1, p2, end);
				X = position.X;
				Y = position.Y;
			}, 0, 1, easing)
				.Commit (this, "MoveTo", 16, length, finished: (f, a) => {
					result.Invoke (a);
				});
			
			return result;
		}
		
		public IContinuation<bool> RelMoveTo (double dx, double dy, uint length = 250, Func<double, double> easing = null)
		{
			return MoveTo (X + dx, Y + dy, length, easing);
		}
		
		public IContinuation<bool> RelRotateTo (double drotation, uint length = 250, Func<double, double> easing = null)
		{
			return RotateTo (Rotation + drotation, length, easing);
		}
		
		public IContinuation<bool> RelScaleTo (double dscale, uint length = 250, Func<double, double> easing = null)
		{
			return ScaleTo (Scale + dscale, length, easing);
		}
		
		public IContinuation<bool> MoveTo (double x, double y, uint length = 250, Func<double, double> easing = null)
		{
			if (easing == null)
				easing = Easing.Linear;
			
			Continuation<bool> result = new Continuation<bool> ();
			
			new Animation ()
				.Insert (0, 1, new Animation (f => X = f, X, x, easing))
					.Insert (0, 1, new Animation (f => Y = f, Y, y, easing))
					.Commit (this, "MoveTo", 16, length, finished: (f, a) => {
						result.Invoke (a);
					});
			
			return result;
		}
		
		public IContinuation<bool> RotateTo (double roatation, uint length = 250, Func<double, double> easing = null)
		{
			if (easing == null)
				easing = Easing.Linear;
			
			Continuation<bool> result = new Continuation<bool> ();
			
			new Animation (f => Rotation = f, Rotation, roatation, easing)
				.Commit (this, "RotateTo", 16, length, finished: (f, a) => {
					result.Invoke (a);
				});
			
			return result;
		}
		
		public IContinuation<bool> ScaleTo (double scale, uint length = 250, Func<double, double> easing = null)
		{
			if (easing == null)
				easing = Easing.Linear;
			
			Continuation<bool> result = new Continuation<bool> ();
			
			new Animation (f => Scale = f, Scale, scale, easing)
				.Commit (this, "ScaleTo", 16, length, finished: (f, a) => {
					result.Invoke (a);
				});
			
			return result;
		}
		
		public IContinuation<bool> SizeTo (double width, double height, uint length = 250, Func<double, double> easing = null)
		{
			if (easing == null)
				easing = Easing.Linear;
			
			Continuation<bool> result = new Continuation<bool> ();
			
			var wInterp = AnimationExtensions.Interpolate (Width, width);
			var hInterp = AnimationExtensions.Interpolate (Height, height);
			new Animation ()
				.Insert (0, 1, new Animation (f => SetSize (wInterp (f), hInterp (f)) , 0, 1, easing))
					.Commit (this, "SizeTo", 16, length, finished: (f, a) => {
						result.Invoke (a);
					});
			
			return result;
		}
		
		public IContinuation<bool> FadeTo (double opacity, uint length = 250, Func<double, double> easing = null)
		{
			if (easing == null)
				easing = Easing.Linear;
			
			Continuation<bool> result = new Continuation<bool> ();
			new Animation (f => Opacity = f, Opacity, opacity, easing)
				.Commit (this, "FadeTo", 16, length, finished: (f, a) => {
					result.Invoke (a);
				});
			
			return result;
		}

		public IContinuation<bool> Animate (uint length, Func<double, double> easing, params object[] args)
		{
			if (args.Length % 2 == 1)
				throw new ArgumentException ("Passed number of arguments must be even");

			CancelAnimations ();

			Continuation<bool> result = new Continuation<bool> ();
			var anim = new Animation ();

			for (int i = 0; i < args.Length; i += 2) {
				string name = args[i] as String;

				if (name == null)
					continue;

				switch (name) {
				case "x":
					anim.Insert (0, 1, new Animation (f => X = f, X, (double)args[i+1]));
					break;
				case "y":
					anim.Insert (0, 1, new Animation (f => Y = f, Y, (double)args[i+1]));
					break;
				case "opacity":
					anim.Insert (0, 1, new Animation (f => Opacity = f, Opacity, (double)args[i+1]));
					break;
				case "width":
					anim.Insert (0, 1, new Animation (f => Width = f, Width, (double)args[i+1]));
					break;
				case "height":
					anim.Insert (0, 1, new Animation (f => Height = f, Height, (double)args[i+1]));
					break;
				case "scale":
					anim.Insert (0, 1, new Animation (f => Scale = f, Scale, (double)args[i+1]));
					break;
				case "rotation":
					anim.Insert (0, 1, new Animation (f => Rotation = f, Rotation, (double)args[i+1]));
					break;
				case "rotationx":
					anim.Insert (0, 1, new Animation (f => RotationX = f, RotationX, (double)args[i+1]));
					break;
				case "rotationy":
					anim.Insert (0, 1, new Animation (f => RotationY = f, RotationY, (double)args[i+1]));
					break;
				}
			}

			anim.Commit (this, "CompoundAnimation", 16, length, easing, (f, a) => {
				result.Invoke (a);
			});

			return result;
		}
		
		public void CancelAnimations ()
		{
			this.AbortAnimation ("MoveTo");
			this.AbortAnimation ("RotateTo");
			this.AbortAnimation ("ScaleTo");
			this.AbortAnimation ("FadeTo");
			this.AbortAnimation ("SizeTo");
		}
		
		public void SetSize (double width, double height)
		{
			if (Width == width && Height == height)
				return;

			Width = width;
			Height = height;
			
			SizeAllocated (Width, Height);
			if (SizeChanged != null)
				SizeChanged (this, EventArgs.Empty);
			QueueDraw ();
		}
		
		protected void SetPreferedSize (double width, double height)
		{
			PreferedWidth = width;
			PreferedHeight = height;
			
			if (PreferedSizeChanged != null)
				PreferedSizeChanged (this, EventArgs.Empty);
		}
		
		void SizeAllocated (double width, double height)
		{
			OnSizeAllocated (width, height);
		}
		
		protected virtual void OnMouseIn () {}
		protected virtual void OnMouseOut () {}
		protected virtual void OnMouseMotion (double x, double y, ModifierType state) {}
		protected virtual void OnButtonPress (ButtonEventArgs args) {}
		protected virtual void OnButtonRelease (ButtonEventArgs args) {}
		protected virtual void OnActivated () {}
		protected virtual void OnFocusIn () {}
		protected virtual void OnFocusOut () {}
		protected virtual void OnKeyPress (IKeyEvent evnt) {}
		protected virtual void OnKeyRelease (IKeyEvent evnt) {}
		protected virtual void OnSizeRequested (ref double width, ref double height) {}
		protected virtual void OnSizeAllocated (double width, double height) {}
		protected virtual void OnTap (TapEventArgs args) {}
		protected virtual void OnDoubleTap (TapEventArgs args) {}
		protected virtual bool OnTouch (TouchEvent evnt) { return false; }
		
		public void QueueDraw ()
		{
			if (RedrawNeeded != null)
				RedrawNeeded (this, EventArgs.Empty);

			if (Parent != null)
				Parent.ChildNeedsDraw (this);
		}

		void ChildNeedsDraw (Node child)
		{
			if (RedrawNeeded != null)
				RedrawNeeded (child, EventArgs.Empty);

			if (Parent != null)
				Parent.ChildNeedsDraw (child);
		}

		public void ShowMenu (int rootX, int rootY, uint button)
		{
			Canvas.ShowMenu (this, rootX, rootY, button);
		}

		public void OnMenuEntryActivated (MenuEntry entry)
		{
			if (MenuItemActivatedEvent != null)
				MenuItemActivatedEvent (this, new MenuItemActivatedArgs (entry));
		}

		#region IComparable implementation
		
		public int CompareTo (Node other)
		{
			return Depth.CompareTo (other.Depth);
		}
		
		#endregion

		public IEnumerable<Node> AllChildren ()
		{
			if (Children == null)
				return Enumerable.Empty<Node> ();
			return Children.Concat(Children.SelectMany(n => n.AllChildren()));
		}

		protected void OnChildrenReordered ()
		{
			if (ChildrenReordered != null)
				ChildrenReordered (this, EventArgs.Empty);
		}
	}
	
}
