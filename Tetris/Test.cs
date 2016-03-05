using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Tetris
{
    class Test
    {
        public static void TestFigures(object figures)
        {
            ElementArray[,] arrFigure = figures as ElementArray[,];

            using (StreamWriter sw = new StreamWriter(@"test.txt", true))
            {
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (arrFigure[i, j].Status)
                        {
                            sw.Write("X ");
                        }
                        else
                        {
                            sw.Write("O ");
                        }
                    }
                    sw.WriteLine();
                }
                sw.WriteLine();
            }
        }
    }
}
