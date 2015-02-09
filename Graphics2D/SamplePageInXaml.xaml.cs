using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Programmation.Xam.Graphics2D;

namespace Graphics2D
{
	public partial class SamplePageInXaml 
		: ContentPage
	{
		public SamplePageInXaml ()
		{
			InitializeComponent ();
		}

		private void HandleBeerSliderUpdate (object sender, ValueChangedEventArgs e)
		{
			_beerView.ScaleTo (_beerSlider.Value);
//			Logger.WriteLine ("{2} Before ForceLayout: HR{0:N}->H{1:N}", _logoView.HeightRequest, _logoView.Height, _logoView.GraphicSource);
//			_scrollView.ForceLayout ();
//			Logger.WriteLine ("{2} After ForceLayout: HR{0:N}->H{1:N}", _logoView.HeightRequest, _logoView.Height, _logoView.GraphicSource);
		}


		private async void HandleActionButtonClicked (object sender, EventArgs e)
		{
			_actionButton.IsEnabled = false;
			if (_mainView.Children.Contains (_logoView)) {
				await _logoView.FadeTo (0, easing: Easing.CubicOut).ConfigureAwait (true);
				_mainView.Children.Remove (_logoView);
				_actionButton.Text = "Show";
			} else {
				_mainView.Children.Add (_logoView);
				await _logoView.FadeTo (1, easing: Easing.CubicIn).ConfigureAwait (true);
				_actionButton.Text = "Hide";
			}
			_actionButton.IsEnabled = true;
		}

	}
}

