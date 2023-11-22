using System.Drawing;

namespace Tetris
{
    public abstract class Figure
    {
        protected Point[] GeneralPoints;
        protected int modeRotation;

        protected Figure(int rotation)
        {
            this.modeRotation = rotation;
        }

        public Point[] GetPoints()
        {
            return GeneralPoints;
        }

        public abstract Point[] GetLowPoints(Point[] point);

        public abstract Point[] GetRightPoints(Point[] point);

        public abstract Point[] GetLeftPoints(Point[] point);

        public abstract Point[] Rotation(Point[] point);

        protected void CorrectPoints(int countX, int x)
        {
            for (var i = 0; i < GeneralPoints.Length; i++)
            {
                GeneralPoints[i].X += x;
                while (GeneralPoints[i].X >= countX)
                {
                    for (var j = 0; j < GeneralPoints.Length; j++)
                    {
                        GeneralPoints[j].X--;
                    }
                }
            }
        }
    }
}