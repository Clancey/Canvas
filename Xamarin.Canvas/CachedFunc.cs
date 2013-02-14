using System;
using System.Collections.Generic;

namespace Xamarin.Canvas
{
	public static class CachedFunc
	{
		public static Func<A, B> Make<A, B> (Func<A, B> f)
		{
			var cache = new Dictionary<A, B> ();
			return a => cache.ContainsKey (a) ? cache[a] : cache[a] = f (a);
		}
	
		public static Func<A, B, C> Make<A, B, C> (Func<A, B, C> f)
		{
			var fTupled = Make<Tuple<A, B>, C> (tuple => f (tuple.Item1, tuple.Item2));
			return (a, b) => fTupled (Tuple.Create (a, b));
		}
	
		public static Func<A, B, C, D> Make<A, B, C, D> (Func<A, B, C, D> f)
		{
			var fTupled = Make<Tuple<A, B, C>, D> (tuple => f (tuple.Item1, tuple.Item2, tuple.Item3));
			return (a, b, c) => fTupled (Tuple.Create (a, b, c));
		}
	}
}

