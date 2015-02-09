using System;

namespace Programmation.Xam.Graphics2D
{
	public class GraphicSource
	{
		public Xamarin.Forms.Size NativeSize { get; private set; }

		public Action<Graphic> SourceAction { get; private set; }

		public GraphicSource (Xamarin.Forms.Size nativeSize, Action<Graphic> action)
		{
			NativeSize = nativeSize;
			SourceAction = action;
		}
	}
}

