using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Programmation.Xam.Graphics2D;
using Programmation.Xam.Graphics2D.iOS;
using CoreGraphics;
using UIKit;
using System.Diagnostics;
using CoreAnimation;
using Foundation;
using System.Linq;
using CoreLocation;

[assembly:ExportRenderer (typeof(Graphic), typeof(GraphicRenderer))]

namespace Programmation.Xam.Graphics2D.iOS
{
	public class GraphicRenderer
		: ViewRenderer<Graphic, UIImageView>
	{
		private float _widthScaleFactor;
		private float _heightScaleFactor;

		private float _offsetX = 0.0f;
		private float _offsetY = 0.0f;

		public GraphicRenderer ()
		{
			_widthScaleFactor = 1;
			_heightScaleFactor = _widthScaleFactor;
		}

		protected override void OnElementChanged (ElementChangedEventArgs<Graphic> e)
		{
			base.OnElementChanged (e);

			if (Element == null || e.OldElement != null) {
				return;
			}

			var native = new UIImageView (CGRect.Empty);
			SetNativeControl (native);
		}

		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			if (Element == null || Control == null) {
				return;
			}

			var element = Element as Graphic;
			var control = Control;

			if (e.PropertyName == Graphic.GraphicSourceProperty.PropertyName ||
			    e.PropertyName == Graphic.GraphicSizeProperty.PropertyName ||
			    e.PropertyName == Graphic.BackColorProperty.PropertyName ||
			    e.PropertyName == Graphic.DrawingSizeProperty.PropertyName ||
			    e.PropertyName == Graphic.DrawingOriginProperty.PropertyName ||
			    e.PropertyName == VisualElement.WidthRequestProperty.PropertyName ||
			    e.PropertyName == VisualElement.HeightRequestProperty.PropertyName ||
			    e.PropertyName == VisualElement.WidthProperty.PropertyName ||
			    e.PropertyName == VisualElement.HeightProperty.PropertyName) {
				Logger.WriteLine ("{0} renderer changed {1}, F[{2:N},{3:N},{4:N},{5:N}]", 
					element.GraphicSource, e.PropertyName,
					control.Frame.Left, control.Frame.Top, control.Frame.Right, control.Frame.Bottom
				);
				control.SetNeedsLayout ();
				control.SetNeedsDisplay ();
			}
		}

		private void DoDrawing (CGRect rect)
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

			var backPath = UIBezierPath.FromRect (view.Frame);
			Logger.WriteLine ("{0} Frame [{1},{2},{3},{4}]", element.GraphicSource, view.Frame.Left, view.Frame.Top, view.Frame.Right, view.Frame.Bottom);
			var backColor = element.BackColor.ToUIColor ();
			backColor.SetFill ();
			backPath.Fill ();

			if (element != null) {
				_context = null;
				_nativePath = null;
				foreach (var command in element.Commands) {
					DrawCommand (command);
				}
			}
		}

		public override void Draw (CGRect rect)
		{
			DoDrawing (rect);
		}

		private CGContext _context = null;
		private UIBezierPath _nativePath = null;

		private void DrawCommand (GraphicCommand command)
		{
			switch (command.Operator) {
			case GraphicOperator.ContextGetCurrentState:
				{
					_context = UIGraphics.GetCurrentContext ();
				}
				break;
			case GraphicOperator.ContextSaveState:
				{
					_context.SaveState ();
				}
				break;
			case GraphicOperator.ContextRestoreState:
				{
					_context.RestoreState ();
				}
				break;
			case GraphicOperator.ContextDrawLinearGradient:
				{
					var colorGradient = (ColorGradient)command.Parameters [0];
					var nativeColors = from color in colorGradient.Colors
					                   select color.ToCGColor ();
					var nativeLocations = from location in colorGradient.Locations
					                      select (nfloat)location;
					CGColorSpace nativeColorSpace = null;
					if (colorGradient.Space is ColorSpaceRGB) {
						nativeColorSpace = CGColorSpace.CreateDeviceRGB ();
					}
					if (colorGradient.Space is ColorSpaceCMYK) {
						nativeColorSpace = CGColorSpace.CreateDeviceCMYK ();
					}
					if (colorGradient.Space is ColorSpaceGray) {
						nativeColorSpace = CGColorSpace.CreateDeviceGray ();
					}
					var nativeGradient = new CGGradient (nativeColorSpace, nativeColors.ToArray (), nativeLocations.ToArray ());
					var start = ((Xamarin.Forms.Point)command.Parameters [1])
						.Scale (_widthScaleFactor, _heightScaleFactor)
						.Offset (_offsetX, _offsetY);
					var end = ((Xamarin.Forms.Point)command.Parameters [2])
						.Scale (_widthScaleFactor, _heightScaleFactor)
						.Offset (_offsetX, _offsetY);
					var option = CGGradientDrawingOptions.DrawsBeforeStartLocation |
					             CGGradientDrawingOptions.DrawsAfterEndLocation;

					_context.DrawLinearGradient (nativeGradient,
						new CGPoint ((nfloat)start.X, (nfloat)start.Y),
						new CGPoint ((nfloat)end.X, (nfloat)end.Y),
						option);
				}
				break;
			case GraphicOperator.BezierNew:
				{
					var path = (BezierPath)command.Parameters [0];
					_nativePath = new UIBezierPath ();
					if (path.MiterLimit.HasValue) {
						_nativePath.MiterLimit = (nfloat)path.MiterLimit.Value;
					}
					if (path.UsesEvenOddFillRule.HasValue) {
						_nativePath.UsesEvenOddFillRule = path.UsesEvenOddFillRule.Value;
					}
				}
				break;
			case GraphicOperator.BezierReset:
				{
					_nativePath.RemoveAllPoints ();
				}
				break;
			case GraphicOperator.BezierMoveTo:
				{
					var start = ((Xamarin.Forms.Point)command.Parameters [0])
						.Scale (_widthScaleFactor, _heightScaleFactor)
						.Offset (_offsetX, _offsetY);
					_nativePath.MoveTo (new CGPoint ((nfloat)start.X, (nfloat)start.Y));
				}
				break;
			case GraphicOperator.BezierAddLineTo:
				{
					var end = ((Xamarin.Forms.Point)command.Parameters [0])
						.Scale (_widthScaleFactor, _heightScaleFactor)
						.Offset (_offsetX, _offsetY);
					_nativePath.AddLineTo (new CGPoint ((nfloat)end.X, (nfloat)end.Y));
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
					_nativePath.AddCurveToPoint (
						new CGPoint ((nfloat)end.X, (nfloat)end.Y), 
						new CGPoint ((nfloat)control1.X, (nfloat)control1.Y), 
						new CGPoint ((nfloat)control2.X, (nfloat)control2.Y)
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
					_nativePath.AddArc (new CGPoint ((nfloat)center.X, (nfloat)center.Y), 
						radius * _widthScaleFactor, 
						startAngle, endAngle, 
						clockwise
					);
				}
				break;
			case GraphicOperator.BezierAddRectangle:
				{
					var rectangle = ((Xamarin.Forms.Rectangle)command.Parameters [0])
						.Scale (_widthScaleFactor, _heightScaleFactor)
						.Offset (_offsetX, _offsetY);
					_nativePath = UIBezierPath.FromRect (new CGRect ((nfloat)rectangle.Left, (nfloat)rectangle.Top, 
						(nfloat)(rectangle.Right - rectangle.Left),
						(nfloat)(rectangle.Bottom - rectangle.Top)));
				}
				break;
			case GraphicOperator.BezierAddOval:
				{
					var rectangle = ((Xamarin.Forms.Rectangle)command.Parameters [0])
						.Scale (_widthScaleFactor, _heightScaleFactor)
						.Offset (_offsetX, _offsetY);
					_nativePath = UIBezierPath.FromOval (new CGRect ((nfloat)rectangle.Left, (nfloat)rectangle.Top, 
						(nfloat)(rectangle.Right - rectangle.Left),
						(nfloat)(rectangle.Bottom - rectangle.Top)));
				}
				break;
			case GraphicOperator.BezierAddRoundedRectangle:
				{
					var rectangle = ((Xamarin.Forms.Rectangle)command.Parameters [0])
						.Scale (_widthScaleFactor, _heightScaleFactor)
						.Offset (_offsetX, _offsetY);
					var cornerRadius = (float)command.Parameters [1]
					                   * _widthScaleFactor;
					_nativePath = UIBezierPath.FromRoundedRect (new CGRect ((nfloat)rectangle.Left, (nfloat)rectangle.Top, 
						(nfloat)(rectangle.Right - rectangle.Left),
						(nfloat)(rectangle.Bottom - rectangle.Top)),
						cornerRadius);
				}
				break;
			case GraphicOperator.BezierAddClip:
				{
					_nativePath.AddClip ();
				}
				break;
			case GraphicOperator.BezierAddPath:
				{
					throw new NotImplementedException ();
				}
				break;
			case GraphicOperator.BezierClosePath:
				{
					_nativePath.ClosePath ();
				}
				break;
			case GraphicOperator.BezierFill:
				{
					var color = (Xamarin.Forms.Color)command.Parameters [0];
					var nativeColor = color.ToUIColor ();
					nativeColor.SetFill ();
					_nativePath.Fill ();
				}
				break;
			case GraphicOperator.BezierStroke:
				{
					var lineWidth = (float?)command.Parameters [0];
					var color = (Xamarin.Forms.Color)command.Parameters [1];
					var nativeColor = color.ToUIColor ();
					if (lineWidth.HasValue) {
						_nativePath.LineWidth = lineWidth.Value;
					}
					nativeColor.SetStroke ();
					_nativePath.Stroke ();
				}
				break;
			default:
				throw(new NotImplementedException ());
			}
		}

		//		public override CGSize IntrinsicContentSize {
		//			get {
		//				var size = base.IntrinsicContentSize;
		//				Logger.WriteLine ("{0} IntrinsicContentSize [{1:N},{2:N}]", ((GraphicView)Element).BuilderSource, size.Width, size.Height);
		//				return size;
		//			}
		//		}
	}
}

