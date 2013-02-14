using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Xamarin.Motion;

namespace Xamarin.Canvas
{

	public interface IKeyEvent
	{
		object NativeEvent { get; }
		bool MoveNext { get; }
		bool MovePrev { get; }
	}
}
