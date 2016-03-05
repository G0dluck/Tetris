using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tetris
{
    class Game
    {
        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        Random random = new Random();

        ElementArray[,] arrFigure = new ElementArray[count_Row, count_Col];
        bool[,] arrFigureTemp = new bool[count_Row, count_Col];

        int rec_Width;
        int rec_Height;
        const int count_Col = 10, count_Row = 20;
        int score;

        Point[] points = new Point[4];
        Point[] past_Points = new Point[4];

        IFigure figure;

        BufferedGraphics bufferedGraphics;
        Brush brush;

        public event EventHandler ScoreInForm;

        public Game(BufferedGraphics bufferedGraphics, int rec_Width, int rec_Height)
        {
            timer.Interval = 500;
            timer.Tick += timer_Tick;

            this.bufferedGraphics = bufferedGraphics;
            this.rec_Width = rec_Width;
            this.rec_Height = rec_Height;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            arrFigureTemp[past_Points[0].Y, past_Points[0].X] = false;
            arrFigureTemp[past_Points[1].Y, past_Points[1].X] = false;
            arrFigureTemp[past_Points[2].Y, past_Points[2].X] = false;
            arrFigureTemp[past_Points[3].Y, past_Points[3].X] = false;

            if (points[3].X != past_Points[3].X || OutOfTheY(points) 
                && !CheckBelowPoints(figure.GetLowPoints(past_Points)))
            {
                if (points[3].X == past_Points[3].X)
                {
                    points[0].Y++;
                    points[1].Y++;
                    points[2].Y++;
                    points[3].Y++;
                }

                arrFigureTemp[points[0].Y, points[0].X] = true;
                arrFigureTemp[points[1].Y, points[1].X] = true;
                arrFigureTemp[points[2].Y, points[2].X] = true;
                arrFigureTemp[points[3].Y, points[3].X] = true;

                repaint(past_Points, points);

                for (int i = 0; i < points.Length; i++)
                {
                    past_Points[i] = points[i];
                }
            }
            else
            {
                timer.Stop();
                for (int i = 0; i < past_Points.Length; i++)
                {
                    arrFigure[past_Points[i].Y, past_Points[i].X].Status = true;
                    arrFigure[past_Points[i].Y, past_Points[i].X].Brush = brush;
                }
                score += 10;
                OnScoreInForm(new ScoreEventArgs(score));

                CheckLines(past_Points);
                /*Thread t = new Thread(Test.TestFigures);
                t.Start(arrFigure);*/
                Start();
            }
        }

        private bool CheckTheEndOfTheGame(Point[] point)
        {
            for (int k = 0; k < point.Length; k++)
            {
                if (arrFigure[point[k].Y, point[k].X].Status)
                {
                    string text;
                    if (Properties.Settings.Default.HighScore < score)
                    {
                        Properties.Settings.Default.HighScore = score;
                        Properties.Settings.Default.Save();
                        text = "NEW HIGHSCORE!!! \n" + "Your results: " + score + " scores";
                    }
                    else
                    {
                        text = "Your results: " + score + " scores \n" + 
                            "HighScore: " + Properties.Settings.Default.HighScore + " scores";
                    }
                    System.Windows.Forms.MessageBox.Show(text, "Game Over!", 
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

                    for (int i = 0; i < count_Row; i++)
                    {
                        for (int j = 0; j < count_Col; j++)
                        {
                            if (arrFigure[i, j].Status)
                            {
                                bufferedGraphics.Graphics.FillRectangle(SystemBrushes.Control,
                                    j * rec_Width + 2, i * rec_Height + 2, rec_Width - 3, rec_Height - 3);
                            }
                        }
                    }
                    bufferedGraphics.Render();
                    return true;
                }
            }
            return false;
        }

        public void OnScoreInForm(ScoreEventArgs e)
        {
            EventHandler scoreInFrom = ScoreInForm;
            if (scoreInFrom != null)
                scoreInFrom(this, e);
        }

        private void CheckLines(Point[] point)
        {
            int max = point[0].Y;
            int min = point[0].Y;
            int numberLine = 0;

            for (int i = 1; i < point.Length; i++)
            {
                if (point[i].Y > max)
                    max = point[i].Y;

                if (point[i].Y < min)
                    min = point[i].Y;
            }

            for (int i = max; i >= min; i--)
            {
                bool removeTheLine = false;
                for (int j = 0; j < count_Col; j++)
                {
                    if (!arrFigure[i, j].Status)
                    {
                        removeTheLine = false;
                        break;
                    }
                    removeTheLine = true;
                }
                if (removeTheLine)
                {
                    numberLine = i;
                    for (int j = 0; j < count_Col; j++)
                    {
                        arrFigure[i, j].Status = false;
                        bufferedGraphics.Graphics.FillRectangle(SystemBrushes.Control,
                            j * rec_Width + 2, i * rec_Height + 2, rec_Width - 3, rec_Height - 3);
                    }
                    bufferedGraphics.Render();
                    DownwardShift(numberLine);
                    i++;
                    min++;
                    score += 100;
                    OnScoreInForm(new ScoreEventArgs(score));
                }
            }       
        }

        private void DownwardShift(int numberLine)
        {
            while (numberLine > 0)
            {
                for (int i = 0; i < count_Col; i++)
                {
                    if (arrFigure[numberLine - 1, i].Status)
                    {
                        arrFigure[numberLine, i].Status = true;
                        arrFigure[numberLine, i].Brush = arrFigure[numberLine - 1, i].Brush;
                        bufferedGraphics.Graphics.FillRectangle(arrFigure[numberLine - 1, i].Brush,
                        i * rec_Width + 2, numberLine * rec_Height + 2, rec_Width - 3, rec_Height - 3);
                    }

                    if (!arrFigure[numberLine - 1, i].Status && arrFigure[numberLine, i].Status)
                    {
                        arrFigure[numberLine, i].Status = false;
                        arrFigure[numberLine, i].Brush = SystemBrushes.Control;
                        bufferedGraphics.Graphics.FillRectangle(SystemBrushes.Control,
                                i * rec_Width + 2, numberLine * rec_Height + 2, rec_Width - 3, rec_Height - 3);
                    }
                }
                bufferedGraphics.Render();

                if (numberLine - 1 == 0)
                    return;
                numberLine--;
                int countElements = 0;
                for (int i = 0; i < count_Col; i++)
                {
                    if (arrFigure[numberLine - 1, i].Status)
                        break;
                    countElements++;
                }

                if (countElements == count_Col)
                {
                    for (int i = 0; i < count_Col; i++)
                    {
                        arrFigure[numberLine, i].Status = false;
                        arrFigure[numberLine, i].Brush = SystemBrushes.Control;
                        bufferedGraphics.Graphics.FillRectangle(SystemBrushes.Control,
                                i * rec_Width + 2, numberLine * rec_Height + 2, rec_Width - 3, rec_Height - 3);
                    }
                    bufferedGraphics.Render();
                    return;
                }
            }
        }

        private bool CheckBelowPoints(Point[] point)
        {
            foreach (var p in points)
            {
                if (arrFigure[p.Y + 1, p.X ].Status)
                    return true;
            }
            return false;
        }

        private void repaint(Point[] pastPoints, Point[] currentPoints)
        {
            System.Collections.IEnumerator past = pastPoints.GetEnumerator();
            System.Collections.IEnumerator current = currentPoints.GetEnumerator();

            while (past.MoveNext())
            {
                Point point = (Point)past.Current;

                bufferedGraphics.Graphics.FillRectangle(SystemBrushes.Control, 
                    point.X * rec_Width + 2, point.Y * rec_Height + 2, rec_Width - 3, rec_Height - 3);
                //bufferedGraphics.Graphics.DrawRectangle(new Pen(Color.Black), point.X * rec_Width, point.Y * rec_Height, rec_Width, rec_Height);
            }

            while (current.MoveNext())
            {
                Point point = (Point)current.Current;

                bufferedGraphics.Graphics.FillRectangle(brush, 
                    point.X * rec_Width + 2, point.Y * rec_Height + 2, rec_Width - 3, rec_Height - 3);
            }

            bufferedGraphics.Render();
        }

        public void Start()
        {
            int random_figure = random.Next(0, 7);

            switch (random_figure)
            {
                case 0:
                    figure = new Figure_Square(count_Col, random.Next(0, count_Col - 1));
                    brush = Brushes.DarkRed;
                    break;
                case 1:
                    figure = new Figure_Line(count_Col, random.Next(0, count_Col - 1), random.Next(0, 2));
                    brush = Brushes.Green;
                    break;
                case 2:
                    figure = new Figure_T(count_Col, random.Next(0, count_Col - 1), random.Next(0, 4));
                    brush = Brushes.DarkViolet;
                    break;
                case 3:
                    figure = new Figure_J(count_Col, random.Next(0, count_Col - 1), random.Next(0, 4));
                    brush = Brushes.YellowGreen;
                    break;
                case 4:
                    figure = new Figure_L(count_Col, random.Next(0, count_Col - 1), random.Next(0, 4));
                    brush = Brushes.Blue;
                    break;
                case 5:
                    figure = new Figure_S(count_Col, random.Next(0, count_Col - 1), random.Next(0, 4));
                    brush = Brushes.Orange;
                    break;
                case 6:
                    figure = new Figure_Z(count_Col, random.Next(0, count_Col - 1), random.Next(0, 4));
                    brush = Brushes.Coral;
                    break;
                default:
                    break;
            }

            if (CheckTheEndOfTheGame(figure.GetPints()))
                return;

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = figure.GetPints()[i];
                past_Points[i] = figure.GetPints()[i];
            }

            arrFigureTemp[points[0].Y, points[0].X] = true;
            arrFigureTemp[points[1].Y, points[1].X] = true;
            arrFigureTemp[points[2].Y, points[2].X] = true;
            arrFigureTemp[points[3].Y, points[3].X] = true;

            repaint(past_Points, points);

            timer.Start();
        }

        public void Moving(System.Windows.Forms.Keys e)
        {
            if (e == System.Windows.Forms.Keys.Right && OutOfTheXRight(points)
                && !CheckTheRightPoints(figure.GetTheRightPoints(past_Points)))
            {
                //timer.Stop();
                points[0].X++;
                points[1].X++;
                points[2].X++;
                points[3].X++;
                timer_Tick(new object(), new EventArgs());
                //repaint();
                //timer.Start();
            }

            if (e == System.Windows.Forms.Keys.Left && OutOfTheXLeft(points)
                && !CheckTheLeftPoints(figure.GetTheLeftPoints(past_Points)))
            {
                //timer.Stop();
                points[0].X--;
                points[1].X--;
                points[2].X--;
                points[3].X--;
                timer_Tick(new object(), new EventArgs());
                //repaint();
                //timer.Start();
            }
        }

        public void RapidLowering(bool e)
        {
            if (e)
            {
                timer.Interval = 75;
            }
            else
            {
                timer.Interval = 500;
            }
        }

        private bool CheckTheRightPoints(Point[] point)
        {
            foreach (var p in point)
            {
                if (arrFigure[p.Y , p.X + 1].Status)
                    return true;
            }
            return false;
        }

        private bool CheckTheLeftPoints(Point[] point)
        {
            foreach (var p in point)
            {
                if (arrFigure[p.Y, p.X - 1].Status)
                    return true;
            }
            return false;
        }

        private bool OutOfTheY(Point[] point)
        {
            if (point[0].Y < count_Row - 1 && point[1].Y < count_Row - 1 &&
                point[2].Y < count_Row - 1 && point[3].Y < count_Row - 1 &&
                point[0].Y >= 0 && point[1].Y >= 0 &&
                point[2].Y >= 0 && point[3].Y >= 0)
            {
                return true;
            }
            return false;
        }

        private bool OutOfTheXRight(Point[] point)
        {
            if (point[0].X < count_Col - 1 && point[1].X < count_Col - 1 &&
                point[2].X < count_Col - 1 && point[3].X < count_Col - 1)
            {
                return true;
            }
            return false;
        }

        private bool OutOfTheXLeft(Point[] point)
        {        
            if (point[0].X > 0 && point[1].X > 0 &&
                point[2].X > 0 && point[3].X > 0)
            {
                return true;
            }
            return false;
        }

        public void Rotation()
        {
            if (figure is Figure_Square)
                return;

            Point[] pointsTemp = figure.Rotation(points);
            if (OutOfTheY(pointsTemp) && AbilityToRotate(pointsTemp) && 
                !CheckBelowPoints(figure.GetLowPoints(pointsTemp)))
            {
                points = pointsTemp;
                timer_Tick(new object(), new EventArgs());
            }
        }

        private bool AbilityToRotate(Point[] point)
        {
            for (int i = 0; i < point.Length; i++)
            {
                if (point[i].X >= count_Col)
                {
                    return false;
                }

                if (point[i].X < 0)
                {
                    return false;
                }

                if (arrFigure[point[i].Y, point[i].X].Status == true)
                    return false;
            }
            return true;
        }
    }
}
