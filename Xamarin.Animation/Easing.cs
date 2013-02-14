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
	
	public static class Easing
	{
		public static readonly Func<float, float> Linear = x => x;
		
		public static readonly Func<float, float> SinOut = x => (float)Math.Sin (x * Math.PI * 0.5f);
		public static readonly Func<float, float> SinIn = x => 1.0f - (float)Math.Cos (x * Math.PI * 0.5f);
		public static readonly Func<float, float> SinInOut = x => -(float)Math.Cos (Math.PI * x) / 2.0f + 0.5f;
		
		public static readonly Func<float, float> CubicIn = x => x * x * x;
		public static readonly Func<float, float> CubicOut = x => (float)Math.Pow (x - 1.0f, 3.0f) + 1.0f;
		public static readonly Func<float, float> CubicInOut = x => x < 0.5f ? (float)Math.Pow (x * 2.0f, 3.0f) / 2.0f :
			(float)(Math.Pow ((x-1)*2.0f, 3.0f) + 2.0f) / 2.0f;
		
		public static readonly Func<float, float> BounceOut;
		public static readonly Func<float, float> BounceIn;
		
		public static readonly Func<float, float> SpringIn = x => x * x * ((1.70158f + 1) * x - 1.70158f);
		public static readonly Func<float, float> SpringOut = x => (x - 1) * (x - 1) * ((1.70158f + 1) * (x - 1) + 1.70158f) + 1;
		
		static Easing ()
		{
			BounceOut = p => {
				if (p < (1 / 2.75f))
				{
					return 7.5625f * p * p;
				}
				else if (p < (2 / 2.75f))
				{
					p -= (1.5f / 2.75f);
					
					return 7.5625f * p * p + .75f;
				}
				else if (p < (2.5f / 2.75f))
				{
					p -= (2.25f / 2.75f);
					
					return 7.5625f * p * p + .9375f;
				}
				else
				{
					p -= (2.625f / 2.75f);
					
					return 7.5625f * p * p + .984375f;
				}
			};
			
			BounceIn = p => 1.0f - BounceOut (p);
		}
	}
	
}
