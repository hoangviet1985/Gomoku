using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Gomoku
{
    class Draw
    {
        public static void DrawO(int x, int y, int width, int height, Canvas cv)
        {

            Ellipse circle = new Ellipse()
            {
                
                Width = width,
                Height = height,
                Stroke = Brushes.Red,
                StrokeThickness = 2,
            };

            cv.Children.Add(circle);

            circle.SetValue(Canvas.LeftProperty, (double)x - HyperParam.circleRadius);
            circle.SetValue(Canvas.TopProperty, (double)y - HyperParam.circleRadius);
        }

        public static void DrawX(int x, int y, Canvas cv)
        {

            var line1 = new Line()
            {
                X1 = x - HyperParam.circleRadius,
                Y1 = y - HyperParam.circleRadius,
                X2 = x + HyperParam.circleRadius,
                Y2 = y + HyperParam.circleRadius,
                Stroke = Brushes.Blue,
                StrokeThickness = 2,
            };

            var line2 = new Line()
            {
                X1 = x - HyperParam.circleRadius,
                Y1 = y + HyperParam.circleRadius,
                X2 = x + HyperParam.circleRadius,
                Y2 = y - HyperParam.circleRadius,
                Stroke = Brushes.Blue,
                StrokeThickness = 2,
            };

            cv.Children.Add(line1);
            cv.Children.Add(line2);
        }
    }
}
