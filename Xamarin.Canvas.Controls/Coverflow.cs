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
		int imagesize;
		Point last;

		double offset;
		double Offset {
			get { return offset; }
			set {
				value = value.Clamp (0, images.Count - 1);
				offset = value;
				LayoutChildren (offset);
			}
		}

		public Coverflow ()
		{
			images = new List<CoverflowItem> ();
			label = new LabelNode ();
			imagesize = 100;

			Add (label);
		}
		
		public Coverflow (IEnumerable<string> files) : this ()
		{
			foreach (var file in files) {
				CoverflowItem item = new CoverflowItem (file);
				Add (item);
			}
		}

		protected override bool OnTouch (Xamarin.Canvas.TouchEvent evnt)
		{
			switch (evnt.Type) {
			case Xamarin.Canvas.TouchType.Down:
				this.AbortAnimation ("KineticScroll");
				last = evnt.Point;
				break;
			case Xamarin.Canvas.TouchType.Move:
				Offset += (last.X - evnt.Point.X) / (imagesize * 0.8);
				last = new Point (evnt.Point.X, evnt.Point.Y);
				break;
			case Xamarin.Canvas.TouchType.Up:
				bool floor = evnt.Velocity.X > 0;
				this.AnimateKinetic ("KineticScroll", (d, v) => {
					Offset -= d / imagesize;
					return v > 1;
				}, evnt.Velocity.X, .025, () => {
					AnimateTo ((int) (floor ? Math.Floor (Offset) : Math.Ceiling (Offset)), 250);
				});
				break;
			}
			return true;
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
			LayoutChildren (Offset);
		}

		void AnimateTo (CoverflowItem item)
		{
			int target = images.IndexOf (item);
			AnimateTo (target, 500);
		}

		void AnimateTo (int index, uint length)
		{
			this.Animate ("Position", f => Offset = f, (float)Offset, (float)index, length: length, easing: Easing.CubicOut);
		}

		void LayoutChildren (double offset)
		{
			imagesize = (int)Math.Min (Height, Width / 1.5) / 2;
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
			item.RotationY = (rotation * 75).Clamp (-72, 72);
			item.X = item.X.Clamp (-size - 10, Width + 10);
			item.QueueDraw ();
		}
	}
}

