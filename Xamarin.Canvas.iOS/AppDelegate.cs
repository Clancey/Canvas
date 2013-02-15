using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Xamarin.Canvas.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			// If you have defined a root view controller, set it here:
			// window.RootViewController = myViewController;
			
			window.RootViewController = new RootViewController ();
			// make the window visible
			window.MakeKeyAndVisible ();
			return true;
		}
	}

	public class PhotoAlbum : GroupNode
	{
		public PhotoAlbum ()
		{
			ImageStack stack = new ImageStack (new [] {
				"cover1.jpg",
				"cover2.jpg",
				"cover3.jpg",
				"cover4.jpg",
				"cover5.jpg"
			});
			Add (stack);

			stack = new ImageStack (new [] {
				"cover6.jpg",
				"cover7.jpg",
				"cover8.jpg",
				"cover9.jpg",
				"cover10.jpg"
			});
			Add (stack);

			stack = new ImageStack (new [] {
				"cover10.jpg",
				"cover5.jpg",
				"cover3.jpg",
				"cover2.jpg",
				"cover7.jpg"
			});
			Add (stack);

			stack = new ImageStack (new [] {
				"cover10.jpg",
				"cover5.jpg",
				"cover3.jpg",
				"cover2.jpg",
				"cover7.jpg"
			});
			Add (stack);

			stack = new ImageStack (new [] {
				"cover10.jpg",
				"cover5.jpg",
				"cover3.jpg",
				"cover2.jpg",
				"cover1.jpg",
				"cover9.jpg",
				"cover4.jpg",
				"cover8.jpg"
			});
			Add (stack);

			stack = new ImageStack (new [] {
				"cover10.jpg",
				"cover5.jpg",
				"cover3.jpg",
				"cover4.jpg"
			});
			Add (stack);
		}

		protected override void OnSizeAllocated (double width, double height)
		{
			int columns = (int) width / 100;
			int spacing = ((int)width % 100) / (columns + 1);
			
			int col = 0;
			
			int x = spacing;
			int y = spacing;
			
			foreach (var child in Children) {
				var position = new Point (x, y);
				var stack = child as ImageStack;
				stack.HomeLocation = position;
				stack.MoveTo (position.X, position.Y);
				stack.SetSize (stack.PreferedWidth, stack.PreferedHeight);
				
				x += 100 + spacing;
				col++;
				
				if (col >= columns) {
					x = spacing;
					col = 0;
					y += 100 + spacing;
				}
			}
		}
	}

	public class ImageStack : GroupNode
	{
		bool spread;
		bool Spread {
			get {
				return spread;
			}
			set {
				if (spread == value)
					return;
				spread = value;
				UpdatePreferedSize ();
			}
		}

		public Point HomeLocation { get; set; }

		public ImageStack (IEnumerable<string> files)
		{
			foreach (string file in files) {
				var image = new ImageNode (file);
				Add (image);
			}

			UpdatePreferedSize ();
		}

		void UpdatePreferedSize ()
		{
			if (spread) {
				(Parent as PhotoAlbum).RaiseChild (this);
				SetPreferedSize (Parent.Width, Parent.Height);
				MoveTo (0, 0, 350, Motion.Easing.CubicInOut);
			} else {
				SetPreferedSize (100, 100);
				MoveTo (HomeLocation.X, HomeLocation.Y, 350, Motion.Easing.CubicInOut);
			}
		}

		public override void Add (Node node)
		{
			node.ActivatedEvent += HandleChildActivated;
			base.Add (node);
		}

		void HandleChildActivated (object sender, EventArgs e)
		{
			Spread = !Spread;
		}

		protected override void OnSizeAllocated (double width, double height)
		{
			if (spread && width > 100) {
				int columns = (int) width / 100;
				int spacing = ((int)width % 100) / (columns + 1);

				int col = 0;

				int x = spacing;
				int y = spacing;

				foreach (var child in Children) {
					var position = new Point (x, y);
					AllocateChild (child, position, 100, 0);

					x += 100 + spacing;
					col++;

					if (col >= columns) {
						x = spacing;
						col = 0;
						y += 100 + spacing;
					}
				}
			} else {
				double childSize = (int) (Math.Min (width, height) * 0.7);
				double rotation = -50;
				foreach (var child in Children) {
					var position = new Point ((Width - childSize) / 2, (Height - childSize) / 2);
					AllocateChild (child, position, childSize, rotation);
					rotation += 20;
				}
			}
		}

		void AllocateChild (Node node, Point position, double size, double rotation)
		{
			node.MoveTo (position.X, position.Y, 350, Motion.Easing.CubicInOut);
			node.SizeTo (size, size, 350, Motion.Easing.CubicInOut);
			node.RotateTo (rotation, 350, Motion.Easing.CubicInOut);
		}

		protected override void OnChildPreferedSizeChanged (object sender, EventArgs e)
		{
		}
	}

	public class RootViewController : UIViewController
	{
		Canvas canvas;
		PhotoAlbum album;
		public override void LoadView ()
		{
			canvas = new Canvas ();
			System.Threading.ThreadPool.SetMinThreads (50, 50);

			album = new PhotoAlbum ();
			canvas.Root.Add (album);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			View = canvas;
		}

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();

			album.WidthRequest = View.Frame.Width;
			album.HeightRequest = View.Frame.Height;
		}
	}
}

