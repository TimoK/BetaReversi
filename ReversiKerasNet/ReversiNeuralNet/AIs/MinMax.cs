using System;
using System.Collections.Generic;
using System.Drawing;

namespace ReversiNeuralNet.AIs
{
    // Implementation of the MinMax algorithm with optional alpha beta pruning
    public class MinMax : AI
    {
        private readonly Heuristic heuristic;
        private readonly int searchdepth;
		private readonly bool alphaBetaPruning;

        public MinMax(Heuristic heuristic, int searchdepth, bool alphaBetaPruning)
        {
            this.heuristic = heuristic;
            this.searchdepth = searchdepth;
            this.alphaBetaPruning = alphaBetaPruning;
            if (searchdepth < 1)
            {
                throw new ArgumentException("Search depth can not be smaller than 1.");
            }
        }

        public Point GetMove(ReversiBoard board, PlayerColor color)
        {
            Point bestMove = new Point(-1, -1);
            double bestScore = double.MinValue;
            double alpha = double.MinValue;

            List<Point> boardPositions = ReversiBoard.GetBoardPositions();
            foreach (Point boardPosition in boardPositions)
            {
                if (board.IsLegalMove(boardPosition, color))
                {
                    ReversiBoard boardWithMove = board.Copy();
                    boardWithMove.MakeMove(boardPosition);
                    double score = GetBoardValuation(boardWithMove, color, searchdepth - 1, alpha, double.MaxValue);
                    if (score >= bestScore)
                    {
                        bestScore = score;
                        bestMove = boardPosition;
                    }
                    if (score > alpha)
                    {
                        alpha = score;
                    }
                }
            }


            return bestMove;
        }

        private double GetBoardValuation(ReversiBoard board, PlayerColor optimisingColor, int currentSearchDepth, double alpha, double beta)
        {
            if (currentSearchDepth == 0) return heuristic.GetScore(board, optimisingColor);

            bool maximising = false;
            if (board.CurrentPlayerColor == optimisingColor)
            {
				maximising = true;
			}

            Point bestMove;
            double bestScore = maximising ? double.MinValue : double.MaxValue;

            PlayerColor currentPlayerColor = ReversiBoard.OtherPlayerColor(optimisingColor);
            if (maximising)
            {
				currentPlayerColor = optimisingColor;
			}

            if (board.GameEnded)
            {
                int optimisingScore = board.GetScore(optimisingColor);
                int opponentScore = board.GetScore(ReversiBoard.OtherPlayerColor(optimisingColor));
                if (optimisingScore > opponentScore)
                {
                    return double.MaxValue;
                }
                if (opponentScore > optimisingScore)
                {
                    return double.MinValue;
                }
                return 0;
            }

            var boardPositions = ReversiBoard.GetBoardPositions();
            foreach (Point boardPosition in boardPositions)
            {
                if (board.IsLegalMove(boardPosition, currentPlayerColor))
                {
                    ReversiBoard boardWithMove = board.Copy();
                    boardWithMove.MakeMove(boardPosition);
                    var score = GetBoardValuation(boardWithMove, optimisingColor, currentSearchDepth - 1, alpha, beta);
                    if (maximising)
                    {
                        if (score >= bestScore)
                        {
                            bestScore = score;
                            bestMove = boardPosition;
                        }
                        if (score > alpha)
                        {
                            alpha = score;
                        }
                    }
                    else
                    {
                        if (score <= bestScore)
                        {
                            bestScore = score;
                            bestMove = boardPosition;
                        }
                        if (score < beta)
                        {
                            beta = score;
                        }
                    }
                    // Cut-off point for alpha-beta pruning
                    // In this part of the searchtree (towards the root) we have seen an option for the opponent (beta) that is worth less
                    // than an option for the player (alpha). An option down this search tree will either be worth more than beta so discarded by the opponent,
                    // (and/)or it will be worth less than alpha so be discarded by the player
                    if (alphaBetaPruning && beta < alpha)
                    {
                        break;
                    }
                }
            }
            return bestScore;
        }
    }
}
