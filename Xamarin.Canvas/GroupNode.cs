using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Xamarin.Canvas
{
	public sealed class RootNode : GroupNode {}

	public class GroupNode : Node
	{
		List<Node> children;
		ReadOnlyCollection<Node> roChildren;
		
		public override ReadOnlyCollection<Node> Children {
			get {
				return roChildren;
			}
		}
		
		public GroupNode ()
		{
			children = new List<Node> ();
			roChildren = children.AsReadOnly ();
		}
		
		public virtual void Add (Node node)
		{
			children.Add (node);
			node.PreferedSizeChanged += OnChildPreferedSizeChanged;
			node.Parent = this;
			if (Canvas != null)
				node.Canvas = Canvas;
			QueueDraw ();

			node.SetSize (node.PreferedWidth, node.PreferedHeight);
			SendChildAdded (node);
		}
		
		protected virtual void OnChildPreferedSizeChanged (object sender, EventArgs e)
		{
			Node node = sender as Node;
			node.SetSize (node.PreferedWidth, node.PreferedHeight);
		}
		
		public virtual void Remove (Node node)
		{
			if (children.Remove (node)) {
				node.Parent = null;
				node.PreferedSizeChanged -= OnChildPreferedSizeChanged;
				QueueDraw ();

				SendChildRemoved (node);
			}
		}
		
		protected override void OnSizeAllocated (double width, double height)
		{
			foreach (var child in Children) {
				child.SetSize (child.PreferedWidth, child.PreferedHeight);
			}
		}
	}
}
