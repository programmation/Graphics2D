using System;

using Xamarin.Forms;
using Programmation.Xam.Graphics2D;
using System.Diagnostics;

namespace Graphics2D
{
	public class SamplePageInCode 
		: ContentPage
	{
		private Slider _beerSlider;
		private Graphic _beerView;
		private Button _actionButton;
		private Graphic _logoView;

		private StackLayout _mainView;
		private ScrollView _scrollView;

		private double _startSliderValue = 1.0;

		public SamplePageInCode ()
		{
			_beerSlider = CreateBeerSlider ();
			_beerView = CreateBeerView ();
			_actionButton = CreateActionButton ();
			_logoView = CreateLogoView ();

			var scrolling = true;
			if (scrolling) {
				_scrollView = new ScrollView ();
			}

			_mainView = new StackLayout {
				BackgroundColor = Color.FromRgb (200, 200, 200),
				VerticalOptions = LayoutOptions.Center,
				Padding = new Thickness (0, 20, 0, 0),
				Children = {
					new Label {
						XAlign = TextAlignment.Center,
						Text = "Welcome to Xamarin Forms!",
						TextColor = Color.White,
					},
					_beerSlider,
					_beerView, 
					_actionButton,
					_logoView,
				},
			};

			if (scrolling) {
				_scrollView.Content = _mainView;
				Content = _scrollView;
			} else {
				Content = _mainView;
			}
			Content.BackgroundColor = Color.FromRgb (235, 235, 235);
		}

		private Slider CreateBeerSlider ()
		{
			var slider = new Slider {
				HorizontalOptions = LayoutOptions.Center,
				Minimum = 0.3,
				Maximum = 2.0,
				Value = _startSliderValue,
			};

			slider.ValueChanged += HandleBeerSliderUpdate;

			return slider;
		}

		private void HandleBeerSliderUpdate (object sender, ValueChangedEventArgs e)
		{
//			_mapMarker.ScaleTo (_beerSlider.Value);
			_beerView.ScaleTo (_beerSlider.Value);
//			Logger.WriteLine ("{2} Before ForceLayout: HR{0:N}->H{1:N}", _logoView.HeightRequest, _logoView.Height, _logoView.GraphicSource);
//			_scrollView.ForceLayout ();
//			Logger.WriteLine ("{2} After ForceLayout: HR{0:N}->H{1:N}", _logoView.HeightRequest, _logoView.Height, _logoView.GraphicSource);
		}

		private Graphic CreateBeerView ()
		{
			var view = Graphic.FromBuilder ("BeerIcon", _beerSlider.Value);
			view.HorizontalOptions = LayoutOptions.Center;

			return view;
		}

		private Button CreateActionButton ()
		{
			var button = new Button {
				Text = "Hide",
				TextColor = Color.White,
				Font = Font.SystemFontOfSize (25, FontAttributes.Bold),
			};

			button.Clicked += HandleActionButtonClicked;

			return button;
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
			_scrollView.ForceLayout ();
		}

		private Graphic CreateLogoView ()
		{
			var view = Graphic.FromBuilder ("Logo");
			view.BackColor = Color.White;
			// BUG: If WidthRequest is -1 or > screen width, Height is not preserved after the first ForceLayout() call
			// If WidthRequest <= screen width all works fine
			var showBug = false;
			if (showBug) {
				view.WidthRequest = 500;
				view.HorizontalOptions = LayoutOptions.CenterAndExpand;
				view.ScalingMode = GraphicViewScalingMode.PreserveAspect;
			} else {
//				view.WidthRequest = 200;
				view.HorizontalOptions = LayoutOptions.Center;
			}
//			view.HeightRequest = 85;

			return view;
		}

		protected override SizeRequest OnSizeRequest (double widthConstraint, double heightConstraint)
		{
			var sizeRequest = base.OnSizeRequest (widthConstraint, heightConstraint);
			Logger.WriteLine ("{0} OnSizeRequest = [WC{1:N}, HC{2:N}]->[WRW{7:N},HRH{8:N}], WR{3:N}->W{4:N}, HR{5:N}->H{6:N}", "Page", widthConstraint, heightConstraint, WidthRequest, Width, HeightRequest, Height, sizeRequest.Request.Width, sizeRequest.Request.Height);
			return sizeRequest;
		}

		protected override void OnSizeAllocated (double width, double height)
		{
			Logger.WriteLine ("{0} OnSizeAllocated = [WA{1:N},HA{2:N}], WR{3:N}->W{4:N}, HR{5:N}->H{6:N}", "Page", width, height, WidthRequest, Width, HeightRequest, Height);
			base.OnSizeAllocated (width, height);
		}

	}

}


