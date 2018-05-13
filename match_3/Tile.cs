using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace match_3
{
    public class Tile
    {
        private int _top;
        private int _left;

        public Color Color { get; }

        public int Top
        {
            get => _top;
            set => _top = value;
        }

        public int Left
        {
            get => _left;
            set => _left = value;
        }

        public bool Deleting { get; set; }
        public TileShape Shape { get; set; }

        public bool Selected { get; set; }

        public Tile(int top, int left, Color color)
        {
            Top = top;
            Left = left;
            Color = color;
            Shape = new TileShape()
            {
                Fill = new SolidColorBrush(color),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 2.0,
                Tag = this
            };
        }

        public void SwapCoordinates(ref Tile other)
        {
            Utility.Swap(ref _top, ref other._top);
            Utility.Swap(ref _left, ref other._left);
        }
    }
}