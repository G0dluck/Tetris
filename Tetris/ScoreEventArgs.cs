using System;

namespace Tetris
{
    public class ScoreEventArgs : EventArgs
    {
        public int Score { get; }

        public ScoreEventArgs(int score)
        {
            Score = score;
        }
    }
}
