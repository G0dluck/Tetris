using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class ScoreEventArgs : EventArgs
    {
        public int Score { get; private set; }
        public ScoreEventArgs(int score)
        {
            this.Score = score;
        }
    }
}
