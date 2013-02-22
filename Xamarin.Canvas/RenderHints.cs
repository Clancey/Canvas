using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Xamarin.Motion;
using System.ComponentModel;

namespace Xamarin.Canvas
{

	public class RenderHints : INotifyPropertyChanged
	{
		bool shadow;
		public bool Shadow {
			get {
				return shadow;
			}
			set {
				shadow = value;
				OnPropertyChanged ("Shadow");
			}
		}

		bool? antiAlias;
		public bool? AntiAlias {
			get {
				return antiAlias;
			}
			set {
				antiAlias = value;
				OnPropertyChanged ("AntiAlias");
			}
		}

		void OnPropertyChanged (string name)
		{
			if (PropertyChanged != null)
				PropertyChanged (this, new PropertyChangedEventArgs (name));
		}

		#region INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged;
		#endregion
	}
	
}
