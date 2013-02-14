using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Xamarin.Canvas
{

	public class ButtonNode : GroupNode
	{
		Node child;
		
		public override CursorType Cursor {
			get {
				return Relief ? base.Cursor : CursorType.Hand;
			}
		}
		
		public bool Relief { get; set; }
		public int Rounding { get; set; }
		
		int internalPadding;
		public int InternalPadding {
			get {
				return internalPadding;
			}
			set {
				internalPadding = value;
				UpdateLayout ();
			}
		}
		
		public ButtonNode (Node child) : this ()
		{
			SetChild (child);
		}
		
		protected ButtonNode ()
		{
			internalPadding = 10;
			Relief = true;
			Rounding = 5;
		}
		
		protected void SetChild (Node child)
		{
			this.child = child;
			child.InputTransparent = true;
			Add (child);
			
			UpdateLayout ();
			child.PreferedSizeChanged += (sender, e) => UpdateLayout ();
		}
		
		void UpdateLayout ()
		{
			double preferedWidth = InternalPadding * 2 + child.PreferedWidth;
			double preferedHeight = InternalPadding * 2 + child.PreferedHeight;
			
			SetPreferedSize (preferedWidth, preferedHeight);
		}
		
		protected override void OnSizeAllocated (double width, double height)
		{
			double childWidth = width - 2 * InternalPadding;
			double childHeight = height - 2 * InternalPadding;
			
			if (childWidth < child.PreferedWidth)
				childWidth = Math.Min (child.PreferedWidth, width);
			
			if (childHeight < child.PreferedHeight)
				childHeight = Math.Min (child.PreferedHeight, height);
			
			childWidth = Math.Min (child.PreferedWidth, childWidth);
			childHeight = Math.Min (child.PreferedHeight, childHeight);
			
			child.SetSize (childWidth, childHeight);
			child.X = (width - childWidth) / 2;
			child.Y = (height - childHeight) / 2;
		}
	}
}
