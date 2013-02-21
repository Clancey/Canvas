//
// Tweener.cs
//
// Author:
//       Jason Smith <jason.smith@xamarin.com>
//
// Copyright (c) 2012 Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Xamarin.Motion
{
	
	public static class AnimationExtensions
	{
		class Info
		{
			public Func<double, double> Easing { get; set; }
			public uint Rate { get; set; }
			public uint Length { get; set; }
			public Animatable Owner { get; set; }
			public Action<double> callback;
			public Action<double, bool> finished;
			public Func<bool> repeat;
			public Tweener tweener;
		}
		
		static Dictionary<string, Info> animations;
		static Dictionary<string, int> kinetics;
		
		static AnimationExtensions ()
		{
			animations = new Dictionary<string, Info> ();
			kinetics = new Dictionary<string, int> ();
		}

		public static void AnimateKinetic (this Animatable self, string name, Func<double, double, bool> callback, double velocity, double drag, Action finished = null)
		{
			name += self.GetHashCode ().ToString ();

			System.Diagnostics.Stopwatch sw = new Stopwatch ();
			sw.Start ();

			if (kinetics.ContainsKey (name)) {
				Ticker.Default.Remove (kinetics[name]);
			}

			double sign = velocity / Math.Abs (velocity);
			velocity = Math.Abs (velocity);

			var tick = Ticker.Default.Insert (() => {
				var ms = sw.ElapsedMilliseconds;
				sw.Stop ();
				sw.Reset ();
				sw.Start ();

				velocity -= drag * ms;
				velocity = Math.Max (0, velocity);

				bool result = false;
				if (velocity > 0) {
					result = callback (sign * velocity * ms, velocity);
				}

				if (!result) {
					finished ();
					kinetics.Remove (name);
				}
				return result;
			});

			kinetics [name] = tick;
		}

		public static void Animate (this Animatable self, string name, Animation animation, uint rate = 16, uint length = 250, 
		                            Func<double, double> easing = null, Action<double, bool> finished = null, Func<bool> repeat = null)
		{
			self.Animate (name, animation.GetCallback (), rate, length, easing, finished, repeat);
		}
		
		public static Func<double, double> Interpolate (double start, double end = 1.0f, double reverseVal = 0.0f, bool reverse = false)
		{
			double target = (reverse ? reverseVal : end);
			return x => start + (target - start) * x;
		}
		
		public static void Animate (this Animatable self, string name, Action<double> callback, double start, double end, uint rate = 16, uint length = 250, 
		                            Func<double, double> easing = null, Action<double, bool> finished = null, Func<bool> repeat = null)
		{
			self.Animate<double> (name, Interpolate (start, end), callback, rate, length, easing, finished, repeat);
		}
		
		public static void Animate (this Animatable self, string name, Action<double> callback, uint rate = 16, uint length = 250, 
		                            Func<double, double> easing = null, Action<double, bool> finished = null, Func<bool> repeat = null)
		{
			self.Animate<double> (name, x => x, callback, rate, length, easing, finished, repeat);
		}
		
		public static void Animate<T> (this Animatable self, string name, Func<double, T> transform, Action<T> callback, uint rate = 16, uint length = 250, 
		                               Func<double, double> easing = null, Action<T, bool> finished = null, Func<bool> repeat = null)
		{
			if (transform == null)
				throw new ArgumentNullException ("transform");
			if (callback == null)
				throw new ArgumentNullException ("callback");
			if (self == null)
				throw new ArgumentNullException ("widget");
			
			self.AbortAnimation (name);
			name += self.GetHashCode ().ToString ();
			
			Action<double> step = f => callback (transform(f));
			Action<double, bool> final = null;
			if (finished != null)
				final = (f, b) => finished (transform(f), b);
			
			var info = new Info {
				Rate = rate,
				Length = length,
				Easing = easing ?? Easing.Linear
			};
			
			Tweener tweener = new Tweener (info.Length, info.Rate);
			tweener.Easing = info.Easing;
			tweener.Handle = name;
			tweener.ValueUpdated += HandleTweenerUpdated;
			tweener.Finished += HandleTweenerFinished;
			
			info.tweener = tweener;
			info.callback = step;
			info.finished = final;
			info.repeat = repeat;
			info.Owner = self;
			
			animations[name] = info;
			tweener.Start ();
			
			info.callback (0.0f);
		}
		
		public static bool AbortAnimation (this Animatable self, string handle)
		{
			handle += self.GetHashCode ().ToString ();
			if (animations.ContainsKey (handle)) {
				Info info = animations [handle];
				info.tweener.ValueUpdated -= HandleTweenerUpdated;
				info.tweener.Finished -= HandleTweenerFinished;
				info.tweener.Stop ();
				
				animations.Remove (handle);
				if (info.finished != null)
					info.finished (1.0f, true);
				return true;

			} else if (kinetics.ContainsKey (handle)) {
				Ticker.Default.Remove (kinetics[handle]);
				kinetics.Remove (handle);
			}
			return false;
		}
		
		public static bool AnimationIsRunning (this Animatable self, string handle)
		{
			handle += self.GetHashCode ().ToString ();
			return animations.ContainsKey (handle);
		}
		
		static void HandleTweenerUpdated (object o, EventArgs args)
		{
			Tweener tweener = o as Tweener;
			Info info = animations[tweener.Handle];
			
			info.callback (tweener.Value);
			info.Owner.QueueDraw ();
		}
		
		static void HandleTweenerFinished (object o, EventArgs args)
		{
			Tweener tweener = o as Tweener;
			Info info = animations[tweener.Handle];
			
			bool repeat = false;
			if (info.repeat != null)
				repeat = info.repeat ();
			
			info.callback (tweener.Value);
			
			if (!repeat) {
				animations.Remove (tweener.Handle);
				tweener.ValueUpdated -= HandleTweenerUpdated;
				tweener.Finished -= HandleTweenerFinished;
			}
			
			if (info.finished != null)
				info.finished (tweener.Value, false);
			info.Owner.QueueDraw ();
			
			if (repeat) {
				tweener.Start ();
			}
		}
	}
	
}
