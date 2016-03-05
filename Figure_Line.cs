using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class Figure_Line : IFigure
    {
        Point[] general_Points;
        int mode_Rotation;

        public Figure_Line(int count_X, int x, int rotation)
        {
            switch (rotation)
            {
                case 0:
                    general_Points = new Point[4]
                    {
                        new Point(0, 0),
                        new Point(0, 1),
                        new Point(0, 2),
                        new Point(0, 3)
                    };
                    break;
                case 1:
                    general_Points = new Point[4]
                    {
                        new Point(0, 0),
                        new Point(1, 0),
                        new Point(2, 0),
                        new Point(3, 0)
                    };
                    break;
                default:
                    break;
            }

            this.mode_Rotation = rotation;

            for (int i = 0; i < general_Points.Length; i++)
            {
                general_Points[i].X += x;
                while (general_Points[i].X >= count_X)
                {
                    for (int j = 0; j < general_Points.Length; j++)
                    {
                        general_Points[i].X--;
                    }
                }
            }
        }

        public Point[] GetPints()
        {
            return general_Points;
        }


        public Point[] GetLowPoints(Point[] point)
        {
            if (mode_Rotation == 0)
                return new Point[] { point[3] };
            else
                return new Point[] { point[0], point[1], point[2], point[3] };
        }


        public Point[] GetTheRightPoints(Point[] point)
        {
            if (mode_Rotation == 0)
                return new Point[] { point[0], point[1], point[2], point[3] };
            else
                return new Point[] { point[3] };
        }

        public Point[] GetTheLeftPoints(Point[] point)
        {
            if (mode_Rotation == 0)
                return new Point[] { point[0], point[1], point[2], point[3] };
            else
                return new Point[] { point[0] };
        }


        public Point[] Rotation(Point[] point)
        {
            if (point[0].Y == point[1].Y)
                mode_Rotation = 0;
            else
                mode_Rotation = 1;

            Point[] pointTemp = new Point[4];

            switch (mode_Rotation)
            {
                case 0:
                    for (int i = 0; i < point.Length; i++)
                    {
                        pointTemp[i] = point[1];
                    }
                    pointTemp[0].Y--;
                    pointTemp[3].Y = ++pointTemp[2].Y + 1;
                    break;
                case 1:
                    for (int i = 0; i < point.Length; i++)
                    {
                        pointTemp[i] = point[1];
                    }
                    pointTemp[0].X--;
                    pointTemp[3].X = ++pointTemp[2].X + 1;
                    break;
                default:
                    break;
            }
            return pointTemp;
        }
    }
}
