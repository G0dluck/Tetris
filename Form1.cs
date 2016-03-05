using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        Game game;

        Graphics graphics;
        BufferedGraphicsContext bufferedGraphicsContext;
        BufferedGraphics bufferedGraphics;

        int rec_Width;
        int rec_Height;
        const int count_X = 10, count_Y = 20;

        private void InitializeGraphics()
        {
            graphics = Graphics.FromHwnd(panel1.Handle);
            bufferedGraphicsContext = new BufferedGraphicsContext();
            bufferedGraphics = bufferedGraphicsContext.Allocate(graphics, new Rectangle(0, 0, 
                panel1.Size.Width, panel1.Size.Height));

            Pen p = new Pen(Color.Black);
            rec_Width = panel1.Width / count_X;
            rec_Height = panel1.Height / count_Y;
            bufferedGraphics.Graphics.Clear(BackColor);
            for (int i = 0; i <= panel1.Width; i += rec_Width)
                for (int j = 0; j <= panel1.Height; j += rec_Height)
                {
                    bufferedGraphics.Graphics.DrawRectangle(p, i, j, rec_Width, rec_Height);
                }

            bufferedGraphics.Render();
        }

        private void start_button_Click(object sender, EventArgs e)
        {
            game = new Game(bufferedGraphics, rec_Width, rec_Height);
            game.ScoreInForm += game_ScoreInForm;
            game.Start();
            this.Focus();
            start_button.Enabled = false;
        }

        void game_ScoreInForm(object sender, EventArgs e)
        {
            if (e is ScoreEventArgs)
            {
                ScoreEventArgs scoreEventArgs = e as ScoreEventArgs;
                score.Text = scoreEventArgs.Score.ToString();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Left)
                game.Moving(e.KeyCode);

            if (e.KeyCode == Keys.Up)
                game.Rotation();

            if (e.KeyCode == Keys.Down)
                game.RapidLowering(true);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            InitializeGraphics();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
                game.RapidLowering(false);
        }

    }
}
