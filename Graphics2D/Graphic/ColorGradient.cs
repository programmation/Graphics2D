using System;
using Xamarin.Forms;

namespace Programmation.Xam.Graphics2D
{
	public enum GradientDrawingOption
	{
		DrawsBeforeStartLocation = 1,
		DrawsAfterEndLocation = 2,
	}

	public class ColorGradient
	{
		public ColorSpace Space { get; set; }

		public Color[] Colors { get; set; }

		public float[] Locations { get; set; }

		public ColorGradient (ColorSpace space, Color[] colors, float[] locations)
		{
			Space = space;
			Colors = colors;
			Locations = locations;
		}
	}
}

