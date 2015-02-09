!#/bin/sh

INPUT_FILE="$1"
OUTPUT_FILE="$2"

sed -e 's/: NSObject//g' -e 's/UIColor.FromRGBA/new Xamarin.Forms.Color/g' -e 's/UIColor/Xamarin.Forms.Color/g' -e 's/CGColorSpace/ColorSpace/g' -e 's/CGColor \[\]/Color \[\]/g' -e 's/.CGColor//g' -e 's/nfloat \[\]/float \[\]/g' -e 's/CGGradientDrawingOptions/GradientDrawingOption/g' -e 's/CGGradient/ColorGradient/g' -e 's/UIGraphics/Graphic/g' -e 's/GetCurrentContext()/GetCurrentContext(view)/g' -e 's/UIBezierPath/BezierPath/g' -e 's/new BezierPath()/new BezierPath(view)/g' -e 's/CGRect/Xamarin.Forms.Rectangle/g' -e 's/CGPoint/Xamarin.Forms.Point/g' -e 's/BezierPath.FromRect(/BezierPath.FromRect(view, /g' -e 's/BezierPath.FromOval(/BezierPath.FromOval(view, /g' -e 's/()$/(Graphic view)/g' "$INPUT_FILE" > "$OUTPUT_FILE"

echo "Done!"
