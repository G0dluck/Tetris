using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class Figure_Z : IFigure
    {
        Point[] general_Points;
        int mode_Rotation;

        public Figure_Z(int count_X, int x, int rotation)
        {
            if (rotation % 2 == 0)
            {
                general_Points = new Point[4]
                    {
                        new Point(1, 1),
                        new Point(1, 0),
                        new Point(2, 1),
                        new Point(0, 0)
                    };
            }
            else
            {
                general_Points = new Point[4]
                    {
                        new Point(0, 1),
                        new Point(1, 1),
                        new Point(0, 2),
                        new Point(1, 0)
                    };
            }

            this.mode_Rotation = rotation;

            for (int i = 0; i < general_Points.Length; i++)
            {
                general_Points[i].X += x;
                while (general_Points[i].X >= count_X)
                {
                    for (int j = 0; j < general_Points.Length; j++)
                    {
                        general_Points[j].X--;
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
            if (mode_Rotation % 2 == 0)
            {
                return new Point[] { point[3], point[0], point[2] };
            }
            else
            {
                return new Point[] { point[1], point[2] };
            }
        }

        public Point[] GetTheRightPoints(Point[] point)
        {
            if (mode_Rotation % 2 == 0)
            {
                return new Point[] { point[2], point[1] };
            }
            else
            {
                return new Point[] { point[2], point[1], point[3] };
            }
        }

        public Point[] GetTheLeftPoints(Point[] point)
        {
            if (mode_Rotation % 2 == 0)
            {
                return new Point[] { point[0], point[3] };
            }
            else
            {
                return new Point[] { point[2], point[0], point[3] };
            }
        }

        public Point[] Rotation(Point[] point)
        {
            if (point[0].X == point[1].X - 1)
                mode_Rotation = 0;
            else
                if (point[0].Y == point[1].Y + 1)
                    mode_Rotation = 1;
                /*else
                    if (point[0].X == point[1].X - 1 && point[0].Y == point[1].Y + 1)
                        mode_Rotation = 2;
                    else
                        mode_Rotation = 3;*/

            Point[] pointTemp = new Point[4];

            pointTemp[0] = point[0];
            if (mode_Rotation % 2 == 0)
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
