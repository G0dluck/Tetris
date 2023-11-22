using System.Drawing;

namespace Tetris
{
    public class FigureSquare : Figure, IFigure
    {
        public FigureSquare(int countX, int x) : base(0)
        {
            GeneralPoints = new[]
            {
                new Point(0, 0),
                new Point(0, 1),
                new Point(1, 0),
                new Point(1, 1)
            };

            CorrectPoints(countX, x);
        }

        public override Point[] GetLowPoints(Point[] point)
        {
            return new[] { point[1], point[3] };
        }

        public override Point[] GetRightPoints(Point[] point)
        {
            return new[] { point[2], point[3] };
        }

        public override Point[] GetLeftPoints(Point[] point)
        {
            return new[] { point[0], point[1] };
        }

        public override Point[] Rotation(Point[] point)
        {
            return point;
        }
    }
}