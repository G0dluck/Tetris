using System.Drawing;

namespace Tetris
{
    public abstract class Figure
    {
        protected Point[] GeneralPoints;
        protected int modeRotation;
        protected int? tempModeRotation;
        private readonly int countX;
        private readonly int positionX;

        protected Figure(int countX, int x, int rotation)
        {
            this.countX = countX;
            this.positionX = x;
            this.modeRotation = rotation;
        }

        protected Figure(int countX, int x)
        {
            this.countX = countX;
            this.positionX = x;
        }

        public abstract Brush Brush { get; }

        public Point[] GetPoints()
        {
            return GeneralPoints;
        }

        public Point[] CorrectPoints()
        {
            var points = (Point[])GeneralPoints.Clone();
            for (var i = 0; i < points.Length; i++)
            {
                points[i].X += positionX;
                while (points[i].X >= countX)
                {
                    for (var j = 0; j < points.Length; j++)
                    {
                        points[j].X--;
                    }
                }
            }

            return points;
        }

        public void RevertModeRotation()
        {
            if (tempModeRotation.HasValue)
            {
                modeRotation = tempModeRotation.Value;
                tempModeRotation = null;
            }
        }

        public abstract Point[] GetLowPoints(Point[] point);

        public abstract Point[] GetRightPoints(Point[] point);

        public abstract Point[] GetLeftPoints(Point[] point);

        public abstract Point[] Rotation(Point[] point);
    }
}