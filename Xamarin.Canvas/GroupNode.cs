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
			get { return roChildren; }
		}
		
		public GroupNode ()
		{
			children = new List<Node> ();
			roChildren = children.AsReadOnly ();
		}

		public void SortChildren (List<Node> compare)
		{
			bool cont = false;
			for (int i = 0; i < children.Count; i++) {
				if (compare[i] != children[i]) {
					cont = true;
					break;
				}
			}

			if (!cont)
				return;

			children = children.OrderBy (n => compare.IndexOf (n)).ToList ();
			roChildren = children.AsReadOnly ();

			OnChildrenReordered ();
		}

		public void RaiseChild (Node node)
		{
			if (!children.Contains (node) || children.Last () == node)
				return;
			
			children.Remove (node);
			children.Add (node);
			
			OnChildrenReordered ();
		}

		public void LowerChild (Node node)
		{
			if (!children.Contains (node) || children.First () == node)
				return;
			
			children.Remove (node);
			children.Insert (0, node);
			
			OnChildrenReordered ();
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
				node.PreferedSizeChanged -= OnChildPreferedSizeChanged;
				QueueDraw ();

				SendChildRemoved (node);
				node.Parent = null;
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
