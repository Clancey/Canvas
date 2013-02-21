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
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Timers;

namespace Xamarin.Motion
{
	internal class Ticker
	{
		static Ticker ticker;
		public static Ticker Default {
			get { return ticker ?? (ticker = new Ticker ()); }
		}

		Timer timer;
		List<Tuple<int, Func<bool>>> timeouts;
		int count;

		public Ticker ()
		{
			count = 0;
			timer = new Timer (16);
			timer.Elapsed += HandleElapsed;
			timeouts = new List<Tuple<int, Func<bool>>> ();
		}

		void HandleElapsed (object sender, ElapsedEventArgs e)
		{
			if (timeouts.Count > 0)
				Tweener.Sync.Invoke (new Action (() => SendSignals ()), null);
		}

		void SendSignals ()
		{
			var localCopy = new List<Tuple<int, Func<bool>>> (timeouts);
			foreach (var timeout in localCopy) {
				bool remove = !timeout.Item2 ();
				if (remove)
					timeouts.RemoveAll (t => t.Item1 == timeout.Item1);
			}

		}

		public int Insert (Func<bool> timeout)
		{
			if (!timer.Enabled)
				timer.Enabled = true;
			count++;
			timeouts.Add (new Tuple<int, Func<bool>> (count, timeout));
			return count;
		}

		public void Remove (int handle)
		{
			timeouts.RemoveAll (t => t.Item1 == handle);

			if (!timeouts.Any ())
				timer.Enabled = false;
		}
	}

	public class Tweener
	{
		public static ISynchronizeInvoke Sync { get; set; }

		public uint Length { get; private set; }
		public uint Rate { get; private set; }
		public double Value { get; private set; }
		public long Timestep { get; private set; }
		public Func<double, double> Easing { get; set; }
		public bool Loop { get; set; }
		public string Handle { get; set; }
		
		public bool IsRunning {
			get { return runningTime.IsRunning; }
		}
		
		public event EventHandler ValueUpdated;
		public event EventHandler Finished;
		
		Stopwatch runningTime;
		int timer;
		long lastMilliseconds;
		
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
			if (timer != 0)
				Ticker.Default.Remove (timer);
			timer = 0;
		}
		
		public void Start ()
		{
			Pause ();

			runningTime.Start ();

			lastMilliseconds = 0;
			timer = Ticker.Default.Insert (() => {
				var ms = runningTime.ElapsedMilliseconds;

				double rawValue = Math.Min (1.0f, ms / (double) Length);
				Value = Easing (rawValue);

				Timestep = ms - lastMilliseconds;
				lastMilliseconds = ms;

				if (ValueUpdated != null)
					ValueUpdated (this, EventArgs.Empty);
				
				if (rawValue >= 1.0f)
				{
					if (Loop) {
						Value = 0.0f;
						runningTime.Reset ();
						runningTime.Start ();
						return true;
					}
					
					runningTime.Stop ();
					runningTime.Reset ();
					if (Finished != null)
						Finished (this, EventArgs.Empty);
					Value = 0.0f;
					timer = 0;
					return false;
				}
				return true;
			});
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
			
			if (timer != 0) {
				Ticker.Default.Remove (timer);
				timer = 0;
			}
		}
	}
}
