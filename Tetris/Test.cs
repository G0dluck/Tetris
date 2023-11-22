using System.IO;

namespace Tetris
{
    public class Test
    {
        public static void TestFigures(object figures)
        {
            ElementArray[,] arrFigure = figures as ElementArray[,];

            using (var sw = new StreamWriter(@"test.txt", true))
            {
                for (var i = 0; i < 20; i++)
                {
                    for (var j = 0; j < 10; j++)
                    {
                        sw.Write(arrFigure != null && arrFigure[i, j].Status ? "X " : "O ");
                    }

                    sw.WriteLine();
                }

                sw.WriteLine();
            }
        }
    }
}
