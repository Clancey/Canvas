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
	internal class Ticker
	{
		Timer timer;
		List<Tuple<int, Func<bool>>> timeouts;
		int count;

		public Ticker ()
		{
			count = 0;
			timer = new Timer (14);
			timer.Elapsed += HandleElapsed;
			timer.Start ();
			timeouts = new List<Tuple<int, Func<bool>>> ();
		}

		void HandleElapsed (object sender, ElapsedEventArgs e)
		{
			Tweener.Sync.Invoke (new Action (() => SendSignals ()), null);
		}

		void SendSignals ()
		{
			List<int> removals = new List<int> ();
			foreach (var timeout in timeouts) {
				bool remove = !timeout.Item2 ();
				if (remove) {
					removals.Add (timeout.Item1);
				}
			}

			foreach (int item in removals)
				timeouts.RemoveAll (t => t.Item1 == item);
		}

		public int Insert (Func<bool> timeout)
		{
			count++;
			timeouts.Add (new Tuple<int, Func<bool>> (count, timeout));
			return count;
		}

		public void Remove (int handle)
		{
			timeouts.RemoveAll (t => t.Item1 == handle);
		}
	}

	public class Tweener
	{
		public static ISynchronizeInvoke Sync { get; set; }

		static Ticker ticker;
		static Ticker Ticker {
			get { return ticker ?? (ticker = new Ticker ()); }
		}

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
		int timer;
		
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
				Ticker.Remove (timer);
			timer = 0;
		}
		
		public void Start ()
		{
			Pause ();

			runningTime.Start ();

			timer = Ticker.Insert (() => {
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
				Ticker.Remove (timer);
				timer = 0;
			}
		}
	}
}
