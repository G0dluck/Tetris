using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tetris
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //Properties.Settings.Default.Reset();

            if (System.IO.File.Exists("test.txt"))
            {
                System.IO.File.Delete("test.txt");
            }
        }

        private Game game;

        private Graphics graphics;
        private BufferedGraphicsContext bufferedGraphicsContext;
        private BufferedGraphics bufferedGraphics;

        private int recWidth;
        private int recHeight;
        private const int countX = 10, countY = 20;

        private void InitializeGraphics()
        {
            graphics = Graphics.FromHwnd(panel1.Handle);
            bufferedGraphicsContext = new BufferedGraphicsContext();
            bufferedGraphics = bufferedGraphicsContext.Allocate(graphics, new Rectangle(0, 0, 
                panel1.Size.Width, panel1.Size.Height));

            var p = new Pen(Color.Black);
            recWidth = panel1.Width / countX;
            recHeight = panel1.Height / countY;
            bufferedGraphics.Graphics.Clear(BackColor);
            for (var i = 0; i <= panel1.Width; i += recWidth)
            {
                for (var j = 0; j <= panel1.Height; j += recHeight)
                {
                    bufferedGraphics.Graphics.DrawRectangle(p, i, j, recWidth, recHeight);
                }
            }

            bufferedGraphics.Render();
        }

        private void StartButtonClick(object sender, EventArgs e)
        {
            game = new Game(bufferedGraphics, recWidth, recHeight);
            game.ScoreInForm += GameScoreInForm;
            game.Start();
            this.Focus();
            start_button.Enabled = false;
        }

        private void GameScoreInForm(object sender, EventArgs e)
        {
            if (e is ScoreEventArgs)
            {
                var scoreEventArgs = e as ScoreEventArgs;
                score.Text = scoreEventArgs.Score.ToString();
            }
        }

        private void Form1KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                case Keys.Left:
                    game.Moving(e.KeyCode);
                    break;
                case Keys.Up:
                    game.Rotation();
                    break;
                case Keys.Down:
                    game.RapidLowering(true);
                    break;
            }
        }

        private void Panel1Paint(object sender, PaintEventArgs e)
        {
            InitializeGraphics();
        }

        private void Form1KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
                game.RapidLowering(false);
        }
    }
}