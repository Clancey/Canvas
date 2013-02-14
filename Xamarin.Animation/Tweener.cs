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
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;

namespace Xamarin.Motion
{
	
	public class Tweener
	{
		public static ISynchronizeInvoke Sync { get; set; }

		public uint Length { get; private set; }
		public uint Rate { get; private set; }
		public float Value { get; private set; }
		public Func<float, float> Easing { get; set; }
		public bool Loop { get; set; }
		public string Handle { get; set; }
		
		public bool IsRunning {
			get { return runningTime.IsRunning; }
		}
		
		public event EventHandler ValueUpdated;
		public event EventHandler Finished;
		
		Stopwatch runningTime;
		Timer timer;
		
		public Tweener (uint length, uint rate)
		{
			Value = 0.0f;
			Length = length;
			Loop = false;
			Rate = rate;
			runningTime = new Stopwatch ();
			Easing = Xamarin.Motion.Easing.Linear;
		}
		
		~Tweener ()
		{
			if (timer != null)
				timer.Stop ();
			timer = null;
		}
		
		public void Start ()
		{
			Pause ();

			runningTime.Start ();
			timer = new Timer (Rate);
			timer.SynchronizingObject = Tweener.Sync;
			timer.Start ();
			timer.Elapsed += (sender, e) => {
				float rawValue = Math.Min (1.0f, runningTime.ElapsedMilliseconds / (float) Length);
				Value = Easing (rawValue);
				if (ValueUpdated != null)
					ValueUpdated (this, EventArgs.Empty);
				
				if (rawValue >= 1.0f)
				{
					if (Loop) {
						Value = 0.0f;
						runningTime.Reset ();
						runningTime.Start ();
						return;
					}
					
					runningTime.Stop ();
					runningTime.Reset ();
					if (Finished != null)
						Finished (this, EventArgs.Empty);
					Value = 0.0f;
					timer.Stop ();
					timer = null;
				}
			};
		}
		
		public void Stop ()
		{
			Pause ();
			runningTime.Reset ();
			Value = 1.0f;
			if (Finished != null)
				Finished (this, EventArgs.Empty);
			Value = 0.0f;
		}
		
		public void Reset ()
		{
			runningTime.Reset ();
			runningTime.Start ();
		}
		
		public void Pause ()
		{
			runningTime.Stop ();
			
			if (timer != null) {
				timer.Stop ();
				timer = null;
			}
		}
	}
}
