using System;

namespace Programmation.Xam.Graphics2D
{
	public class ColorSpace
	{
		public ColorSpace ()
		{
		}

		public static ColorSpace CreateDeviceRGB ()
		{
			return new ColorSpaceRGB ();
		}
	}

	public class ColorSpaceRGB
		: ColorSpace
	{
		public ColorSpaceRGB ()
			: base ()
		{
		}
	}

	public class ColorSpaceCMYK
		: ColorSpace
	{
		public ColorSpaceCMYK ()
			: base ()
		{
		}
	}

	public class ColorSpaceGray
		: ColorSpace
	{
		public ColorSpaceGray ()
			: base ()
		{
		}
	}

}

