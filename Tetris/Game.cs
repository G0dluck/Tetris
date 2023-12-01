using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Tetris
{
    public class Game
    {
        private readonly Timer timer = new Timer();
        private readonly Random random = new Random();

        private readonly ElementArray[,] arrFigure = new ElementArray[countRow, countCol];

        private readonly int recWidth;
        private readonly int recHeight;
        private const int countCol = 10, countRow = 20;
        private int score;

        private readonly Point[] points = new Point[4];
        private readonly Point[] pastPoints = new Point[4];

        private IFigure currentFigure;
        private IFigure nextFigure;

        private readonly BufferedGraphics gameBufferedGraphics;
        private readonly BufferedGraphics nextBufferedGraphics;

        private readonly Action GameEnd;

        public event EventHandler ScoreInForm;

        public Game(BufferedGraphics gameBufferedGraphics, BufferedGraphics nextBufferedGraphics, int rec_Width, int rec_Height, Action endFn)
        {
            timer.Interval = 500;
            timer.Tick += TimerTick;

            this.gameBufferedGraphics = gameBufferedGraphics;
            this.nextBufferedGraphics = nextBufferedGraphics;
            this.recWidth = rec_Width;
            this.recHeight = rec_Height;
            this.GameEnd = endFn;
        }

        public void TimerTick(object sender, EventArgs e)
        {
            if (points[3].X != pastPoints[3].X || OutOfY(points) && 
                !CheckBelowPoints(currentFigure.GetLowPoints(pastPoints)))
            {
                if (points[3].X == pastPoints[3].X)
                {
                    points[0].Y++;
                    points[1].Y++;
                    points[2].Y++;
                    points[3].Y++;
                }

                RepaintGame();

                for (var i = 0; i < points.Length; i++)
                {
                    pastPoints[i] = points[i];
                }
            }
            else
            {
                StartNewFigure();
            }
        }

        private void StartNewFigure()
        {
            timer.Stop();
            foreach (var p in pastPoints)
            {
                arrFigure[p.Y, p.X].Status = true;
                arrFigure[p.Y, p.X].Brush = currentFigure.Brush;
            }

            score += 10;
            OnScoreInForm(new ScoreEventArgs(score));

            CheckLines(pastPoints);
            /*Thread t = new Thread(Test.TestFigures);
            t.Start(arrFigure);*/
            Start();
        }

        private bool CheckEndOfGame(Point[] point)
        {
            if (point.Any(p => arrFigure[p.Y, p.X].Status))
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

                MessageBox.Show(text, "Game Over!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                for (var i = 0; i < countRow; i++)
                {
                    for (var j = 0; j < countCol; j++)
                    {
                        if (arrFigure[i, j].Status)
                        {
                            gameBufferedGraphics.Graphics.FillRectangle(SystemBrushes.Control,
                                j * recWidth + 2, i * recHeight + 2, recWidth - 3, recHeight - 3);
                        }
                    }
                }

                Repaint(nextFigure.GetPoints(), nextBufferedGraphics, SystemBrushes.Control);

                gameBufferedGraphics.Render();
                nextBufferedGraphics.Render();
                GameEnd();
                return true;
            }

            return false;
        }

        public void OnScoreInForm(ScoreEventArgs e)
        {
            var scoreInFrom = ScoreInForm;
            scoreInFrom?.Invoke(this, e);
        }

        private void CheckLines(Point[] point)
        {
            var pointY = point.Select(x => x.Y).ToArray();
            var max = pointY.Max();
            var min = pointY.Min();

            for (var i = max; i >= min; i--)
            {
                var removeLine = false;
                for (var j = 0; j < countCol; j++)
                {
                    if (!arrFigure[i, j].Status)
                    {
                        removeLine = false;
                        break;
                    }

                    removeLine = true;
                }

                if (removeLine)
                {
                    var numberLine = i;
                    for (var j = 0; j < countCol; j++)
                    {
                        arrFigure[i, j].Status = false;
                        gameBufferedGraphics.Graphics.FillRectangle(SystemBrushes.Control,
                            j * recWidth + 2, i * recHeight + 2, recWidth - 3, recHeight - 3);
                    }

                    gameBufferedGraphics.Render();
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
                for (var i = 0; i < countCol; i++)
                {
                    if (arrFigure[numberLine - 1, i].Status)
                    {
                        arrFigure[numberLine, i].Status = true;
                        arrFigure[numberLine, i].Brush = arrFigure[numberLine - 1, i].Brush;
                        gameBufferedGraphics.Graphics.FillRectangle(arrFigure[numberLine - 1, i].Brush,
                        i * recWidth + 2, numberLine * recHeight + 2, recWidth - 3, recHeight - 3);
                    }

                    if (!arrFigure[numberLine - 1, i].Status && arrFigure[numberLine, i].Status)
                    {
                        arrFigure[numberLine, i].Status = false;
                        arrFigure[numberLine, i].Brush = SystemBrushes.Control;
                        gameBufferedGraphics.Graphics.FillRectangle(SystemBrushes.Control,
                                i * recWidth + 2, numberLine * recHeight + 2, recWidth - 3, recHeight - 3);
                    }
                }

                gameBufferedGraphics.Render();

                if (numberLine - 1 == 0)
                {
                    return;
                }

                numberLine--;
                var countElements = 0;
                for (var i = 0; i < countCol; i++)
                {
                    if (arrFigure[numberLine - 1, i].Status)
                    {
                        break;
                    }

                    countElements++;
                }

                if (countElements == countCol)
                {
                    for (var i = 0; i < countCol; i++)
                    {
                        arrFigure[numberLine, i].Status = false;
                        arrFigure[numberLine, i].Brush = SystemBrushes.Control;
                        gameBufferedGraphics.Graphics.FillRectangle(SystemBrushes.Control,
                                i * recWidth + 2, numberLine * recHeight + 2, recWidth - 3, recHeight - 3);
                    }

                    gameBufferedGraphics.Render();
                    return;
                }
            }
        }

        private bool CheckBelowPoints(Point[] point)
        {
            return point.Any(p => arrFigure[p.Y + 1, p.X].Status);
        }

        private void RepaintGame()
        {
            Repaint(pastPoints, gameBufferedGraphics, SystemBrushes.Control);
            Repaint(points, gameBufferedGraphics, currentFigure.Brush);
            gameBufferedGraphics.Render();
        }

        private void RepaintNext(Point[] currentPoints)
        {
            Repaint(currentPoints, nextBufferedGraphics, SystemBrushes.Control);
            Repaint(nextFigure.GetPoints(), nextBufferedGraphics, nextFigure.Brush);
            nextBufferedGraphics.Render();
        }

        private void Repaint(Point[] currentPoints, BufferedGraphics bufferedGraphics, Brush brush)
        {
            if (currentPoints != null)
            {
                foreach (var point in currentPoints)
                {
                    bufferedGraphics.Graphics.FillRectangle(brush, 
                        point.X * recWidth + 2, point.Y * recHeight + 2, recWidth - 3, recHeight - 3);
                }
            }
        }

        private IFigure GetFigure()
        {
            var randomfigure = random.Next(0, 7);
            IFigure figure;
            switch (randomfigure)
            {
                case 0:
                    figure = new FigureSquare(countCol, random.Next(0, countCol - 1));
                    break;
                case 1:
                    figure = new FigureLine(countCol, random.Next(0, countCol - 1), random.Next(0, 2));
                    break;
                case 2:
                    figure = new FigureT(countCol, random.Next(0, countCol - 1), random.Next(0, 4));
                    break;
                case 3:
                    figure = new FigureJ(countCol, random.Next(0, countCol - 1), random.Next(0, 4));
                    break;
                case 4:
                    figure = new FigureL(countCol, random.Next(0, countCol - 1), random.Next(0, 4));
                    break;
                case 5:
                    figure = new FigureS(countCol, random.Next(0, countCol - 1), random.Next(0, 4));
                    break;
                case 6:
                    figure = new FigureZ(countCol, random.Next(0, countCol - 1), random.Next(0, 4));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return figure;
        }

        public void Start()
        {
            var currentPoints = nextFigure?.GetPoints();
            currentFigure = nextFigure ?? GetFigure();
            nextFigure = GetFigure();

            RepaintNext(currentPoints);

            var figurePoints = currentFigure.CorrectPoints();
            if (CheckEndOfGame(figurePoints))
            {
                return;
            }

            for (var i = 0; i < points.Length; i++)
            {
                points[i] = figurePoints[i];
                pastPoints[i] = figurePoints[i];
            }

            RepaintGame();

            timer.Start();
        }

        public void Moving(Keys e)
        {
            switch(e)
            {
                case Keys.Right when OutOfXRight(points)
                                     && !CheckRightPoints(currentFigure.GetRightPoints(pastPoints)):
                    //timer.Stop();
                    points[0].X++;
                    points[1].X++;
                    points[2].X++;
                    points[3].X++;
                    TimerTick(new object(), EventArgs.Empty);
                    //repaint();
                    //timer.Start();
                    break;
                case Keys.Left when OutOfXLeft(points)
                                    && !CheckLeftPoints(currentFigure.GetLeftPoints(pastPoints)):
                    //timer.Stop();
                    points[0].X--;
                    points[1].X--;
                    points[2].X--;
                    points[3].X--;
                    TimerTick(new object(), EventArgs.Empty);
                    //repaint();
                    //timer.Start();
                    break;
            }
        }

        public void RapidLowering(bool e)
        {
            timer.Interval = e ? 50 : 500;
        }

        private bool CheckRightPoints(Point[] point)
        {
            return point.Any(p => arrFigure[p.Y, p.X + 1].Status);
        }

        private bool CheckLeftPoints(Point[] point)
        {
            return point.Any(p => arrFigure[p.Y, p.X - 1].Status);
        }

        private bool OutOfY(Point[] point)
        {
            return point[0].Y < countRow - 1 && point[1].Y < countRow - 1 &&
                   point[2].Y < countRow - 1 && point[3].Y < countRow - 1 &&
                   point[0].Y >= 0 && point[1].Y >= 0 &&
                   point[2].Y >= 0 && point[3].Y >= 0;
        }

        private bool OutOfXRight(Point[] point)
        {
            return point[0].X < countCol - 1 && point[1].X < countCol - 1 &&
                   point[2].X < countCol - 1 && point[3].X < countCol - 1;
        }

        private bool OutOfXLeft(Point[] point)
        {
            return point[0].X > 0 && point[1].X > 0 &&
                   point[2].X > 0 && point[3].X > 0;
        }

        public void Rotation()
        {
            if (currentFigure is FigureSquare)
            {
                return;
            }

            var pointsTemp = currentFigure.Rotation(points);
            if (OutOfY(pointsTemp) && AbilityToRotate(pointsTemp) && 
                !CheckBelowPoints(currentFigure.GetLowPoints(pointsTemp)))
            {
                for (var i = 0; i < pointsTemp.Length; i++)
                {
                    points[i] = pointsTemp[i];
                }

                TimerTick(new object(), EventArgs.Empty);
            }
            else
            {
                currentFigure.RevertModeRotation();
            }
        }

        private bool AbilityToRotate(Point[] point)
        {
            return point.All(p => p.X < countCol && p.X >= 0 && !arrFigure[p.Y, p.X].Status);
        }
    }
}