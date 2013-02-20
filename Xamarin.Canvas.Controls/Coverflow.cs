using System;
using System.Linq;
using System.Collections.Generic;

using Xamarin.Motion;

namespace Xamarin.Canvas.Controls
{
	public class CoverflowItem : ImageNode
	{
		public string Name { get; set; }

		public CoverflowItem (string file) : base (file) 
		{
		}
	}

	public class Coverflow : GroupNode
	{
		List<CoverflowItem> images;
		LabelNode label;
		double offset;

		public Coverflow ()
		{
			images = new List<CoverflowItem> ();
			label = new LabelNode ();

			Add (label);
		}

		public Coverflow (IEnumerable<string> files) : this ()
		{
			foreach (var file in files) {
				CoverflowItem item = new CoverflowItem (file);
				Add (item);
			}
		}

		public override void Add (Node node)
		{
			if (!(node is CoverflowItem))
				return;
			var cover = node as CoverflowItem;
			images.Add (cover);
			cover.ActivatedEvent += (sender, e) => AnimateTo (cover);
			base.Add (cover);
		}

		protected override void OnChildPreferedSizeChanged (object sender, EventArgs e)
		{
		}

		protected override void OnSizeAllocated (double width, double height)
		{
			LayoutChildren (offset);
		}

		void AnimateTo (CoverflowItem item)
		{
			int target = images.IndexOf (item);
			this.Animate ("Position", f => { offset = f; LayoutChildren (offset); }, (float)offset, (float)target, length: 500, easing: Easing.CubicOut);
		}

		void LayoutChildren (double offset)
		{
			int imagesize = (int)Math.Min (Height, Width / 1.5) / 2;
			double position = -offset;
			foreach (var image in images) {
				LayoutChildForPosition (image, imagesize, position);
				position += 1;
			}

			List<Node> newOrder = Children
				.Where (n => n.RotationY <= 0)
				.OrderBy (n => n.RotationY)
					.Concat (Children.Where (n => n.RotationY > 0).OrderByDescending (n => n.RotationY)).ToList ();
			SortChildren (newOrder);
		}

		void LayoutChildForPosition (CoverflowItem item, int size, double position)
		{
			var rotation = -Math.Atan (position * 2) / (Math.PI / 2);

			Point center = new Point (Width / 2, Height / 2);
			center.X += size * (1.3 - Math.Abs (rotation * rotation)) * position;

			item.SetSize (size, size);
			item.X = center.X - size / 2;
			item.Y = center.Y - size / 2;
			item.RotationY = rotation * 75;
			item.QueueDraw ();
		}
	}
}

