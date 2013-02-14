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
			public Func<float, float> Easing { get; set; }
			public uint Rate { get; set; }
			public uint Length { get; set; }
			public Animatable Owner { get; set; }
			public Action<float> callback;
			public Action<float, bool> finished;
			public Func<bool> repeat;
			public Tweener tweener;
		}
		
		static Dictionary<string, Info> animations;
		
		static AnimationExtensions ()
		{
			animations = new Dictionary<string, Info> ();
		}
		
		public static void Animate (this Animatable self, string name, Animation animation, uint rate = 16, uint length = 250, 
		                            Func<float, float> easing = null, Action<float, bool> finished = null, Func<bool> repeat = null)
		{
			self.Animate (name, animation.GetCallback (), rate, length, easing, finished, repeat);
		}
		
		public static Func<float, float> Interpolate (float start, float end = 1.0f, float reverseVal = 0.0f, bool reverse = false)
		{
			float target = (reverse ? reverseVal : end);
			return x => start + (target - start) * x;
		}
		
		public static void Animate (this Animatable self, string name, Action<float> callback, float start, float end, uint rate = 16, uint length = 250, 
		                            Func<float, float> easing = null, Action<float, bool> finished = null, Func<bool> repeat = null)
		{
			self.Animate<float> (name, Interpolate (start, end), callback, rate, length, easing, finished, repeat);
		}
		
		public static void Animate (this Animatable self, string name, Action<float> callback, uint rate = 16, uint length = 250, 
		                            Func<float, float> easing = null, Action<float, bool> finished = null, Func<bool> repeat = null)
		{
			self.Animate<float> (name, x => x, callback, rate, length, easing, finished, repeat);
		}
		
		public static void Animate<T> (this Animatable self, string name, Func<float, T> transform, Action<T> callback, uint rate = 16, uint length = 250, 
		                               Func<float, float> easing = null, Action<T, bool> finished = null, Func<bool> repeat = null) 
		{
			if (transform == null)
				throw new ArgumentNullException ("transform");
			if (callback == null)
				throw new ArgumentNullException ("callback");
			if (self == null)
				throw new ArgumentNullException ("widget");
			
			self.AbortAnimation (name);
			name += self.GetHashCode ().ToString ();
			
			Action<float> step = f => callback (transform(f));
			Action<float, bool> final = null;
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
			info.repeat = repeat ?? (() => false);
			info.Owner = self;
			
			animations[name] = info;
			tweener.Start ();
			
			info.callback (0.0f);
		}
		
		public static bool AbortAnimation (this Animatable self, string handle)
		{
			handle += self.GetHashCode ().ToString ();
			if (!animations.ContainsKey (handle))
				return false;
			
			Info info = animations[handle];
			info.tweener.ValueUpdated -= HandleTweenerUpdated;
			info.tweener.Finished -= HandleTweenerFinished;
			info.tweener.Stop ();
			
			animations.Remove (handle);
			if (info.finished != null)
				info.finished (1.0f, true);
			return true;
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
			
			bool repeat = info.repeat ();
			
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
