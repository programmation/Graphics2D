using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Programmation.Xam.Graphics2D
{
	public class BezierPath
		: IBezierPath
	{
		public Graphic View;

		public List<BezierCommand> Commands { get; private set; }

		public float? LineWidth { get; set; }

		public float? MiterLimit { get; set; }

		public bool? UsesEvenOddFillRule { get; set; }

		public BezierPath (Graphic view)
		{
			View = view;
			Commands = new List<BezierCommand> ();
			view.AddPath (this);
		}

		public static BezierPath FromRect (Graphic view, Xamarin.Forms.Rectangle rectangle)
		{
			var path = new BezierPath (view);
			path.AddRectangle (rectangle);
			return path;
		}

		public static BezierPath FromOval (Graphic view, Xamarin.Forms.Rectangle rectangle)
		{
			var path = new BezierPath (view);
			path.AddOval (rectangle);
			return path;
		}

		public static BezierPath FromRoundedRect (Graphic view, Xamarin.Forms.Rectangle rectangle, float cornerRadius)
		{
			var path = new BezierPath (view);
			path.AddRoundedRectangle (rectangle, cornerRadius);
			return path;
		}

		private void AddCommand (BezierCommand command)
		{
			View.AddCommand (command);
		}

		#region IBezierPath implementation

		public void Reset ()
		{
			AddCommand (BezierCommand.Reset ());
		}

		public void MoveTo (Xamarin.Forms.Point start)
		{
			AddCommand (BezierCommand.MoveTo (start));
		}

		public void AddLineTo (Xamarin.Forms.Point end)
		{
			AddCommand (BezierCommand.AddLineTo (end));
		}

		public void AddCurveToPoint (Xamarin.Forms.Point end, Xamarin.Forms.Point control1, Xamarin.Forms.Point control2)
		{
			AddCommand (BezierCommand.AddCurveToPoint (end, control1, control2));
		}

		public void AddArc (Xamarin.Forms.Point center, float radius, float startAngle, float endAngle, bool clockwise)
		{
			AddCommand (BezierCommand.AddArc (center, radius, startAngle, endAngle, clockwise));
		}

		public void AddRectangle (Xamarin.Forms.Rectangle rectangle)
		{
			AddCommand (BezierCommand.AddRectangle (rectangle));
		}

		public void AddOval (Xamarin.Forms.Rectangle rectangle)
		{
			AddCommand (BezierCommand.AddOval (rectangle));
		}

		public void AddRoundedRectangle (Xamarin.Forms.Rectangle rectangle, float cornerRadius)
		{
			AddCommand (BezierCommand.AddRoundedRectangle (rectangle, cornerRadius));
		}

		public void AddClip ()
		{
			AddCommand (BezierCommand.AddClip ());
		}

		public void ClosePath ()
		{
			AddCommand (BezierCommand.ClosePath ());
		}

		public void Fill ()
		{
			AddCommand (BezierCommand.Fill ());
		}

		public void Fill (Xamarin.Forms.Color color)
		{
			AddCommand (BezierCommand.Fill (color));
		}

		public void Stroke ()
		{
			AddCommand (BezierCommand.Stroke (LineWidth));
		}

		public void Stroke (Xamarin.Forms.Color color)
		{
			AddCommand (BezierCommand.Stroke (LineWidth, color));
		}

		#endregion
	}
}

