using System.Drawing;

namespace Tetris
{
    public class FigureLine : Figure, IFigure
    {
        public FigureLine(int countX, int x, int rotation) : base(countX, x, rotation)
        {
            switch (rotation)
            {
                case 0:
                    GeneralPoints = new[]
                    {
                        new Point(0, 0),
                        new Point(0, 1),
                        new Point(0, 2),
                        new Point(0, 3)
                    };
                    break;
                case 1:
                    GeneralPoints = new[]
                    {
                        new Point(0, 0),
                        new Point(1, 0),
                        new Point(2, 0),
                        new Point(3, 0)
                    };
                    break;
                default:
                    GeneralPoints = new Point[4];
                    break;
            }
        }

        public override Brush Brush => Brushes.Green;

        public override Point[] GetLowPoints(Point[] point)
        {
            return modeRotation == 0 ? new[] { point[3] } : new[] { point[0], point[1], point[2], point[3] };
        }


        public override Point[] GetRightPoints(Point[] point)
        {
            return modeRotation == 0 ? new[] { point[0], point[1], point[2], point[3] } : new[] { point[3] };
        }

        public override Point[] GetLeftPoints(Point[] point)
        {
            return modeRotation == 0 ? new[] { point[0], point[1], point[2], point[3] } : new[] { point[0] };
        }

        public override Point[] Rotation(Point[] point)
        {
            modeRotation = point[0].Y == point[1].Y ? 0 : 1;

            var pointTemp = new Point[4];

            switch (modeRotation)
            {
                case 0:
                    for (var i = 0; i < point.Length; i++)
                    {
                        pointTemp[i] = point[1];
                    }

                    pointTemp[0].Y--;
                    pointTemp[3].Y = ++pointTemp[2].Y + 1;
                    break;
                case 1:
                    for (var i = 0; i < point.Length; i++)
                    {
                        pointTemp[i] = point[1];
                    }

                    pointTemp[0].X--;
                    pointTemp[3].X = ++pointTemp[2].X + 1;
                    break;
            }

            return pointTemp;
        }
    }
}