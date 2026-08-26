using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Tetris
{
    public class Game : IDisposable
    {
        private readonly Timer timer = new Timer();
        private readonly Random random = new Random();

        private readonly Cell[,] board;
        private readonly int columns;
        private readonly int rows;

        private readonly Action<Point, Brush> drawGameCell;
        private readonly Action<Point> clearGameCell;
        private readonly Action renderGame;
        private readonly Action<Point, Brush> drawNextCell;
        private readonly Action<Point> clearNextCell;
        private readonly Action renderNext;
        private readonly Action clearNextBoard;

        private Tetromino currentTetromino;
        private Tetromino nextTetromino;
        private Point[] previousPoints;
        private Point[] previousNextPoints;
        private int score;

        public event EventHandler<ScoreEventArgs> ScoreChanged;

        public event EventHandler<GameOverEventArgs> GameOver;

        public Game(
            int columns,
            int rows,
            Action<Point, Brush> drawGameCell,
            Action<Point> clearGameCell,
            Action renderGame,
            Action<Point, Brush> drawNextCell,
            Action<Point> clearNextCell,
            Action renderNext,
            Action clearNextBoard)
        {
            this.columns = columns;
            this.rows = rows;
            this.drawGameCell = drawGameCell;
            this.clearGameCell = clearGameCell;
            this.renderGame = renderGame;
            this.drawNextCell = drawNextCell;
            this.clearNextCell = clearNextCell;
            this.renderNext = renderNext;
            this.clearNextBoard = clearNextBoard;

            board = new Cell[rows, columns];

            timer.Interval = 500;
            timer.Tick += TimerTick;
        }

        public void Start()
        {
            ClearBoard();

            currentTetromino = nextTetromino ?? Tetromino.CreateRandom(random, columns);
            nextTetromino = Tetromino.CreateRandom(random, 4);

            clearNextBoard?.Invoke();
            DrawNextTetromino();

            if (IsGameOver(currentTetromino.GetPoints()))
            {
                OnGameOver();
                return;
            }

            previousPoints = currentTetromino.GetPoints();
            DrawCurrentTetromino();

            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        public void Move(int deltaX)
        {
            if (currentTetromino == null)
                return;

            var newPosition = new Point(currentTetromino.Position.X + deltaX, currentTetromino.Position.Y);
            var candidate = currentTetromino.GetPointsAt(newPosition, currentTetromino.CurrentState);

            if (!CanPlace(candidate))
                return;

            currentTetromino.Position = newPosition;
            RepaintCurrentTetromino();
        }

        public void Rotate()
        {
            if (currentTetromino == null)
                return;

            const int direction = 1;
            var originalPosition = currentTetromino.Position;
            var rotated = currentTetromino.GetRotatedPoints(direction);

            if (CanPlace(rotated))
            {
                ApplyRotation(direction);
                return;
            }

            // Simple wall kicks
            for (var kick = 1; kick <= 2; kick++)
            {
                var kickedLeft = rotated.Select(p => new Point(p.X - kick, p.Y)).ToArray();
                if (CanPlace(kickedLeft))
                {
                    currentTetromino.Position = new Point(originalPosition.X - kick, originalPosition.Y);
                    ApplyRotation(direction);
                    return;
                }

                var kickedRight = rotated.Select(p => new Point(p.X + kick, p.Y)).ToArray();
                if (CanPlace(kickedRight))
                {
                    currentTetromino.Position = new Point(originalPosition.X + kick, originalPosition.Y);
                    ApplyRotation(direction);
                    return;
                }
            }
        }

        public void FastDrop(bool enabled)
        {
            timer.Interval = enabled ? 50 : 500;
        }

        public void Dispose()
        {
            timer?.Stop();
            timer?.Dispose();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (currentTetromino == null)
                return;

            var newPosition = new Point(currentTetromino.Position.X, currentTetromino.Position.Y + 1);
            var candidate = currentTetromino.GetPointsAt(newPosition, currentTetromino.CurrentState);

            if (CanPlace(candidate))
            {
                currentTetromino.Position = newPosition;
                RepaintCurrentTetromino();
            }
            else
            {
                LockTetromino();
                ClearLines();
                StartNewTetromino();
            }
        }

        private void ApplyRotation(int direction)
        {
            currentTetromino.Rotate(direction);
            RepaintCurrentTetromino();
        }

        private void RepaintCurrentTetromino()
        {
            ClearCurrentTetromino();
            previousPoints = currentTetromino.GetPoints();
            DrawCurrentTetromino();
        }

        private void DrawCurrentTetromino()
        {
            foreach (var point in currentTetromino.GetPoints())
            {
                drawGameCell(point, currentTetromino.Brush);
            }

            renderGame?.Invoke();
        }

        private void ClearCurrentTetromino()
        {
            foreach (var point in previousPoints)
            {
                clearGameCell(point);
            }
        }

        private void DrawNextTetromino()
        {
            if (previousNextPoints != null)
            {
                foreach (var point in previousNextPoints)
                {
                    clearNextCell(point);
                }
            }

            previousNextPoints = nextTetromino.GetPoints();
            foreach (var point in previousNextPoints)
            {
                drawNextCell(point, nextTetromino.Brush);
            }

            renderNext?.Invoke();
        }

        private void LockTetromino()
        {
            foreach (var point in currentTetromino.GetPoints())
            {
                board[point.Y, point.X] = new Cell
                {
                    IsOccupied = true,
                    Brush = currentTetromino.Brush
                };
            }

            score += 10;
            OnScoreChanged();
        }

        private void StartNewTetromino()
        {
            timer.Stop();

            currentTetromino = nextTetromino;
            nextTetromino = Tetromino.CreateRandom(random, 4);

            DrawNextTetromino();

            if (IsGameOver(currentTetromino.GetPoints()))
            {
                OnGameOver();
                return;
            }

            previousPoints = currentTetromino.GetPoints();
            DrawCurrentTetromino();

            timer.Start();
        }

        private void ClearLines()
        {
            for (var y = rows - 1; y >= 0; y--)
            {
                if (!IsLineFull(y))
                    continue;

                RemoveLine(y);
                ShiftLinesDown(y);
                score += 100;
                OnScoreChanged();
                y++;
            }
        }

        private bool IsLineFull(int y)
        {
            for (var x = 0; x < columns; x++)
            {
                if (!board[y, x].IsOccupied)
                    return false;
            }

            return true;
        }

        private void RemoveLine(int y)
        {
            for (var x = 0; x < columns; x++)
            {
                board[y, x].IsOccupied = false;
                board[y, x].Brush = null;
                clearGameCell(new Point(x, y));
            }

            renderGame?.Invoke();
        }

        private void ShiftLinesDown(int startY)
        {
            for (var y = startY; y > 0; y--)
            {
                for (var x = 0; x < columns; x++)
                {
                    board[y, x] = board[y - 1, x];
                }

                RedrawLine(y);
            }

            for (var x = 0; x < columns; x++)
            {
                board[0, x].IsOccupied = false;
                board[0, x].Brush = null;
                clearGameCell(new Point(x, 0));
            }
        }

        private void RedrawLine(int y)
        {
            for (var x = 0; x < columns; x++)
            {
                var point = new Point(x, y);
                if (board[y, x].IsOccupied)
                    drawGameCell(point, board[y, x].Brush);
                else
                    clearGameCell(point);
            }

            renderGame?.Invoke();
        }

        private void ClearBoard()
        {
            for (var y = 0; y < rows; y++)
            {
                for (var x = 0; x < columns; x++)
                {
                    board[y, x].IsOccupied = false;
                    board[y, x].Brush = null;
                    clearGameCell(new Point(x, y));
                }
            }
        }

        private bool CanPlace(Point[] points)
        {
            return points.All(p =>
                p.X >= 0 && p.X < columns &&
                p.Y >= 0 && p.Y < rows &&
                !board[p.Y, p.X].IsOccupied);
        }

        private bool IsGameOver(Point[] points)
        {
            return points.Any(p => board[p.Y, p.X].IsOccupied);
        }

        private void OnScoreChanged()
        {
            ScoreChanged?.Invoke(this, new ScoreEventArgs(score));
        }

        private void OnGameOver()
        {
            timer.Stop();

            var isHighScore = Properties.Settings.Default.HighScore < score;
            if (isHighScore)
            {
                Properties.Settings.Default.HighScore = score;
                Properties.Settings.Default.Save();
            }

            GameOver?.Invoke(this, new GameOverEventArgs(score, isHighScore));
        }
    }
}