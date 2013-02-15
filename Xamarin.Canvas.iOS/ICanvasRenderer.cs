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

	public interface ICanvasRenderer
	{
		Node Node { get; }
	}
	
}
