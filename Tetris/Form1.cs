using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tetris
{
    public partial class Form1 : Form
    {
        private Game game;

        private Graphics gameGraphics;
        private BufferedGraphicsContext gameBufferedGraphicsContext;
        private BufferedGraphics gameBufferedGraphics;

        private Graphics nextGraphics;
        private BufferedGraphicsContext nextBufferedGraphicsContext;
        private BufferedGraphics nextBufferedGraphics;

        private int cellWidth;
        private int cellHeight;
        private const int Columns = 10;
        private const int Rows = 20;

        public Form1()
        {
            InitializeComponent();
            InitializeGraphics();

            FormClosed += Form1FormClosed;
            panel2.Paint += Panel2Paint;

            if (System.IO.File.Exists("test.txt"))
            {
                System.IO.File.Delete("test.txt");
            }
        }

        private void InitializeGraphics()
        {
            gameGraphics = Graphics.FromHwnd(panel1.Handle);
            gameBufferedGraphicsContext = new BufferedGraphicsContext();
            gameBufferedGraphics = gameBufferedGraphicsContext.Allocate(gameGraphics, new Rectangle(0, 0,
                panel1.Size.Width, panel1.Size.Height));

            nextGraphics = Graphics.FromHwnd(panel2.Handle);
            nextBufferedGraphicsContext = new BufferedGraphicsContext();
            nextBufferedGraphics = nextBufferedGraphicsContext.Allocate(nextGraphics, new Rectangle(0, 0,
                panel2.Size.Width, panel2.Size.Height));

            cellWidth = panel1.Width / Columns;
            cellHeight = panel1.Height / Rows;

            DrawGrid(gameBufferedGraphics, panel1.Width, panel1.Height);
            DrawGrid(nextBufferedGraphics, panel2.Width, panel2.Height);

            gameBufferedGraphics.Render();
            nextBufferedGraphics.Render();
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
                return;

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
            }
        }

        private void Form1KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
                game?.FastDrop(false);
        }

        private void Panel1Paint(object sender, PaintEventArgs e)
        {
            gameBufferedGraphics?.Render();
        }

        private void Panel2Paint(object sender, PaintEventArgs e)
        {
            nextBufferedGraphics?.Render();
        }

        private void Form1FormClosed(object sender, FormClosedEventArgs e)
        {
            game?.Dispose();

            gameBufferedGraphics?.Dispose();
            nextBufferedGraphics?.Dispose();
            gameBufferedGraphicsContext?.Dispose();
            nextBufferedGraphicsContext?.Dispose();
            gameGraphics?.Dispose();
            nextGraphics?.Dispose();
        }
    }
}