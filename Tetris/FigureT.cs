using System.Drawing;

namespace Tetris
{
    public class FigureT : Figure, IFigure
    {
        public FigureT(int countX, int x, int rotation) : base(countX, x, rotation)
        {
            switch (rotation)
            {
                case 0:
                    GeneralPoints = new[]
                    {
                        new Point(1, 1),
                        new Point(1, 0),
                        new Point(2, 1),
                        new Point(0, 1)
                    };
                    break;
                case 1:
                    GeneralPoints = new[]
                    {
                        new Point(0, 1),
                        new Point(1, 1),
                        new Point(0, 2),
                        new Point(0, 0)
                    };
                    break;
                case 2:
                    GeneralPoints = new[]
                    {
                        new Point(1, 0),
                        new Point(1, 1),
                        new Point(0, 0),
                        new Point(2, 0)
                    };
                    break;
                case 3:
                    GeneralPoints = new[]
                    {
                        new Point(1, 1),
                        new Point(0, 1),
                        new Point(1, 0),
                        new Point(1, 2)
                    };
                    break;
                default:
                    GeneralPoints = new Point[4];
                    break;
            }
        }

        public override Brush Brush => Brushes.DarkViolet;

        public override Point[] GetLowPoints(Point[] point)
        {
            switch (modeRotation)
            {
                case 0:
                    return new[] { point[3], point[0], point[2] };
                case 1:
                    return new[] { point[2], point[1] };
                case 2:
                    return new[] { point[2], point[1], point[3] };
                case 3:
                    return new[] { point[1], point[3] };
                default:
                    return point;
            }
        }

        public override Point[] GetRightPoints(Point[] point)
        {
            switch (modeRotation)
            {
                case 0:
                    return new[] { point[1], point[2] };
                case 1:
                    return new[] { point[3], point[1], point[2] };
                case 2:
                    return new[] { point[3], point[1] };
                case 3:
                    return new[] { point[2], point[0], point[3] };
                default:
                    return point;
            }
        }

        public override Point[] GetLeftPoints(Point[] point)
        {
            switch (modeRotation)
            {
                case 0:
                    return new[] { point[1], point[3] };
                case 1:
                    return new[] { point[3], point[0], point[2] };
                case 2:
                    return new[] { point[2], point[1] };
                case 3:
                    return new[] { point[2], point[1], point[3] };
                default:
                    return point;
            }
        }

        public override Point[] Rotation(Point[] point)
        {
            tempModeRotation = modeRotation;
            if (point[0].X == point[1].X + 1)
                modeRotation = 0;
            else
                if (point[0].Y == point[1].Y + 1)
                    modeRotation = 1;
                else
                    if (point[0].X == point[1].X - 1)
                        modeRotation = 2;
                    else
                        modeRotation = 3;

            var pointTemp = new Point[4];

            pointTemp[0] = point[0];
            pointTemp[1] = point[2];
            pointTemp[3] = point[1];
            switch (modeRotation)
            {
                case 0:
                    pointTemp[2].X = point[2].X + 1;
                    pointTemp[2].Y = point[2].Y + 1;
                    break;
                case 1:
                    pointTemp[2].X = point[2].X - 1;
                    pointTemp[2].Y = point[2].Y + 1;
                    break;
                case 2:
                    pointTemp[2].X = point[2].X - 1;
                    pointTemp[2].Y = point[2].Y - 1;
                    break;
                case 3:
                    pointTemp[2].X = point[2].X + 1;
                    pointTemp[2].Y = point[2].Y - 1;
                    break;
            }

            return pointTemp;
        }
    }
}
