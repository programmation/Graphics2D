using System;
using Xamarin.Forms;
using Programmation.Xam.Graphics2D;

namespace Graphics2D
{
	public class TigerPage
		: ContentPage
	{
		public TigerPage ()
		{
			Content = new StackLayout {
				VerticalOptions = LayoutOptions.Center,
				Children = {
					new Graphic {
						GraphicSource = "Tiger",
					},
				}
			};
		}
	}
}

