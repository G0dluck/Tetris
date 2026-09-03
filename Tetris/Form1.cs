using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tetris
{
    public partial class Form1 : Form
    {
        private Game game;

        private BufferedGraphicsContext gameBufferedGraphicsContext;
        private BufferedGraphics gameBufferedGraphics;
        private Graphics gameGraphics;
        private Size gameBufferSize;

        private BufferedGraphicsContext nextBufferedGraphicsContext;
        private BufferedGraphics nextBufferedGraphics;
        private Graphics nextGraphics;
        private Size nextBufferSize;

        private int cellWidth;
        private int cellHeight;
        private const int Columns = 10;
        private const int Rows = 20;

        public Form1()
        {
            InitializeComponent();

            FormClosed += Form1FormClosed;
            Deactivate += Form1Deactivate;

            // Панели фиксированного размера: буферы рассчитаны под них, окно не растягиваем
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            panel1.Resize += PanelResized;
            panel2.Resize += PanelResized;
            panel2.Paint += Panel2Paint;

            gameBufferedGraphicsContext = new BufferedGraphicsContext();
            nextBufferedGraphicsContext = new BufferedGraphicsContext();

            EnsureGameBuffer();
            EnsureNextBuffer();
        }

        private void EnsureGameBuffer()
        {
            var width = Math.Max(panel1.Width, 1);
            var height = Math.Max(panel1.Height, 1);

            if (gameBufferedGraphics != null && gameBufferSize.Width == width && gameBufferSize.Height == height)
            {
                return;
            }

            gameBufferedGraphics?.Dispose();
            gameGraphics?.Dispose();
            gameGraphics = panel1.CreateGraphics();
            gameBufferedGraphics = gameBufferedGraphicsContext.Allocate(gameGraphics, new Rectangle(0, 0, width, height));
            gameBufferSize = new Size(width, height);

            cellWidth = panel1.Width / Columns;
            cellHeight = panel1.Height / Rows;

            DrawGrid(gameBufferedGraphics, panel1.Width, panel1.Height);
            gameBufferedGraphics.Render();
        }

        private void EnsureNextBuffer()
        {
            var width = Math.Max(panel2.Width, 1);
            var height = Math.Max(panel2.Height, 1);

            if (nextBufferedGraphics != null && nextBufferSize.Width == width && nextBufferSize.Height == height)
            {
                return;
            }

            nextBufferedGraphics?.Dispose();
            nextGraphics?.Dispose();
            nextGraphics = panel2.CreateGraphics();
            nextBufferedGraphics = nextBufferedGraphicsContext.Allocate(nextGraphics, new Rectangle(0, 0, width, height));
            nextBufferSize = new Size(width, height);

            DrawGrid(nextBufferedGraphics, panel2.Width, panel2.Height);
            nextBufferedGraphics.Render();
        }

        private void PanelResized(object sender, EventArgs e)
        {
            EnsureGameBuffer();
            EnsureNextBuffer();
            game?.RenderBoard();
        }

        private void DrawGrid(BufferedGraphics bufferedGraphics, int width, int height)
        {
            bufferedGraphics.Graphics.Clear(BackColor);

            using (var pen = new Pen(Color.Black))
            {
                for (var x = 0; x <= width; x += cellWidth)
                {
                    for (var y = 0; y <= height; y += cellHeight)
                    {
                        bufferedGraphics.Graphics.DrawRectangle(pen, x, y, cellWidth, cellHeight);
                    }
                }
            }
        }

        private void ClearNextBoard()
        {
            EnsureNextBuffer();
            nextBufferedGraphics.Graphics.Clear(BackColor);
            DrawGrid(nextBufferedGraphics, panel2.Width, panel2.Height);
            nextBufferedGraphics.Render();
        }

        private void StartButtonClick(object sender, EventArgs e)
        {
            game?.Stop();
            game?.Dispose();
            game = null;

            game = new Game(
                Columns,
                Rows,
                (point, brush) => DrawCell(gameBufferedGraphics, point, brush),
                point => ClearCell(gameBufferedGraphics, point),
                () => gameBufferedGraphics.Render(),
                (point, brush) => DrawCell(nextBufferedGraphics, point, brush),
                point => ClearCell(nextBufferedGraphics, point),
                () => nextBufferedGraphics.Render(),
                () => ClearNextBoard());

            game.ScoreChanged += GameScoreChanged;
            game.GameOver += GameGameOver;
            game.Start();

            Text = "Tetris";
            start_button.Enabled = false;
            Focus();
        }

        private void DrawCell(BufferedGraphics bufferedGraphics, Point point, Brush brush)
        {
            bufferedGraphics.Graphics.FillRectangle(brush,
                point.X * cellWidth + 2, point.Y * cellHeight + 2, cellWidth - 3, cellHeight - 3);
        }

        private void ClearCell(BufferedGraphics bufferedGraphics, Point point)
        {
            bufferedGraphics.Graphics.FillRectangle(SystemBrushes.Control,
                point.X * cellWidth + 2, point.Y * cellHeight + 2, cellWidth - 3, cellHeight - 3);
        }

        private void GameScoreChanged(object sender, ScoreEventArgs e)
        {
            score.Text = e.Score.ToString();
        }

        private void GameGameOver(object sender, GameOverEventArgs e)
        {
            var text = e.IsHighScore
                ? "NEW HIGHSCORE!!! \nYour results: " + e.Score + " scores"
                : "Your results: " + e.Score + " scores \nHighScore: " + Properties.Settings.Default.HighScore +
                  " scores";

            MessageBox.Show(text, "Game Over!", MessageBoxButtons.OK, MessageBoxIcon.Information);

            gameBufferedGraphics.Graphics.Clear(BackColor);
            DrawGrid(gameBufferedGraphics, panel1.Width, panel1.Height);
            gameBufferedGraphics.Render();

            ClearNextBoard();

            start_button.Enabled = true;
            score.Text = "0";
            game = null;
        }

        private void Form1KeyDown(object sender, KeyEventArgs e)
        {
            if (game == null)
            {
                return;
            }

            // Во время паузы разрешена только клавиша возобновления
            if (game.IsPaused && e.KeyCode != Keys.P)
            {
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.Right:
                    game.Move(1);
                    break;
                case Keys.Left:
                    game.Move(-1);
                    break;
                case Keys.Up:
                    game.Rotate();
                    break;
                case Keys.Down:
                    game.FastDrop(true);
                    break;
                case Keys.Space:
                    game.HardDrop();
                    break;
                case Keys.P:
                    TogglePause();
                    break;
            }
        }

        private void Form1KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                game?.FastDrop(false);
            }
        }

        private void Form1Deactivate(object sender, EventArgs e)
        {
            // Сброс быстрого падения, если KeyUp не дошёл из-за потери фокуса
            game?.FastDrop(false);
        }

        private void TogglePause()
        {
            game.TogglePause();
            Text = game.IsPaused ? "Tetris — Paused (P)" : "Tetris";
        }

        private void Panel1Paint(object sender, PaintEventArgs e)
        {
            EnsureGameBuffer();
            gameBufferedGraphics?.Render();
        }

        private void Panel2Paint(object sender, PaintEventArgs e)
        {
            EnsureNextBuffer();
            nextBufferedGraphics?.Render();
        }

        private void Form1FormClosed(object sender, FormClosedEventArgs e)
        {
            game?.Dispose();

            gameBufferedGraphics?.Dispose();
            nextBufferedGraphics?.Dispose();
            gameGraphics?.Dispose();
            nextGraphics?.Dispose();
            gameBufferedGraphicsContext?.Dispose();
            nextBufferedGraphicsContext?.Dispose();
        }
    }
}