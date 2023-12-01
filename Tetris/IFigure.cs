using System.Drawing;

namespace Tetris
{
    public interface IFigure
    {
        Brush Brush { get; }

        Point[] GetPoints();

        Point[] CorrectPoints();

        void RevertModeRotation();

        Point[] GetLowPoints(Point[] point);

        Point[] GetRightPoints(Point[] point);

        Point[] GetLeftPoints(Point[] point);

        Point[] Rotation(Point[] point);
    }
}
