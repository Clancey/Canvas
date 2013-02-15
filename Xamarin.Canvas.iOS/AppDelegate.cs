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

	public class RootViewController : UIViewController
	{
		public override void LoadView ()
		{
			Canvas canvas = new Canvas ();

			BoxNode box = new BoxNode (new Color (1, 0, 0), 100, 100);
			box.X = 100;
			box.Y = 100;
			box.ActivatedEvent += (object sender, EventArgs e) => box.RelRotateTo (50);
			canvas.Root.Add (box);

			LabelNode label = new LabelNode ("Label Test");
			label.Color = new Color (1, 1, 1);
			label.X = 50;
			label.Y = 50;
			canvas.Root.Add (label);

			View = canvas;
		}
	}
}

