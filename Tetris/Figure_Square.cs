using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class Figure_Square : IFigure
    {
        Point[] points = new Point[4] 
        {
            new Point(0, 0),
            new Point(0, 1),
            new Point(1, 0),
            new Point(1, 1)
        };

        public Figure_Square(int count_X, int x)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i].X += x;
                while (points[i].X >= count_X)
                {
                    for (int j = 0; j < points.Length; j++)
                    {
                        points[i].X--;
                    }
                }
            }
        }

        public Point[] GetPints()
        {
            return points;
        }


        public Point[] GetLowPoints(Point[] point)
        {
            return new Point[] { point[1], point[3] };
        }


        public Point[] GetTheRightPoints(Point[] point)
        {
            return new Point[] { point[2], point[3] };
        }


        public Point[] GetTheLeftPoints(Point[] point)
        {
            return new Point[] { point[0], point[1] };
        }


        public Point[] Rotation(Point[] point)
        {
            throw new NotImplementedException();
        }
    }
}
