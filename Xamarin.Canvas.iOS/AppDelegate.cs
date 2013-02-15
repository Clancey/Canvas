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

	public class ImageStack : GroupNode
	{
		public ImageStack (IEnumerable<string> files)
		{
			foreach (string file in files) {
				var image = new ImageNode (file);
				Add (image);
			}

			SetPreferedSize (100, 100);
		}

		public override void Add (Node node)
		{
			node.ActivatedEvent += HandleChildActivated;
			base.Add (node);
		}

		void HandleChildActivated (object sender, EventArgs e)
		{
			
		}

		protected override void OnSizeAllocated (double width, double height)
		{
			double childSize = (int) (Math.Min (width, height) * 0.7);
			double rotation = 0;
			foreach (var child in Children) {
				AllocateChild (child, childSize, rotation);
				rotation += 20;
			}

		}

		void AllocateChild (Node node, double size, double rotation)
		{
			node.MoveTo ((Width - size) / 2, (Height - size) / 2);
			node.SizeTo (size, size);
			node.RotateTo (rotation);
		}

		protected override void OnChildPreferedSizeChanged (object sender, EventArgs e)
		{
		}
	}

	public class RootViewController : UIViewController
	{
		public override void LoadView ()
		{
			Canvas canvas = new Canvas ();

			BoxNode box = new BoxNode (new Color (1, 0, 0), 100, 100);
			box.X = 100;
			box.Y = 100;
			box.ActivatedEvent += (object sender, EventArgs e) => box.RelRotateTo (500, 10000);
			canvas.Root.Add (box);

			LabelNode label = new LabelNode ("Label Test");
			label.Color = new Color (1, 1, 1);
			label.X = 50;
			label.Y = 50;
			canvas.Root.Add (label);

//			ImageNode image = new ImageNode ("cover1.jpg");
//			canvas.Root.Add (image);

			ImageStack stack = new ImageStack (new [] {
				"cover1.jpg",
				"cover2.jpg",
				"cover3.jpg",
				"cover4.jpg",
				"cover5.jpg",
				"cover6.jpg"
			});
			canvas.Root.Add (stack);


			View = canvas;
		}
	}
}

