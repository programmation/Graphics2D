using System;
using Xamarin.Forms;

namespace Programmation.Xam.Graphics2D
{
	public class GraphicContext
	{
		public Graphic View { get; set; }

		public GraphicContext ()
		{
		}

		public GraphicContext Get ()
		{
			View.AddCommand (new GraphicCommand (GraphicOperator.ContextGetCurrentState));
			return this;
		}

		public void SaveState ()
		{
			View.AddCommand (new GraphicCommand (GraphicOperator.ContextSaveState));
		}

		public void RestoreState ()
		{
			View.AddCommand (new GraphicCommand (GraphicOperator.ContextRestoreState));
		}

		public void DrawLinearGradient (
			ColorGradient colorGradient, 
			Point start, Point end, 
			GradientDrawingOption drawingOption)
		{
			var command = new GraphicCommand (GraphicOperator.ContextDrawLinearGradient);
			command.AddParameter (colorGradient);
			command.AddParameter (start);
			command.AddParameter (end);
			command.AddParameter (drawingOption);
			View.AddCommand (command);
		}
	}
}

