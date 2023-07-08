using System;
using System.Collections.Generic;
using System.Drawing;

namespace ReversiNeuralNet.Heuristics
{
    /* Did not want to design a heuristic myself (beyond scope of this project) so used a heuristic from someone else
        * Author: Kartikkukreja https://github.com/kartikkukreja
        * Code: https://github.com/kartikkukreja/blog-codes/blob/master/src/Heuristic%20Function%20for%20Reversi%20(Othello).cpp
        * Code ported to C# and my implementation of Reversi
        * Blogpost describing the heuristic: https://kartikkukreja.wordpress.com/2013/03/30/heuristic-function-for-reversiothello/ 
        */
    public class DynamicHeuristic : Heuristic
    {
        double[,] boardValuation;
        double[] scoreWeights;

        // Use the board valuation and score weights by Kartikkukreja as default
        double[] defaultScoreWeights = { 10, 801.724, 382.026, 78.922, 74.396, 10 };
        double[] defaultTileValuation = { 20, -3, 11, 8, -7, -4, 1, 2, 2, -3 };

        public DynamicHeuristic(double[] mirroredTileValuation = null, double[] scoreWeights = null)
        {
            if (mirroredTileValuation == null) mirroredTileValuation = defaultTileValuation;
            boardValuation = GetBoardValuation(mirroredTileValuation);

            if (scoreWeights == null) scoreWeights = defaultScoreWeights;
            this.scoreWeights = scoreWeights;
        }

        public static void WriteArray(int[,] array)
        {
            for (int y = 0; y < array.GetLength(0); ++y)
            {
                for (int x = 0; x < array.GetLength(1); ++x)
                {
                    Console.Write(array[x, y] + " ");
                }
                Console.WriteLine();
            }

        }

        public double GetScore(ReversiBoard board, PlayerColor playerColor)
        {
            PlayerColor opponentColor = ReversiBoard.OtherPlayerColor(playerColor);
            BoardSquareState playerTile = ReversiBoard.GetTile(playerColor);
            BoardSquareState opponentTile = ReversiBoard.GetTile(opponentColor);

            List<Point> boardLocations = ReversiBoard.GetBoardPositions();

            int my_tiles = 0, opp_tiles = 0, my_front_tiles = 0, opp_front_tiles = 0;
            double p = 0, c = 0, l = 0, m = 0, f = 0, d = 0;

            // Piece difference, frontier disks and disk squares
            foreach (Point location in boardLocations)
            {
                if (board.GetBoardState(location) == playerTile)
                {
                    d += boardValuation[location.X, location.Y];
                    my_tiles++;
                }
                else if (board.GetBoardState(location) == opponentTile)
                {
                    d -= boardValuation[location.X, location.Y];
                    opp_tiles++;
                }
                if (board.GetBoardState(location) != BoardSquareState.Empty)
                {
                    foreach (Point direction in board.directions)
                    {
                        Point adj_space = location; adj_space.Offset(direction);
                        if (!ReversiBoard.OutOfBounds(adj_space) && board.GetBoardState(adj_space) == BoardSquareState.Empty)
                        {
                            if (board.GetBoardState(location) == playerTile) my_front_tiles++;
                            else opp_front_tiles++;
                            break;
                        }
                    }
                }
            }
            if (my_tiles > opp_tiles)
                p = (100.0 * my_tiles) / (my_tiles + opp_tiles);
            else if (my_tiles < opp_tiles)
                p = -(100.0 * opp_tiles) / (my_tiles + opp_tiles);
            else p = 0;

            if (my_front_tiles > opp_front_tiles)
                f = -(100.0 * my_front_tiles) / (my_front_tiles + opp_front_tiles);
            else if (my_front_tiles < opp_front_tiles)
                f = (100.0 * opp_front_tiles) / (my_front_tiles + opp_front_tiles);
            else f = 0;

            // Corner occupancy
            int playerCorner = 0, opponentCorner = 0;
            if (board.GetBoardState(new Point(0, 0)) == playerTile) ++playerCorner;
            else if (board.GetBoardState(new Point(0, 0)) == opponentTile) ++opponentCorner;
            if (board.GetBoardState(new Point(7, 0)) == playerTile) ++playerCorner;
            else if (board.GetBoardState(new Point(7, 0)) == opponentTile) ++opponentCorner;
            if (board.GetBoardState(new Point(0, 7)) == playerTile) ++playerCorner;
            else if (board.GetBoardState(new Point(0, 7)) == opponentTile) ++opponentCorner;
            if (board.GetBoardState(new Point(7, 7)) == playerTile) ++playerCorner;
            else if (board.GetBoardState(new Point(7, 7)) == opponentTile) ++opponentCorner;
            c = 25 * (playerCorner - opponentCorner);

            // Corner closeness
            int playerCornerClose = 0, opponentCornerClose = 0;
            if (board.GetBoardState(new Point(0, 0)) == BoardSquareState.Empty)
            {
                if (board.GetBoardState(new Point(0, 1)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(0, 1)) == opponentTile) opponentCornerClose++;
                if (board.GetBoardState(new Point(1, 1)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(1, 1)) == opponentTile) opponentCornerClose++;
                if (board.GetBoardState(new Point(1, 0)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(1, 0)) == opponentTile) opponentCornerClose++;
            }
            if (board.GetBoardState(new Point(0, 7)) == BoardSquareState.Empty)
            {
                if (board.GetBoardState(new Point(0, 6)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(0, 6)) == opponentTile) opponentCornerClose++;
                if (board.GetBoardState(new Point(1, 6)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(1, 6)) == opponentTile) opponentCornerClose++;
                if (board.GetBoardState(new Point(1, 7)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(1, 7)) == opponentTile) opponentCornerClose++;
            }
            if (board.GetBoardState(new Point(7, 0)) == BoardSquareState.Empty)
            {
                if (board.GetBoardState(new Point(7, 1)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(7, 1)) == opponentTile) opponentCornerClose++;
                if (board.GetBoardState(new Point(6, 1)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(6, 1)) == opponentTile) opponentCornerClose++;
                if (board.GetBoardState(new Point(6, 0)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(6, 0)) == opponentTile) opponentCornerClose++;
            }
            if (board.GetBoardState(new Point(7, 7)) == BoardSquareState.Empty)
            {
                if (board.GetBoardState(new Point(6, 7)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(6, 7)) == opponentTile) opponentCornerClose++;
                if (board.GetBoardState(new Point(6, 6)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(6, 6)) == opponentTile) opponentCornerClose++;
                if (board.GetBoardState(new Point(7, 6)) == playerTile) playerCornerClose++;
                else if (board.GetBoardState(new Point(7, 6)) == opponentTile) opponentCornerClose++;
            }
            l = -12.5 * (playerCornerClose - opponentCornerClose);

            // Mobility
            int playerValidMoves = board.NumLegalMoves(playerColor);
            int opponentValidMoves = board.NumLegalMoves(opponentColor);
            if (playerValidMoves > opponentValidMoves)
                m = (100.0 * playerValidMoves) / (playerValidMoves + opponentValidMoves);
            else if (playerValidMoves < opponentValidMoves)
                m = -(100.0 * opponentValidMoves) / (playerValidMoves + opponentValidMoves);
            else m = 0;

            // final weighted score
            double score = (scoreWeights[0] * p) + (scoreWeights[1] * c) + (scoreWeights[2] * l)
                + (scoreWeights[3] * m) + (scoreWeights[4] * f) + (scoreWeights[5] * d);
            return score;
        }

        private double[,] GetBoardValuation(double[] mirroredTileValuations)
        {
            double[,] boardValuation = new double[Constants.BOARD_LENGTH, Constants.BOARD_LENGTH];
            int mirroredTileValuationsIndex = 0;
            int halfSize = Constants.BOARD_LENGTH / 2;

            if (mirroredTileValuations.Length != ((halfSize + 1) * halfSize) / 2)
            {
                Console.WriteLine(((Constants.BOARD_LENGTH + 1) * Constants.BOARD_LENGTH) / 2);
                throw new ArgumentException("Not the right amount of valuations to fill the board.");
            }

            // Fill in one quarter of the board
            for (int x = 0; x < 4; ++x)
            {
                for (int y = x; y < 4; ++y)
                {
                    boardValuation[x, y] = mirroredTileValuations[mirroredTileValuationsIndex];
                    boardValuation[y, x] = mirroredTileValuations[mirroredTileValuationsIndex];
                    ++mirroredTileValuationsIndex;
                }
            }
            // Fill in the rest of the board
            for (int x = 0; x < 4; ++x)
            {
                for (int y = 0; y < 4; ++y)
                {
                    boardValuation[7 - x, y] = boardValuation[x, y];
                    boardValuation[x, 7 - y] = boardValuation[x, y];
                    boardValuation[7 - x, 7 - y] = boardValuation[x, y];
                }
            }
            return boardValuation;
        }
    }
}
