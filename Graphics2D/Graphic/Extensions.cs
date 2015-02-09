using System;
using Xamarin.Forms;

namespace Programmation.Xam.Graphics2D
{
	public static class Extensions
	{
		public static Xamarin.Forms.Point Scale (this Xamarin.Forms.Point point, float factor)
		{
			var scaledPoint = new Xamarin.Forms.Point (point.X * factor, point.Y * factor);
			return scaledPoint;
		}

		public static Xamarin.Forms.Point Scale (this Xamarin.Forms.Point point, float factorX, float factorY)
		{
			var scaledPoint = new Xamarin.Forms.Point (point.X * factorX, point.Y * factorY);
			return scaledPoint;
		}

		public static Xamarin.Forms.Point Offset (this Xamarin.Forms.Point point, float offsetX, float offsetY)
		{
			var offsetPoint = new Xamarin.Forms.Point (point.X + offsetX, point.Y + offsetY);
			return offsetPoint;
		}

		public static Xamarin.Forms.Size Scale (this Xamarin.Forms.Size size, float factor)
		{
			var scaledSize = new Xamarin.Forms.Size (size.Width * factor, size.Height * factor);
			return scaledSize;
		}

		public static Xamarin.Forms.Size Scale (this Xamarin.Forms.Size size, float widthFactor, float heightFactor)
		{
			var scaledSize = new Xamarin.Forms.Size (size.Width * widthFactor, size.Height * heightFactor);
			return scaledSize;
		}

		public static Xamarin.Forms.Rectangle Scale (this Xamarin.Forms.Rectangle rectangle, float factor)
		{
			var scaledTopLeft = new Xamarin.Forms.Point (rectangle.Left, rectangle.Top)
				.Scale (factor);
			var scaledSize = new Xamarin.Forms.Size (rectangle.Right - rectangle.Left, rectangle.Bottom - rectangle.Top)
				.Scale (factor);
			var scaledRectangle = new Xamarin.Forms.Rectangle (scaledTopLeft.X, scaledTopLeft.Y, scaledSize.Width, scaledSize.Height);
			return scaledRectangle;
		}

		public static Xamarin.Forms.Rectangle Scale (this Xamarin.Forms.Rectangle rectangle, float widthFactor, float heightFactor)
		{
			var scaledTopLeft = new Xamarin.Forms.Point (rectangle.Left, rectangle.Top)
				.Scale (widthFactor, heightFactor);
			var scaledSize = new Xamarin.Forms.Size (rectangle.Right - rectangle.Left, rectangle.Bottom - rectangle.Top)
				.Scale (widthFactor, heightFactor);
			var scaledRectangle = new Xamarin.Forms.Rectangle (scaledTopLeft.X, scaledTopLeft.Y, scaledSize.Width, scaledSize.Height);
			return scaledRectangle;
		}

		public static Xamarin.Forms.Rectangle Offset (this Xamarin.Forms.Rectangle rectangle, float offsetX, float offsetY)
		{
			var offsetTopLeft = new Xamarin.Forms.Point (rectangle.Left, rectangle.Top)
				.Offset (offsetX, offsetY);
			var size = new Xamarin.Forms.Size (rectangle.Right - rectangle.Left, rectangle.Bottom - rectangle.Top);
			var offsetRectangle = new Xamarin.Forms.Rectangle (offsetTopLeft, size);
			return offsetRectangle;
		}

		public static bool HasChanged (this Xamarin.Forms.Size size, Xamarin.Forms.Size previousSize)
		{
			if (Math.Abs (previousSize.Width - size.Width) > 0.0001)
				return true;
			if (Math.Abs (previousSize.Height - size.Height) > 0.0001)
				return true;
			return false;
		}

		public static bool HasMoved (this Xamarin.Forms.Point point, Xamarin.Forms.Point previousPoint)
		{
			if (Math.Abs (previousPoint.X - point.X) > 0.0001)
				return true;
			if (Math.Abs (previousPoint.Y - point.Y) > 0.0001)
				return true;
			return false;
		}

	}
}

