using System;
using System.Collections.Generic;

namespace Programmation.Xam.Graphics2D
{
	public enum GraphicOperator
	{
		BezierNew,
		BezierReset,
		BezierMoveTo,
		BezierAddLineTo,
		BezierAddCurveToPoint,
		BezierAddArc,
		BezierClosePath,
		BezierFill,
		BezierStroke,
		BezierAddRectangle,
		BezierAddOval,
		BezierAddPath,
		BezierAddRoundedRectangle,
		BezierAddClip,
		ContextGetCurrentState,
		ContextSaveState,
		ContextRestoreState,
		ContextDrawLinearGradient,
	}

	public class GraphicCommand
	{
		public GraphicOperator Operator { get; private set; }

		public List<object> Parameters { get; private set; }

		public GraphicCommand ()
		{
			Parameters = new List<object> ();
		}

		public GraphicCommand (GraphicOperator op)
			: this ()
		{
			Operator = op;
		}

		public void AddParameter (object parameter)
		{
			Parameters.Add (parameter);
		}
	}

	public class BezierCommand
		: GraphicCommand
	{
		public static BezierCommand Reset ()
		{
			var command = new BezierCommand (GraphicOperator.BezierReset);
			return command;
		}

		public static BezierCommand MoveTo (Xamarin.Forms.Point start)
		{
			var command = new BezierCommand (GraphicOperator.BezierMoveTo);
			command.AddParameter (start);
			return command;
		}

		public static BezierCommand AddLineTo (Xamarin.Forms.Point end)
		{
			var command = new BezierCommand (GraphicOperator.BezierAddLineTo);
			command.AddParameter (end);
			return command;
		}

		public static BezierCommand AddCurveToPoint (Xamarin.Forms.Point end, Xamarin.Forms.Point control1, Xamarin.Forms.Point control2)
		{
			var command = new BezierCommand (GraphicOperator.BezierAddCurveToPoint);
			command.AddParameter (end);
			command.AddParameter (control1);
			command.AddParameter (control2);
			return command;
		}

		public static BezierCommand AddArc (Xamarin.Forms.Point center, float radius, float startAngle, float endAngle, bool clockwise)
		{
			var command = new BezierCommand (GraphicOperator.BezierAddArc);
			command.AddParameter (center);
			command.AddParameter (radius);
			command.AddParameter (startAngle);
			command.AddParameter (endAngle);
			command.AddParameter (clockwise);
			return command;
		}

		public static BezierCommand AddRectangle (Xamarin.Forms.Rectangle rectangle)
		{
			var command = new BezierCommand (GraphicOperator.BezierAddRectangle);
			command.AddParameter (rectangle);
			return command;
		}

		public static BezierCommand AddOval (Xamarin.Forms.Rectangle rectangle)
		{
			var command = new BezierCommand (GraphicOperator.BezierAddOval);
			command.AddParameter (rectangle);
			return command;
		}

		public static BezierCommand AddRoundedRectangle (Xamarin.Forms.Rectangle rectangle, float cornerRadius)
		{
			var command = new BezierCommand (GraphicOperator.BezierAddRoundedRectangle);
			command.AddParameter (rectangle);
			command.AddParameter (cornerRadius);
			return command;
		}

		public static BezierCommand AddClip ()
		{
			var command = new BezierCommand (GraphicOperator.BezierAddClip);
			return command;
		}

		public static BezierCommand ClosePath ()
		{
			var command = new BezierCommand (GraphicOperator.BezierClosePath);
			return command;
		}

		public static BezierCommand Fill ()
		{
			var command = new BezierCommand (GraphicOperator.BezierFill);
			var color = Graphic.ContextColor;
			command.AddParameter (color);
			return command;
		}

		public static BezierCommand Fill (Xamarin.Forms.Color color)
		{
			var command = new BezierCommand (GraphicOperator.BezierFill);
			command.AddParameter (color);
			return command;
		}

		public static BezierCommand Stroke (float? lineWidth)
		{
			var command = new BezierCommand (GraphicOperator.BezierStroke);
			var color = Graphic.ContextColor;
			command.AddParameter (lineWidth);
			command.AddParameter (color);
			return command;
		}

		public static BezierCommand Stroke (float? lineWidth, Xamarin.Forms.Color color)
		{
			var command = new BezierCommand (GraphicOperator.BezierStroke);
			command.AddParameter (lineWidth);
			command.AddParameter (color);
			return command;
		}

		public BezierCommand (GraphicOperator op)
			: base (op)
		{
		}
	}

	public class ContextCommand
		: GraphicCommand
	{
		public static ContextCommand GetCurrentContext ()
		{
			var command = new ContextCommand (GraphicOperator.ContextGetCurrentState);
			return command;
		}

		public static ContextCommand SaveState ()
		{
			var command = new ContextCommand (GraphicOperator.ContextSaveState);
			return command;
		}

		public static ContextCommand RestoreState ()
		{
			var command = new ContextCommand (GraphicOperator.ContextRestoreState);
			return command;
		}

		public static ContextCommand DrawLinearGradient (
			ColorGradient colorGradient,
			Xamarin.Forms.Point start, 
			Xamarin.Forms.Point end, 
			GradientDrawingOption option)
		{
			var command = new ContextCommand (GraphicOperator.ContextDrawLinearGradient);
			command.AddParameter (colorGradient);
			command.AddParameter (start);
			command.AddParameter (end);
			command.AddParameter (option);
			return command;
		}

		public ContextCommand (GraphicOperator op)
			: base (op)
		{
		}
	}

}

