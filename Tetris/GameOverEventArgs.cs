using System;

namespace Tetris
{
    public class GameOverEventArgs : EventArgs
    {
        public int Score { get; }

        public bool IsHighScore { get; }

        public GameOverEventArgs(int score, bool isHighScore)
        {
            Score = score;
            IsHighScore = isHighScore;
        }
    }
}