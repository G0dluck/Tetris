using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    interface IFigure
    {
        Point[] GetPints();

        Point[] GetLowPoints(Point[] point);

        Point[] GetTheRightPoints(Point[] point);

        Point[] GetTheLeftPoints(Point[] point);

        Point[] Rotation(Point[] point);
    }
}
