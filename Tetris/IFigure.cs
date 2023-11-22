using System.Drawing;

namespace Tetris
{
    interface IFigure
    {
        Point[] GetPoints();

        Point[] GetLowPoints(Point[] point);

        Point[] GetRightPoints(Point[] point);

        Point[] GetLeftPoints(Point[] point);

        Point[] Rotation(Point[] point);
    }
}
