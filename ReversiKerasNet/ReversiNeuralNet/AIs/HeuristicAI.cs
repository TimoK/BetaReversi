using System.Collections.Generic;
using System.Drawing;

namespace ReversiNeuralNet.AIs
{
    public class HeuristicAI : AI
    {
        private readonly Heuristic heuristic;

        public HeuristicAI(Heuristic heuristic)
        {
            this.heuristic = heuristic;
        }

        public Point GetMove(ReversiBoard board, PlayerColor color)
        {
            Point bestMove = new Point(0, 0);
            double bestScore = double.MinValue;

            List<Point> boardPositions = ReversiBoard.GetBoardPositions();
            foreach (Point boardPosition in boardPositions)
            {
                if (board.IsLegalMove(boardPosition, color))
                {
                    ReversiBoard boardWithMove = board.Copy();
                    boardWithMove.MakeMove(boardPosition);
                    double score = heuristic.GetScore(boardWithMove, color);
                    if (score >= bestScore)
                    {
                        bestScore = score;
                        bestMove = boardPosition;
                    }
                }
            }
            return bestMove;
        }
    }
}
