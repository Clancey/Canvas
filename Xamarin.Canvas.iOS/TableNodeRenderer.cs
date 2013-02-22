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
	internal class TableNodeDelegate : UITableViewDelegate
	{
	}

	internal class TableNodeDataSource : UITableViewDataSource
	{
		TableNode node;

		public TableNodeDataSource (TableNode node)
		{
			this.node = node;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{

		}
	}

	public class TableNodeRenderer : NativeViewRenderer
	{
		TableNode table;
		UITableView view;

		public TableNodeRenderer (TableNode table) : base (table)
		{
			this.table = table;
			this.view = new UITableView (new RectangleF (0, 0, 100, 100), UITableViewStyle.Grouped);
			SetView (view);

			TableNodeDataSource data = new TableNodeDataSource (table);
			view.DataSource = data;
		}
	}
	
}
