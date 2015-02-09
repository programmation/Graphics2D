using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace Programmation.Xam.Graphics2D
{
	public enum GraphicViewScalingMode
	{
		PreserveAspect,
		PreserveWidth,
		PreserveHeight,
	}

	public class Graphic
		: Xamarin.Forms.Image
	{
		private static Dictionary<string, GraphicSource> SourceRegister = new Dictionary<string, GraphicSource> ();

		public static void RegisterSource (Xamarin.Forms.Size nativeSize, string key, Action<Graphic> action)
		{
			var builder = new GraphicSource (nativeSize, action);
			SourceRegister.Add (key, builder);
		}

		public static void AutoRegister (Type drawingMethodsType, Type imageType)
		{
			var drawingTypeInfo = drawingMethodsType.GetTypeInfo ();
			var methods = (from method in drawingTypeInfo.DeclaredMethods
			               where method.Name.StartsWith ("Draw")
			               where method.IsPublic
			               where method.IsStatic
			               select method).ToList ();

			var imageTypeInfo = imageType.GetTypeInfo ();
			var fields = imageTypeInfo.DeclaredFields;
			var sizeField = imageTypeInfo.GetDeclaredField ("DefaultSize");
			var size = (Xamarin.Forms.Size)sizeField.GetValue (null);

			foreach (var method in methods) {
				var key = method.Name.Replace ("Draw", "");
				Graphic.RegisterSource (size, key, (v) => method.Invoke (null, new object[] { v }));
			}
		}

		[ThreadStatic]
		public static Color ContextColor;

		[ThreadStatic]
		public static GraphicContext CurrentContext;

		public static GraphicContext GetCurrentContext (Graphic view)
		{
			if (CurrentContext == null) {
				CurrentContext = new GraphicContext ();
			}
			CurrentContext.View = view;
			return CurrentContext.Get ();
		}

		public static readonly Xamarin.Forms.BindableProperty GraphicSizeProperty = 
			Xamarin.Forms.BindableProperty.Create<Graphic, Xamarin.Forms.Size> (v => v.GraphicSize, Xamarin.Forms.Size.Zero);

		public Xamarin.Forms.Size GraphicSize {
			get { 
				return (Xamarin.Forms.Size)GetValue (GraphicSizeProperty);
			}
			set {
				SetValue (GraphicSizeProperty, value);
			}
		}

		public static readonly Xamarin.Forms.BindableProperty GraphicSourceProperty = 
			Xamarin.Forms.BindableProperty.Create<Graphic, string> (v => v.GraphicSource, null);

		public string GraphicSource {
			get { 
				return (string)GetValue (GraphicSourceProperty);
			}
			set {
				SetValue (GraphicSourceProperty, value);
			}
		}

		public static readonly Xamarin.Forms.BindableProperty InitialScaleProperty = 
			Xamarin.Forms.BindableProperty.Create<Graphic, double> (v => v.InitialScale, 1.0);

		public double InitialScale {
			get { 
				return (double)GetValue (InitialScaleProperty);
			}
			set {
				SetValue (InitialScaleProperty, value);
			}
		}

		public static readonly Xamarin.Forms.BindableProperty ScalingModeProperty = 
			Xamarin.Forms.BindableProperty.Create<Graphic, GraphicViewScalingMode> (v => v.ScalingMode, GraphicViewScalingMode.PreserveAspect);

		public GraphicViewScalingMode ScalingMode {
			get { 
				return (GraphicViewScalingMode)GetValue (ScalingModeProperty);
			}
			set {
				SetValue (ScalingModeProperty, value);
			}
		}

		public static readonly Xamarin.Forms.BindableProperty DrawingSizeProperty = 
			Xamarin.Forms.BindableProperty.Create<Graphic, Xamarin.Forms.Size> (v => v.DrawingSize, Xamarin.Forms.Size.Zero);

		public Xamarin.Forms.Size DrawingSize {
			get { 
				return (Xamarin.Forms.Size)GetValue (DrawingSizeProperty);
			}
			set {
				SetValue (DrawingSizeProperty, value);
			}
		}

		public static readonly Xamarin.Forms.BindableProperty DrawingOriginProperty = 
			Xamarin.Forms.BindableProperty.Create<Graphic, Xamarin.Forms.Point> (v => v.DrawingOrigin, Xamarin.Forms.Point.Zero);

		public Xamarin.Forms.Point DrawingOrigin {
			get { 
				return (Xamarin.Forms.Point)GetValue (DrawingOriginProperty);
			}
			set {
				SetValue (DrawingOriginProperty, value);
			}
		}

		public static readonly Xamarin.Forms.BindableProperty BackColorProperty = 
			Xamarin.Forms.BindableProperty.Create<Graphic, Color> (v => v.BackColor, Color.Transparent);

		public Color BackColor {
			get { 
				return (Color)GetValue (BackColorProperty);
			}
			set {
				SetValue (BackColorProperty, value);
			}
		}

		public List<GraphicCommand> Commands { get; private set; }

		public void Reset ()
		{
			GraphicSize = Xamarin.Forms.Size.Zero;
			Commands.Clear ();
			WidthRequest = -1;
			HeightRequest = -1;
		}

		public Graphic ()
		{
			BackgroundColor = Xamarin.Forms.Color.Transparent;
			Commands = new List<GraphicCommand> ();
			GraphicSize = new Xamarin.Forms.Size (-1, -1);
			WidthRequest = -1;
			HeightRequest = -1;
		}

		public Graphic (Xamarin.Forms.Size nativeSize)
			: this ()
		{
			GraphicSize = nativeSize;
			ScaleTo (1);
		}

		public Graphic (Xamarin.Forms.Size nativeSize, Xamarin.Forms.Size requiredSize)
			: this (nativeSize)
		{
			ScaleTo (requiredSize);
		}

		public Graphic (Xamarin.Forms.Size nativeSize, double scale)
			: this (nativeSize)
		{
			ScaleTo (scale);
		}

		public Graphic (Xamarin.Forms.Size nativeSize, double widthScale, double heightScale)
			: this (nativeSize)
		{
			ScaleTo (widthScale, heightScale);
		}

		public Graphic (string key)
			: this ()
		{
			GraphicSource = key;
		}

		public Graphic (string key, Xamarin.Forms.Size requiredSize)
			: this ()
		{
			GraphicSource = key;
			ScaleTo (requiredSize);
		}

		public Graphic (string key, double scale)
			: this ()
		{
			GraphicSource = key;
			ScaleTo (scale);
		}

		public Graphic (string key, double widthScale, double heightScale)
			: this ()
		{
			GraphicSource = key;
			ScaleTo (widthScale, heightScale);
		}

		public static Graphic FromBuilder (Xamarin.Forms.Size nativeSize, Action<Graphic> action)
		{
			var view = new Graphic (nativeSize);
			action (view);
			return view;
		}

		public static Graphic FromBuilder (Xamarin.Forms.Size nativeSize, Xamarin.Forms.Size requiredSize, Action<Graphic> action)
		{
			var view = new Graphic (nativeSize, requiredSize);
			action (view);
			return view;
		}

		public static Graphic FromBuilder (Xamarin.Forms.Size nativeSize, double scale, Action<Graphic> action)
		{
			var view = new Graphic (nativeSize, scale);
			action (view);
			return view;
		}

		public static Graphic FromBuilder (Xamarin.Forms.Size nativeSize, double widthScale, double heightScale, Action<Graphic> action)
		{
			var view = new Graphic (nativeSize, widthScale, heightScale);
			action (view);
			return view;
		}

		public static Graphic FromBuilder (string key)
		{
			var view = new Graphic (key);
			return view;
		}

		public static Graphic FromBuilder (string key, Xamarin.Forms.Size requiredSize)
		{
			var view = new Graphic (key, requiredSize);
			return view;
		}

		public static Graphic FromBuilder (string key, double scale)
		{
			var view = new Graphic (key, scale);
			return view;
		}

		public static Graphic FromBuilder (string key, double widthScale, double heightScale)
		{
			var view = new Graphic (key, widthScale, heightScale);
			return view;
		}

		public void AddCommand (GraphicCommand command)
		{
			Commands.Add (command);
		}

		public GraphicContext GetContext ()
		{
			return CurrentContext.Get ();
		}

		public void AddPath (BezierPath path)
		{
			var command = new GraphicCommand (GraphicOperator.BezierNew);
			command.AddParameter (path);
			Commands.Add (command);
			foreach (var pathCommand in path.Commands) {
				Commands.Add (pathCommand);
			}
		}

		public void ScaleTo (double scale)
		{
			ScaleTo (new Xamarin.Forms.Size (GraphicSize.Width * scale, GraphicSize.Height * scale));
		}

		public void ScaleTo (double widthScale, double heightScale)
		{
			ScaleTo (new Xamarin.Forms.Size (GraphicSize.Width * widthScale, GraphicSize.Height * heightScale));
		}

		public void ScaleTo (Xamarin.Forms.Size newSize)
		{
			WidthRequest = newSize.Width;
			HeightRequest = newSize.Height;
		}

		private Xamarin.Forms.Size _previousDrawingSize = Xamarin.Forms.Size.Zero;
		private Xamarin.Forms.Point _previousDrawingOrigin = Xamarin.Forms.Point.Zero;

		protected override void OnPropertyChanged (string propertyName = null)
		{
			base.OnPropertyChanged (propertyName);

			if (propertyName == null) {
				return;
			}

			if (propertyName == Graphic.GraphicSourceProperty.PropertyName) {
				Logger.WriteLine ("BuilderSource {0}", GraphicSource);
				if (!string.IsNullOrEmpty (GraphicSource)) {
					var builder = SourceRegister [GraphicSource];
					GraphicSize = builder.NativeSize;
					Commands.Clear ();
					builder.SourceAction (this);
				}
				InvalidateMeasure ();
			}
			if (propertyName == Graphic.GraphicSizeProperty.PropertyName) {
				Logger.WriteLine ("{2} GraphicSize [{0:N},{1:N}]", GraphicSize.Width, GraphicSize.Height, GraphicSource);
				InvalidateMeasure ();
			}
			if (propertyName == Graphic.InitialScaleProperty.PropertyName) {
				Logger.WriteLine ("{1} InitialScale {0}", InitialScale, GraphicSource);
				ScaleTo (InitialScale);
			}
			if (propertyName == Graphic.DrawingSizeProperty.PropertyName) {
				Logger.WriteLine ("{2} DrawingSize [{0:N},{1:N}]", DrawingSize.Width, DrawingSize.Height, GraphicSource);
				if (DrawingSize.HasChanged (_previousDrawingSize)) {
					InvalidateMeasure ();
					_previousDrawingSize = DrawingSize;
				}
			}
			if (propertyName == Graphic.DrawingOriginProperty.PropertyName) {
				Logger.WriteLine ("{2} DrawingOrigin [{0:N},{1:N}]", DrawingOrigin.X, DrawingOrigin.Y, GraphicSource);
				if (DrawingOrigin.HasMoved (_previousDrawingOrigin)) {
					InvalidateMeasure ();
					_previousDrawingOrigin = DrawingOrigin;
				}
			}
			if (propertyName == VisualElement.WidthProperty.PropertyName) {
				Logger.WriteLine ("{1} Width {0:N}", Width, GraphicSource);
			}
			if (propertyName == VisualElement.HeightProperty.PropertyName) {
				Logger.WriteLine ("{1} Height {0:N}", Height, GraphicSource);
			}
			if (propertyName == VisualElement.WidthRequestProperty.PropertyName) {
				Logger.WriteLine ("{1} WidthRequest {0:N}", WidthRequest, GraphicSource);
				if (WidthRequest < 0) {
					return;
				}
				if (ScalingMode == GraphicViewScalingMode.PreserveAspect) {
					HeightRequest = WidthRequest / GraphicSize.Width * GraphicSize.Height;
				}
			}
			if (propertyName == VisualElement.HeightRequestProperty.PropertyName) {
				Logger.WriteLine ("{1} HeightRequest {0:N}", HeightRequest, GraphicSource);
				if (HeightRequest < 0) {
					return;
				}
				if (ScalingMode == GraphicViewScalingMode.PreserveAspect) {
					WidthRequest = HeightRequest / GraphicSize.Height * GraphicSize.Width;
				}
			}
			if (propertyName == View.HorizontalOptionsProperty.PropertyName) {
				Logger.WriteLine ("{1} HorizontalOptions {0}", HorizontalOptions, GraphicSource);
			}
			if (propertyName == View.VerticalOptionsProperty.PropertyName) {
				Logger.WriteLine ("{1} VerticalOptions {0}", VerticalOptions, GraphicSource);
			}
		}

		//		private bool _allowMultipleInvalidations = true;
		private bool _invalidated = false;
		private bool _fixBug = true;

		protected override void InvalidateMeasure ()
		{
			if (_invalidated) {
				Logger.WriteLine ("{0} Skip InvalidateMeasure", GraphicSource);
				return;
			}
			Logger.WriteLine ("{0} InvalidateMeasure", GraphicSource);
			base.InvalidateMeasure ();
//			if (!_allowMultipleInvalidations) {
//				_invalidated = true;
//			}
		}

		protected override SizeRequest OnSizeRequest (double widthConstraint, double heightConstraint)
		{
			var size = CalculateDrawingSize (widthConstraint, heightConstraint);
			DrawingSize = size;
			DrawingOrigin = CalculateDrawingOrigin (DrawingSize, new Xamarin.Forms.Size (widthConstraint, heightConstraint));
			Logger.WriteLine ("{0} OnSizeRequest = [WC{1:N},HC{2:N}]->[WRW{7:N},HRH{8:N}]@[TX{9:N},TY{10:N}], WR{3:N}->W{4:N}, HR{5:N}->H{6:N}", 
				GraphicSource, 
				widthConstraint, heightConstraint, 
				WidthRequest, Width, HeightRequest, Height, 
				size.Width, size.Height, 
				DrawingOrigin.X, DrawingOrigin.Y
			);
			return new SizeRequest (size);
		}

		protected override void OnSizeAllocated (double width, double height)
		{
			base.OnSizeAllocated (width, height);
			// BUG: Under some unknown conditions, Height reverts to the original height set on initialisation
			// Force a recalc until Height is the same as HeightRequest
			var bugDetected = false;
			if (_fixBug
			    && Math.Abs (Height - -1) > 0.0001
			    && Math.Abs (HeightRequest - -1) > 0.0001
			    && Math.Abs (Height - HeightRequest) > 0.0001) {
				bugDetected = true;
				InvalidateMeasure ();
			}
			DrawingOrigin = CalculateDrawingOrigin (DrawingSize, new Xamarin.Forms.Size (width, height));
			Logger.WriteLine ("{0} OnSizeAllocated = [WA{1:N},HA{2:N}]@[TX{8:N},TY{9:N}], WR{3:N}->W{4:N}, HR{5:N}->H{6:N}{7}", 
				GraphicSource, 
				width, height, 
				WidthRequest, Width, HeightRequest, Height, 
				bugDetected ? " ***************************" : "", 
				DrawingOrigin.X, DrawingOrigin.Y
			);
			_invalidated = false;
			InvalidateMeasure ();
		}

		private Xamarin.Forms.Size CalculateDrawingSize (double width, double height)
		{
			var renderWidth = width;
			var renderHeight = height;

			if (WidthRequest < 0) {
				if (HorizontalOptions.Expands) {
					renderWidth = width;
				} else {
					// Can't allow render width to be bigger than allocated frame
					// Causes drawing problems and makes the view stretch or shrink when rotated
					renderWidth = Math.Min (width, GraphicSize.Width);
				}
			} else {
				if (HorizontalOptions.Expands) {
					renderWidth = width;
				} else {
					renderWidth = WidthRequest;
				}
			}

			if (HeightRequest < 0) {
				renderHeight = Math.Min (height, CalculateRenderHeight (renderWidth, renderHeight));
			} else {
				if (VerticalOptions.Expands) {
					renderHeight = height;
				} else {
					renderHeight = HeightRequest;
				}
			}

			renderWidth = CalculateRenderWidth (renderHeight, renderWidth);

			return new Xamarin.Forms.Size (renderWidth, renderHeight);
		}

		private double CalculateRenderHeight (double renderWidth, double heightConstraint)
		{
			var renderHeight = heightConstraint;
			if (ScalingMode == GraphicViewScalingMode.PreserveAspect) {
				renderHeight = renderWidth / GraphicSize.Width * GraphicSize.Height;
			}
			return renderHeight;
		}

		private double CalculateRenderWidth (double renderHeight, double widthConstraint)
		{
			var renderWidth = widthConstraint;
			if (ScalingMode == GraphicViewScalingMode.PreserveAspect) {
				renderWidth = renderHeight / GraphicSize.Height * GraphicSize.Width;
			}
			return renderWidth;
		}

		private Xamarin.Forms.Point CalculateDrawingOrigin (Xamarin.Forms.Size renderSize, Xamarin.Forms.Size sizeConstraint)
		{
			var leadingX = 0.0;
			var translateX = leadingX;

			var alignX = HorizontalOptions.Alignment;

			if (alignX == LayoutOptions.Start.Alignment) {
				translateX = leadingX;
			} else if (alignX == LayoutOptions.Center.Alignment) {
				if (renderSize.Width > sizeConstraint.Width) {
					translateX = leadingX + sizeConstraint.Width / 2 - renderSize.Width / 2;
				} else {
					translateX = leadingX;
				} 
			} else if (alignX == LayoutOptions.Fill.Alignment) {
				translateX = leadingX;
			} else if (alignX == LayoutOptions.End.Alignment) {
				translateX = sizeConstraint.Width - renderSize.Width;
			}

			var leadingY = 0.0;
			var translateY = leadingY;

			var alignY = VerticalOptions.Alignment;

			if (alignY == LayoutOptions.Start.Alignment) {
				translateY = leadingY;
			} else if (alignY == LayoutOptions.Center.Alignment) {
				if (renderSize.Height > sizeConstraint.Height) {
					translateY = leadingY + sizeConstraint.Height / 2 - renderSize.Height / 2;
				} else {
					translateY = leadingY;
				}
			} else if (alignY == LayoutOptions.Fill.Alignment) {
				translateY = leadingY;
			} else if (alignY == LayoutOptions.End.Alignment) {
				translateY = sizeConstraint.Height - renderSize.Height;
			}

			Logger.WriteLine ("{0} Origin [{1:N},{2:N}]", GraphicSource, translateX, translateY);
			return new Xamarin.Forms.Point (translateX, translateY);
		}
	}

	public static class ColorExtensions
	{
		public static void SetFill (this Color fillColor)
		{
			Graphic.ContextColor = fillColor;
		}

		public static void SetStroke (this Color strokeColor)
		{
			Graphic.ContextColor = strokeColor;
		}
	}

}

