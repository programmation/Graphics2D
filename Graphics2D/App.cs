using System;
using Xamarin.Forms;
using Programmation.Xam.Graphics2D;

namespace Graphics2D
{
	public class App : Application
	{
		public App ()
		{
			Graphic.RegisterSource (new Size (108, 126), "BeerIcon", ((v) => Samples.DrawBeerIcon (v)));
			Graphic.RegisterSource (new Size (484, 203), "Logo", ((v) => Samples.DrawLogo (v)));
			Graphic.RegisterSource (new Size (540, 540), "Tiger", ((v) => Tiger.DrawTiger (v)));

			// The root page of your application
			MainPage = new RootPage ();
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

