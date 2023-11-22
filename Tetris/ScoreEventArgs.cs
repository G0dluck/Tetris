using System;

namespace Tetris
{
    public class ScoreEventArgs : EventArgs
    {
        public int Score { get; private set; }

        public ScoreEventArgs(int score)
        {
            this.Score = score;
        }
    }
}
