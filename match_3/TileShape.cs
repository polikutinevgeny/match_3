using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace match_3
{
    public class TileShape : Shape
    {
        protected override Geometry DefiningGeometry =>
            new RectangleGeometry(
                new Rect(
                    ActualWidth * 0.05, ActualWidth * 0.05, ActualWidth * 0.9,
                    ActualHeight * 0.9), ActualWidth * 0.1, ActualHeight * 0.1);
    }
}