using System;
using System.Drawing;
using System.Linq;

namespace Tetris
{
    public class Tetromino
    {
        private readonly Point[][] states;

        public Tetromino(Point[][] states, Brush brush)
        {
            if (states == null)
                throw new ArgumentNullException(nameof(states));
            if (states.Length == 0)
                throw new ArgumentException("States cannot be empty.", nameof(states));

            this.states = states;
            this.Brush = brush ?? throw new ArgumentNullException(nameof(brush));
        }

        public Brush Brush { get; }

        public Point Position { get; set; }

        public int CurrentState { get; private set; }

        public Point[] GetPoints()
        {
            return GetPointsForState(CurrentState);
        }

        public Point[] GetRotatedPoints(int direction)
        {
            var targetState = NormalizeState(CurrentState + direction);
            return GetPointsForState(targetState);
        }

        public Point[] GetPointsAt(Point position, int state)
        {
            var targetState = NormalizeState(state);
            return states[targetState]
                .Select(p => new Point(p.X + position.X, p.Y + position.Y))
                .ToArray();
        }

        public void Rotate(int direction)
        {
            CurrentState = NormalizeState(CurrentState + direction);
        }

        private Point[] GetPointsForState(int state)
        {
            return states[state]
                .Select(p => new Point(p.X + Position.X, p.Y + Position.Y))
                .ToArray();
        }

        private int NormalizeState(int state)
        {
            var length = states.Length;
            return ((state % length) + length) % length;
        }

        public static Tetromino CreateRandom(Random random, int boardWidth)
        {
            switch (random.Next(7))
            {
                case 0: return CreateI(random, boardWidth);
                case 1: return CreateO(random, boardWidth);
                case 2: return CreateT(random, boardWidth);
                case 3: return CreateJ(random, boardWidth);
                case 4: return CreateL(random, boardWidth);
                case 5: return CreateS(random, boardWidth);
                case 6: return CreateZ(random, boardWidth);
                default: throw new InvalidOperationException();
            }
        }

        private static Tetromino CreateI(Random random, int boardWidth)
        {
            var figure = new Tetromino(
                new[]
                {
                    new[] { new Point(0, 1), new Point(1, 1), new Point(2, 1), new Point(3, 1) },
                    new[] { new Point(2, 0), new Point(2, 1), new Point(2, 2), new Point(2, 3) }
                },
                Brushes.Green);

            figure.Position = new Point(random.Next(0, boardWidth - 3), 0);
            return figure;
        }

        private static Tetromino CreateO(Random random, int boardWidth)
        {
            var figure = new Tetromino(
                new[]
                {
                    new[] { new Point(0, 0), new Point(1, 0), new Point(0, 1), new Point(1, 1) }
                },
                Brushes.DarkRed);

            figure.Position = new Point(random.Next(0, boardWidth - 1), 0);
            return figure;
        }

        private static Tetromino CreateT(Random random, int boardWidth)
        {
            var figure = new Tetromino(
                new[]
                {
                    new[] { new Point(1, 0), new Point(0, 1), new Point(1, 1), new Point(2, 1) },
                    new[] { new Point(0, 0), new Point(0, 1), new Point(1, 1), new Point(0, 2) },
                    new[] { new Point(0, 1), new Point(1, 1), new Point(2, 1), new Point(1, 2) },
                    new[] { new Point(2, 0), new Point(1, 1), new Point(2, 1), new Point(2, 2) }
                },
                Brushes.DarkViolet);

            figure.Position = new Point(random.Next(0, boardWidth - 2), 0);
            return figure;
        }

        private static Tetromino CreateJ(Random random, int boardWidth)
        {
            var figure = new Tetromino(
                new[]
                {
                    new[] { new Point(2, 0), new Point(2, 1), new Point(1, 2), new Point(2, 2) },
                    new[] { new Point(0, 1), new Point(0, 2), new Point(1, 2), new Point(2, 2) },
                    new[] { new Point(0, 0), new Point(1, 0), new Point(0, 1), new Point(0, 2) },
                    new[] { new Point(0, 0), new Point(1, 0), new Point(2, 0), new Point(2, 1) }
                },
                Brushes.YellowGreen);

            figure.Position = new Point(random.Next(0, boardWidth - 2), 0);
            return figure;
        }

        private static Tetromino CreateL(Random random, int boardWidth)
        {
            var figure = new Tetromino(
                new[]
                {
                    new[] { new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(1, 2) },
                    new[] { new Point(0, 0), new Point(1, 0), new Point(2, 0), new Point(0, 1) },
                    new[] { new Point(1, 0), new Point(2, 0), new Point(2, 1), new Point(2, 2) },
                    new[] { new Point(2, 0), new Point(0, 1), new Point(1, 1), new Point(2, 1) }
                },
                Brushes.Blue);

            figure.Position = new Point(random.Next(0, boardWidth - 2), 0);
            return figure;
        }

        private static Tetromino CreateS(Random random, int boardWidth)
        {
            var figure = new Tetromino(
                new[]
                {
                    new[] { new Point(1, 0), new Point(2, 0), new Point(0, 1), new Point(1, 1) },
                    new[] { new Point(0, 0), new Point(0, 1), new Point(1, 1), new Point(1, 2) }
                },
                Brushes.Orange);

            figure.Position = new Point(random.Next(0, boardWidth - 2), 0);
            return figure;
        }

        private static Tetromino CreateZ(Random random, int boardWidth)
        {
            var figure = new Tetromino(
                new[]
                {
                    new[] { new Point(0, 0), new Point(1, 0), new Point(1, 1), new Point(2, 1) },
                    new[] { new Point(1, 0), new Point(0, 1), new Point(1, 1), new Point(0, 2) }
                },
                Brushes.Coral);

            figure.Position = new Point(random.Next(0, boardWidth - 2), 0);
            return figure;
        }
    }
}
