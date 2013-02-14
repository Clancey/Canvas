using System;
using System.Collections.Generic;

namespace Xamarin.Canvas
{
	public interface IRenderer
	{
		/// <summary>
		/// Affinty represents the renderers priority on a specific node. Affinity only gets computed once
		/// or when specially requested by the Canvas, not at every draw. Standard renderers report an
		/// affinity of 1 or -1 for accept or rejecting a node for rendering. Higher affinity numbers get higher
		/// priority. Affinity of 0 is reserved for the null renderer which renders nothing.
		/// </summary>
		int Affinity (Node node);
	}
	
}
