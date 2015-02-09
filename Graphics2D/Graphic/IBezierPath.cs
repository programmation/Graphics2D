using System;
using Xamarin.Forms;

namespace Programmation.Xam.Graphics2D
{
	public interface IBezierPath
	{
		float? MiterLimit { get; set; }

		bool? UsesEvenOddFillRule { get; set; }

		void Reset ();

		void MoveTo (Xamarin.Forms.Point start);

		void AddLineTo (Xamarin.Forms.Point end);

		void AddCurveToPoint (Xamarin.Forms.Point end, Xamarin.Forms.Point control1, Xamarin.Forms.Point control2);

		void AddArc (Xamarin.Forms.Point center, float radius, float startAngle, float endAngle, bool clockwise);

		void AddRectangle (Xamarin.Forms.Rectangle rectangle);

		void AddOval (Xamarin.Forms.Rectangle rectangle);

		void AddRoundedRectangle (Xamarin.Forms.Rectangle rectangle, float cornerRadius);

		void AddClip ();

		void ClosePath ();

		void Fill ();

		void Fill (Xamarin.Forms.Color color);

		void Stroke ();

		void Stroke (Xamarin.Forms.Color color);
	}
}

