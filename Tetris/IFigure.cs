using System.Drawing;

namespace Tetris
{
    public interface IFigure
    {
        Brush Brush { get; }

        Point[] GetPoints();

        Point[] CorrectPoints();

        Point[] GetLowPoints(Point[] point);

        Point[] GetRightPoints(Point[] point);

        Point[] GetLeftPoints(Point[] point);

        Point[] Rotation(Point[] point);
    }
}
