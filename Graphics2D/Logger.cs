using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace Programmation.Xam.Graphics2D
{
	public class Logger
	{
		public Logger ()
		{
		}

		public static void WriteLine (params object[] args)
		{
			var formattedString = "";
			if (args.Length > 0) {
				var format = args [0] as string;
				var rest = new List<object> ();
				var index = 1;
				while (index < args.Length) {
					rest.Add (args [index]);
					++index;
				}
				formattedString = string.Format (format, rest.ToArray ());
				if (formattedString.StartsWith ("BeerIcon", StringComparison.CurrentCulture)) {
					return;
				}
//				if (formattedString.StartsWith ("Logo", StringComparison.CurrentCulture)) {
//					return;
//				}
			}
			Debug.WriteLine (formattedString);
		}
	}
}

