using System.Drawing;

namespace Tetris
{
    public class FigureZ : Figure, IFigure
    {
        public FigureZ(int countX, int x, int rotation) : base(rotation)
        {
            if (rotation % 2 == 0)
            {
                GeneralPoints = new[]
                {
                    new Point(1, 1),
                    new Point(1, 0),
                    new Point(2, 1),
                    new Point(0, 0)
                };
            }
            else
            {
                GeneralPoints = new[]
                {
                    new Point(0, 1),
                    new Point(1, 1),
                    new Point(0, 2),
                    new Point(1, 0)
                };
            }

            CorrectPoints(countX, x);
        }

        public override Point[] GetLowPoints(Point[] point)
        {
            return modeRotation % 2 == 0 ? new[] { point[3], point[0], point[2] } : new[] { point[1], point[2] };
        }

        public override Point[] GetRightPoints(Point[] point)
        {
            return modeRotation % 2 == 0 ? new[] { point[2], point[1] } : new[] { point[2], point[1], point[3] };
        }

        public override Point[] GetLeftPoints(Point[] point)
        {
            return modeRotation % 2 == 0 ? new[] { point[0], point[3] } : new[] { point[2], point[0], point[3] };
        }

        public override Point[] Rotation(Point[] point)
        {
            if (point[0].X == point[1].X - 1)
                modeRotation = 0;
            else
                if (point[0].Y == point[1].Y + 1)
                    modeRotation = 1;
                /*else
                    if (point[0].X == point[1].X - 1 && point[0].Y == point[1].Y + 1)
                        mode_Rotation = 2;
                    else
                        mode_Rotation = 3;*/

            var pointTemp = new Point[4];

            pointTemp[0] = point[0];
            if (modeRotation % 2 == 0)
            {
                pointTemp[1].X = point[0].X;
                pointTemp[1].Y = point[0].Y - 1;
                pointTemp[3].X = pointTemp[1].X - 1;
                pointTemp[3].Y = pointTemp[1].Y;
                pointTemp[2].X = point[0].X + 1;
                pointTemp[2].Y = point[0].Y;
            }
            else
            {
                pointTemp[1].X = point[0].X + 1;
                pointTemp[1].Y = point[0].Y;
                pointTemp[3].X = pointTemp[1].X;
                pointTemp[3].Y = pointTemp[1].Y - 1;
                pointTemp[2].X = point[0].X;
                pointTemp[2].Y = point[0].Y + 1;
            }

            return pointTemp;
        }
    }
}