using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Globalization;

namespace NPO.SPARC._3.Control
{
    public class ArrowIndicator : System.Windows.Controls.Control
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(ArrowIndicator),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty MinScaleValueProperty =
            DependencyProperty.Register("MinScaleValue", typeof(double), typeof(ArrowIndicator), new PropertyMetadata(-50.0));

        public static readonly DependencyProperty MaxScaleValueProperty =
            DependencyProperty.Register("MaxScaleValue", typeof(double), typeof(ArrowIndicator), new PropertyMetadata(50.0));

        public double MinScaleValue
        {
            get { return (double)GetValue(MinScaleValueProperty); }
            set { SetValue(MinScaleValueProperty, value); }
        }

        public double MaxScaleValue
        {
            get { return (double)GetValue(MaxScaleValueProperty); }
            set { SetValue(MaxScaleValueProperty, value); }
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ScaleColorProperty =
            DependencyProperty.Register("ScaleColor", typeof(Brush), typeof(ArrowIndicator),
                new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush ScaleColor
        {
            get { return (Brush)GetValue(ScaleColorProperty); }
            set { SetValue(ScaleColorProperty, value); }
        }

        static ArrowIndicator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ArrowIndicator), new FrameworkPropertyMetadata(typeof(ArrowIndicator)));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            double indicatorWidth = ActualWidth;
            double indicatorHeight = ActualHeight;

            double centerX = indicatorWidth / 2;
            double centerY = indicatorHeight;

            double arrowLength = indicatorHeight * 0.9;
            double arrowWidth = indicatorWidth * 0.2;

            double arrowAngle = (Value / (MaxScaleValue - MinScaleValue)) * 180;
            arrowAngle = Math.Max(-180, Math.Min(180, arrowAngle));

            Point startPoint = new Point(centerX, centerY);
            Point endPoint = new Point(centerX, centerY - arrowLength);

            double arrowBaseWidth = arrowWidth / 2;

            StreamGeometry arrowGeometry = new StreamGeometry();
            using (StreamGeometryContext context = arrowGeometry.Open())
            {
                context.BeginFigure(startPoint, true, true);
                context.LineTo(new Point(startPoint.X - arrowBaseWidth, startPoint.Y - arrowLength), true, false);
                context.LineTo(new Point(startPoint.X + arrowBaseWidth, startPoint.Y - arrowLength), true, false);
            }

            arrowGeometry.Transform = new RotateTransform(arrowAngle, centerX, centerY - arrowLength);
            drawingContext.DrawGeometry(Brushes.Red, null, arrowGeometry);
        }
    }
}
