using System;

namespace Xamarin.Canvas
{
	public enum CursorType
	{
		Normal,
		Hand,
		IBeam,
	}

	public interface ICanvas
	{
		ICanvasEngine Engine { get; }
		RootNode Root { get; }

		void SetCursor (CursorType type);
		void FocusNode (Node node);
		void ShowMenu (Node node, int rootX, int rootY, uint button);

		void Destroy ();
	}
}

