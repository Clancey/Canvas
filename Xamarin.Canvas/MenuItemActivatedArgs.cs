using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Xamarin.Motion;

namespace Xamarin.Canvas
{
	
	public class MenuItemActivatedArgs : EventArgs
	{
		public MenuEntry Entry { get; private set; }
		
		public MenuItemActivatedArgs (MenuEntry entry)
		{
			Entry = entry;
		}
		
	}
	
}
