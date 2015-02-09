using System;

using Xamarin.Forms;

namespace Graphics2D
{
	public class RootPage 
		: TabbedPage
	{
		public RootPage ()
		{
			if (true) {
				Children.Add (new SamplePageInCode {
					Title = "Code",
				});
			}
			if (true) {
				Children.Add (new SamplePageInXaml {
					Title = "XAML",
				});
			}
			if (true) {
				Children.Add (new TigerPage {
					Title = "Tiger",
				});
			}
		}
	}
}


