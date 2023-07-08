using System;
using System.Collections.Generic;
using System.Drawing;

namespace ReversiNeuralNet.AIs
{
    public interface AI
    {
        Point GetMove(ReversiBoard board, PlayerColor color);
    }

    public class SimpleAI : AI
    {
        public Point GetMove(ReversiBoard board, PlayerColor color)
        {
            List<Point> boardPositions = ReversiBoard.GetBoardPositions();
            foreach (Point boardPosition in boardPositions)
            {
                if (board.IsLegalMove(boardPosition, color))
                {
                    return boardPosition;
                }
            }
            throw new Exception("No legal moves available.");
        }
    }
}
