using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Programmation.Xam.Graphics2D;
using Programmation.Xam.Graphics2D.Droid;
using Android.Graphics;
using Android.Widget;

[assembly:ExportRenderer (typeof(Graphic), typeof(GraphicRenderer))]

namespace Programmation.Xam.Graphics2D.Droid
{
	public class GraphicRenderer
		: ViewRenderer<Graphic, ImageView>
	{
		internal static float Density;

		private float _widthScaleFactor;
		private float _heightScaleFactor;

		private float _offsetX = 0.0f;
		private float _offsetY = 0.0f;

		public GraphicRenderer ()
		{
			Density = Resources.DisplayMetrics.Density;
			_widthScaleFactor = Density;
			_heightScaleFactor = _widthScaleFactor;
		}

		protected override void OnElementChanged (ElementChangedEventArgs<Graphic> e)
		{
			base.OnElementChanged (e);

			if (Element == null || e.OldElement != null) {
				return;
			}

			var native = new Android.Widget.ImageView (Forms.Context);
			SetNativeControl (native);
		}

		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			if (Element == null || Control == null) {
				return;
			}

			var element = Element as Graphic;

			if (e.PropertyName == Graphic.GraphicSourceProperty.PropertyName ||
			    e.PropertyName == Graphic.GraphicSizeProperty.PropertyName ||
			    e.PropertyName == Graphic.BackColorProperty.PropertyName ||
			    e.PropertyName == Graphic.DrawingSizeProperty.PropertyName ||
			    e.PropertyName == Graphic.DrawingOriginProperty.PropertyName ||
			    e.PropertyName == VisualElement.WidthRequestProperty.PropertyName ||
			    e.PropertyName == VisualElement.HeightRequestProperty.PropertyName ||
			    e.PropertyName == VisualElement.WidthProperty.PropertyName ||
			    e.PropertyName == VisualElement.HeightProperty.PropertyName) {
				Logger.WriteLine ("{0} renderer changed {1}", element.GraphicSource, e.PropertyName);
				Control.Invalidate ();
			}
		}

		protected override void OnDraw (Canvas canvas)
		{
			var element = Element as Graphic;

			var view = Control;

			Logger.WriteLine ("Renderer: {0}, WR{2:N}->W{1:N}, HR{4:N}->H{3:N}@[X{5:N}+{7:N},Y{6:N}+{8:N}], [DW{9:N},DH{10:N}]", 
				element.GraphicSource, 
				element.Width, element.WidthRequest, 
				element.Height, element.HeightRequest, 
				element.X, element.Y, 
				element.DrawingOrigin.X, element.DrawingOrigin.Y,
				element.DrawingSize.Width, element.DrawingSize.Height
			);
			if (!element.GraphicSize.IsZero) {
				var width = element.DrawingSize.Width;
				if (Math.Abs (width - (-1)) > 0.1 && Math.Abs (element.GraphicSize.Width - width) > 0.1) {
					_widthScaleFactor = (float)(width / element.GraphicSize.Width);
				} else {
					_widthScaleFactor = 1;
				}
				var height = element.DrawingSize.Height;
				if (Math.Abs (height - (-1)) > 0.1 && Math.Abs (element.GraphicSize.Height - height) > 0.1) {
					_heightScaleFactor = (float)(height / element.GraphicSize.Height);
				} else {
					_heightScaleFactor = 1;
				}
				_offsetX = (float)element.DrawingOrigin.X;
				_offsetY = (float)element.DrawingOrigin.Y;
			}
			_widthScaleFactor *= Density;
			_heightScaleFactor *= Density;

			var backPath = new Path ();
			backPath.MoveTo (view.Left, view.Top);
			backPath.LineTo (view.Right, view.Top);
			backPath.LineTo (view.Right, view.Bottom);
			backPath.LineTo (view.Left, view.Bottom);
			backPath.Close ();
			Logger.WriteLine ("{0} Frame [{1},{2},{3},{4}]", element.GraphicSource, view.Left, view.Top, view.Right, view.Bottom);
			var backColor = element.BackColor.ToAndroid ();
			var paint = new Paint ();
			paint.SetStyle (Paint.Style.Fill);
			paint.Color = backColor;
			canvas.DrawPath (backPath, paint);

			if (element != null) {
				foreach (var command in element.Commands) {
					DrawCommand (command, canvas);
				}
			}
		}

		private Paint _paint = null;
		private Path _nativePath = null;

		private void DrawCommand (GraphicCommand command, Canvas canvas)
		{
			switch (command.Operator) {
			case GraphicOperator.ContextGetCurrentState:
				{
				}
				break;
			case GraphicOperator.ContextSaveState:
				{
					canvas.Save ();
				}
				break;
			case GraphicOperator.ContextRestoreState:
				{
					canvas.Restore ();
				}
				break;
			case GraphicOperator.ContextDrawLinearGradient:
				{
//					var colorGradient = (ColorGradient)command.Parameters [0];
//					var nativeColors = from color in colorGradient.Colors
//					                   select color.ToCGColor ();
//					var nativeLocations = from location in colorGradient.Locations
//					                      select (nfloat)location;
//					var nativeColorSpace = colorGradient.Space is ColorSpaceRGB ? CGColorSpace.CreateDeviceRGB () : CGColorSpace.CreateDeviceGray ();
//					var nativeGradient = new CGGradient (nativeColorSpace, nativeColors.ToArray (), nativeLocations.ToArray ());
//					var start = ((Xamarin.Forms.Point)command.Parameters [1])
//						.Scale (_widthScaleFactor, _heightScaleFactor)
//						.Offset (_offsetX, _offsetY);
//					var end = ((Xamarin.Forms.Point)command.Parameters [2])
//						.Scale (_widthScaleFactor, _heightScaleFactor)
//						.Offset (_offsetX, _offsetY);
//					var option = CGGradientDrawingOptions.DrawsBeforeStartLocation |
//					             CGGradientDrawingOptions.DrawsAfterEndLocation;
//
//					_context.DrawLinearGradient (nativeGradient,
//						new CGPoint ((nfloat)start.X, (nfloat)start.Y),
//						new CGPoint ((nfloat)end.X, (nfloat)end.Y),
//						option);
				}
				break;
			case GraphicOperator.BezierNew:
				{
					var path = (BezierPath)command.Parameters [0];
					_nativePath = new Path ();
					_paint = new Paint ();
					if (path.MiterLimit.HasValue) {
						var effect = new CornerPathEffect (path.MiterLimit.Value);
						_paint.SetPathEffect (effect);
					}
					if (path.UsesEvenOddFillRule.HasValue && path.UsesEvenOddFillRule.Value) {
						_nativePath.SetFillType (Path.FillType.EvenOdd);
					}
				}
				break;
			case GraphicOperator.BezierReset:
				{
					_nativePath.Reset ();
				}
				break;
			case GraphicOperator.BezierMoveTo:
				{
					var start = ((Xamarin.Forms.Point)command.Parameters [0])
						.Scale (_widthScaleFactor, _heightScaleFactor)
						.Offset (_offsetX, _offsetY);
					_nativePath.MoveTo ((float)start.X, (float)start.Y);
				}
				break;
			case GraphicOperator.BezierAddLineTo:
				{
					var end = ((Xamarin.Forms.Point)command.Parameters [0])
						.Scale (_widthScaleFactor, _heightScaleFactor)
						.Offset (_offsetX, _offsetY);
					_nativePath.LineTo ((float)end.X, (float)end.Y);
				}
				break;
			case GraphicOperator.BezierAddCurveToPoint:
				{
					var end = ((Xamarin.Forms.Point)command.Parameters [0])
						.Scale (_widthScaleFactor, _heightScaleFactor)
						.Offset (_offsetX, _offsetY);
					var control1 = ((Xamarin.Forms.Point)command.Parameters [1])
						.Scale (_widthScaleFactor, _heightScaleFactor)
						.Offset (_offsetX, _offsetY);
					var control2 = ((Xamarin.Forms.Point)command.Parameters [2])
						.Scale (_widthScaleFactor, _heightScaleFactor)
						.Offset (_offsetX, _offsetY);
					_nativePath.CubicTo (
						(float)control1.X, (float)control1.Y, 
						(float)control2.X, (float)control2.Y,
						(float)end.X, (float)end.Y
					);
				}
				break;
			case GraphicOperator.BezierAddArc:
				{
					var center = ((Xamarin.Forms.Point)command.Parameters [0])
						.Scale (_widthScaleFactor, _heightScaleFactor)
						.Offset (_offsetX, _offsetY);
					var radius = (float)command.Parameters [1];
					var startAngle = (float)command.Parameters [2];
					var endAngle = (float)command.Parameters [3];
					var clockwise = (bool)command.Parameters [4];
					var oval = new RectF (
						           (float)center.X - radius * _widthScaleFactor, 
						           (float)center.Y - radius * _heightScaleFactor,
						           (float)center.X + radius * _widthScaleFactor, 
						           (float)center.Y + radius * _heightScaleFactor
					           );
					var sweep = endAngle - startAngle;
					_nativePath.AddArc (oval, startAngle, sweep);
				}
				break;
			case GraphicOperator.BezierAddRectangle:
				{
					var rectangle = ((Xamarin.Forms.Rectangle)command.Parameters [0])
						.Scale (_widthScaleFactor, _heightScaleFactor)
						.Offset (_offsetX, _offsetY);
					_nativePath.MoveTo ((float)rectangle.Left, (float)rectangle.Top);
					_nativePath.LineTo ((float)rectangle.Right, (float)rectangle.Top);
					_nativePath.LineTo ((float)rectangle.Right, (float)rectangle.Bottom);
					_nativePath.LineTo ((float)rectangle.Left, (float)rectangle.Bottom);
					_nativePath.Close ();
				}
				break;
			case GraphicOperator.BezierAddOval:
				{
					var rectangle = ((Xamarin.Forms.Rectangle)command.Parameters [0])
						.Scale (_widthScaleFactor, _heightScaleFactor)
						.Offset (_offsetX, _offsetY);
					_nativePath.AddOval (new RectF ((float)rectangle.Left, (float)rectangle.Top, (float)rectangle.Right, (float)rectangle.Bottom), Path.Direction.Cw);
				}
				break;
			case GraphicOperator.BezierAddRoundedRectangle:
				{
					throw new NotImplementedException ();
				}
				break;
			case GraphicOperator.BezierAddClip:
				{
					canvas.ClipPath (_nativePath);
				}
				break;
			case GraphicOperator.BezierAddPath:
				{
					throw new NotImplementedException ();
				}
				break;
			case GraphicOperator.BezierClosePath:
				{
					_nativePath.Close ();
				}
				break;
			case GraphicOperator.BezierFill:
				{
					var color = (Xamarin.Forms.Color)command.Parameters [0];
					var nativeColor = color.ToAndroid ();
					_paint.SetStyle (Paint.Style.Fill);
					_paint.Color = nativeColor;
					canvas.DrawPath (_nativePath, _paint);
				}
				break;
			case GraphicOperator.BezierStroke:
				{
					var lineWidth = (float?)command.Parameters [0];
					var color = (Xamarin.Forms.Color)command.Parameters [1];
					var nativeColor = color.ToAndroid ();
					_paint.SetStyle (Paint.Style.Stroke);
					if (lineWidth.HasValue) {
						_paint.StrokeWidth = lineWidth.Value;
					}
					_paint.Color = nativeColor;
					canvas.DrawPath (_nativePath, _paint);
				}
				break;
			default:
				throw(new NotImplementedException ());
			}
		}

	}
}

