using System.Drawing;

namespace Tetris
{
    public struct Cell
    {
        public bool IsOccupied { get; set; }

        public Brush Brush { get; set; }
    }
}