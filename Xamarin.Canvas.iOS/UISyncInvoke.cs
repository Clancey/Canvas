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
	public class UISyncInvoke : NSObject, ISynchronizeInvoke
	{
		#region ISynchronizeInvoke implementation
		
		public IAsyncResult BeginInvoke (Delegate method, object[] args)
		{
			InvokeOnMainThread (() => method.DynamicInvoke (args));
			return null;
		}
		
		public object EndInvoke (IAsyncResult result)
		{
			return null;
		}
		
		public object Invoke (Delegate method, object[] args)
		{
			InvokeOnMainThread (() => method.DynamicInvoke (args));
			return null;
		}
		
		public bool InvokeRequired {
			get { return true; }
		}
		
		#endregion
		
	}
	
}
