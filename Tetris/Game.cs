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

        private const int PreviewCells = 4;

        // «7-bag»: каждая из 7 фигур выпадает ровно один раз за «мешок»
        private readonly int[] bag = new int[7];
        private int bagIndex;

        public event EventHandler<ScoreEventArgs> ScoreChanged;

        public event EventHandler<GameOverEventArgs> GameOver;

        public bool IsPaused { get; private set; }

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

            RefillBag();

            timer.Interval = 500;
            timer.Tick += TimerTick;
        }

        public void Start()
        {
            ClearBoard();

            currentTetromino = CreateNextTetromino();
            nextTetromino = CreateNextTetromino();

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
            {
                return;
            }

            var newPosition = new Point(currentTetromino.Position.X + deltaX, currentTetromino.Position.Y);
            var candidate = currentTetromino.GetPointsAt(newPosition, currentTetromino.CurrentState);

            if (!CanPlace(candidate))
            {
                return;
            }

            currentTetromino.Position = newPosition;
            RepaintCurrentTetromino();
        }

        public void Rotate()
        {
            if (currentTetromino == null)
            {
                return;
            }

            const int direction = 1;
            var originalPosition = currentTetromino.Position;
            var rotated = currentTetromino.GetRotatedPoints(direction);

            if (CanPlace(rotated))
            {
                ApplyRotation(direction);
                return;
            }

            // Простой wall kick влево/вправо на 1…2 клетки
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

            // Вертикальный kick вверх — позволяет вращаться вплотную к полу
            var kickedUp = rotated.Select(p => new Point(p.X, p.Y - 1)).ToArray();
            if (CanPlace(kickedUp))
            {
                currentTetromino.Position = new Point(originalPosition.X, originalPosition.Y - 1);
                ApplyRotation(direction);
            }
        }

        public void HardDrop()
        {
            if (currentTetromino == null)
            {
                return;
            }

            var y = currentTetromino.Position.Y;
            while (true)
            {
                var candidate = currentTetromino.GetPointsAt(
                    new Point(currentTetromino.Position.X, y + 1), currentTetromino.CurrentState);
                if (!CanPlace(candidate))
                {
                    break;
                }

                y++;
            }

            currentTetromino.Position = new Point(currentTetromino.Position.X, y);
            RepaintCurrentTetromino();

            LockTetromino();
            ClearLines();
            StartNewTetromino();
        }

        public void TogglePause()
        {
            if (currentTetromino == null)
            {
                return;
            }

            IsPaused = !IsPaused;
            if (IsPaused)
            {
                timer.Stop();
            }
            else
            {
                timer.Start();
            }
        }

        public void FastDrop(bool enabled)
        {
            timer.Interval = enabled ? 50 : 500;
        }

        /// <summary>
        /// Полная перерисовка доски и текущей фигуры (для восстановления буфера после изменения размеров панелей).
        /// </summary>
        public void RenderBoard()
        {
            RedrawBoard();
            if (currentTetromino != null)
            {
                previousPoints = currentTetromino.GetPoints();
                DrawCurrentTetromino();
            }
        }

        public void Dispose()
        {
            timer?.Stop();
            timer?.Dispose();
        }

        private int NextShapeIndex()
        {
            if (bagIndex >= bag.Length)
            {
                RefillBag();
            }

            return bag[bagIndex++];
        }

        private void RefillBag()
        {
            for (var i = 0; i < bag.Length; i++)
            {
                bag[i] = i;
            }

            // Перемешивание Фишера–Йетса
            for (var i = bag.Length - 1; i > 0; i--)
            {
                var j = random.Next(i + 1);
                var tmp = bag[i];
                bag[i] = bag[j];
                bag[j] = tmp;
            }

            bagIndex = 0;
        }

        private Tetromino CreateNextTetromino()
        {
            return Tetromino.Create(NextShapeIndex(), random, columns);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (currentTetromino == null)
            {
                return;
            }

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

            // Фигура рисуется по центру панели предпросмотра, независимо от Position
            previousNextPoints = GetCenteredPreviewPoints(nextTetromino.GetPoints());
            foreach (var point in previousNextPoints)
            {
                drawNextCell(point, nextTetromino.Brush);
            }

            renderNext?.Invoke();
        }

        private Point[] GetCenteredPreviewPoints(Point[] points)
        {
            var minX = points.Min(p => p.X);
            var maxX = points.Max(p => p.X);
            var minY = points.Min(p => p.Y);
            var maxY = points.Max(p => p.Y);

            var width = maxX - minX + 1;
            var height = maxY - minY + 1;
            var offsetX = (PreviewCells - width) / 2;
            var offsetY = (PreviewCells - height) / 2;

            return points
                .Select(p => new Point(p.X - minX + offsetX, p.Y - minY + offsetY))
                .ToArray();
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
            nextTetromino = CreateNextTetromino();

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
            var cleared = 0;

            // Снизу вверх: полные строки пропускаем (считаем), неполные сдвигаем вниз на их место
            for (var y = rows - 1; y >= 0; y--)
            {
                if (IsLineFull(y))
                {
                    cleared++;
                }
                else if (cleared > 0)
                {
                    CopyRow(y, y + cleared);
                    ClearRow(y);
                }
            }

            if (cleared == 0)
            {
                return;
            }

            for (var y = 0; y < cleared; y++)
            {
                ClearRow(y);
            }

            RedrawBoard();

            score += 100 * cleared;
            OnScoreChanged();
        }

        private bool IsLineFull(int y)
        {
            for (var x = 0; x < columns; x++)
            {
                if (!board[y, x].IsOccupied)
                {
                    return false;
                }
            }

            return true;
        }

        private void CopyRow(int from, int to)
        {
            for (var x = 0; x < columns; x++)
            {
                board[to, x] = board[from, x];
            }
        }

        private void ClearRow(int y)
        {
            for (var x = 0; x < columns; x++)
            {
                board[y, x].IsOccupied = false;
                board[y, x].Brush = null;
            }
        }

        private void RedrawBoard()
        {
            for (var y = 0; y < rows; y++)
            {
                for (var x = 0; x < columns; x++)
                {
                    var point = new Point(x, y);
                    if (board[y, x].IsOccupied)
                    {
                        drawGameCell(point, board[y, x].Brush);
                    }
                    else
                    {
                        clearGameCell(point);
                    }
                }
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
            return points.Any(p =>
                p.X < 0 || p.X >= columns ||
                p.Y < 0 || p.Y >= rows ||
                board[p.Y, p.X].IsOccupied);
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