using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiNeuralNet
{
    public class ReversiBoard
    {
        public BoardSquareState[,] Board = new BoardSquareState[8, 8];
        private PlayerColor currentPlayerColor = PlayerColor.White;
        public List<Point> directions;

        int whiteScore = 2;
        int blackScore = 2;
        bool gameEnded = false;

        public int WhiteScore
        {
            get
            {
                return whiteScore;
            }
        }

        public int BlackScore
        {
            get
            {
                return blackScore;
            }
        }

        public int GetScore(PlayerColor color)
        {
            if (color == PlayerColor.White) return WhiteScore;
            return BlackScore;
        }

        public bool GameEnded
        {
            get
            {
                return gameEnded;
            }
        }

        public PlayerColor CurrentPlayerColor
        {
            get
            {
                return currentPlayerColor;
            }
        }

        public ReversiBoard()
        {
            StartUpBoard();
            directions = MakeDirections();
        }

        public ReversiBoard(BoardSquareState[,] board, PlayerColor currentPlayerColor)
		{
            Board = board;
            this.currentPlayerColor = currentPlayerColor;
            directions = MakeDirections();
        }

        private void StartUpBoard()
        {
            // First empty the board, then put the starting pieces on the board
            EmptyBoard();
            Board[3, 3] = BoardSquareState.White;
            Board[4, 3] = BoardSquareState.Black;
            Board[3, 4] = BoardSquareState.Black;
            Board[4, 4] = BoardSquareState.White;
            // Make white the first player to move
            currentPlayerColor = PlayerColor.White;
        }

        private void EmptyBoard()
        {
            // Make all the tiles empty
            for (int x = 0; x < Constants.BOARD_LENGTH; ++x)
                for (int y = 0; y < Constants.BOARD_LENGTH; ++y)
                    Board[x, y] = BoardSquareState.Empty;
        }

        public void MakeMove(Point move)
        {
            if (OutOfBounds(move))
            {
                throw new ArgumentException("Move position out of board bounds.");
            }
            if (Board[move.X, move.Y] != BoardSquareState.Empty)
            {
                throw new ArgumentException("Boardmove not possible, position not empty.");
            }
            if (!IsLegalMove(move, currentPlayerColor))
            {
                return;
            }

            PlayerColor opponentPlayerColor = OtherPlayerColor(currentPlayerColor);
            // Assign the tile that the player picked to the player
            Board[move.X, move.Y] = GetTile(currentPlayerColor);
            IncreasePlayerScore(currentPlayerColor, 1);
            // Go over into each direction, if a own tile is discovered change all opponent tiles inbetween to own
            foreach (Point direction in directions)
            {
                Point position = move;
                List<Point> opponentTilesToChange = new List<Point>();
                while (true)
                {
                    position.Offset(direction);
                    if (OutOfBounds(position))
                    {
                        break;
                    }
                    if (Board[position.X, position.Y] == BoardSquareState.Empty) 
                    {
						break;
					}
                    if (Board[position.X, position.Y] == GetTile(opponentPlayerColor)) 
                    {
						opponentTilesToChange.Add(position);
					}
                    if (Board[position.X, position.Y] == GetTile(currentPlayerColor))
                    {
                        foreach (var opponentTileToChange in opponentTilesToChange)
                        {
                            Board[opponentTileToChange.X, opponentTileToChange.Y] = GetTile(currentPlayerColor);
                        }
                        IncreasePlayerScore(currentPlayerColor, opponentTilesToChange.Count);
                        IncreasePlayerScore(opponentPlayerColor, -opponentTilesToChange.Count);
                        break;
                    }
                }
            }

            //Pass the turn
            PlayerColor previousPlayerColor = currentPlayerColor;
            currentPlayerColor = opponentPlayerColor;
            if (CurrentPlayerHasNoLegalMoves())
            {
                currentPlayerColor = previousPlayerColor;
                if (CurrentPlayerHasNoLegalMoves())
                {
                    gameEnded = true;
                }
            }
        }

        private void IncreasePlayerScore(PlayerColor playerColor, int amount)
        {
            if (playerColor == PlayerColor.White)
            {
                whiteScore += amount;
            }
            else
            {
                blackScore += amount;
            }
        }

        private bool CurrentPlayerHasNoLegalMoves()
        {
            List<Point> boardPositions = GetBoardPositions();
            foreach (Point boardPosition in boardPositions)
            {
                if (IsLegalMove(boardPosition, currentPlayerColor))
                {
                    return false;
                }
            }
            return true;
        }

        public ReversiBoard Copy()
        {
            ReversiBoard copy = new ReversiBoard();
            copy.CopyBoard(this);
            return copy;
        }

        public void CopyBoard(ReversiBoard toCopy)
        {
            blackScore = toCopy.BlackScore;
            whiteScore = toCopy.WhiteScore;
            gameEnded = toCopy.GameEnded;
            currentPlayerColor = toCopy.CurrentPlayerColor;
            foreach (Point position in GetBoardPositions())
            {
                Board[position.X, position.Y] = toCopy.GetBoardState(position);
            }
        }

        public int NumLegalMoves(PlayerColor playerColor)
        {
            int numLegalMoves = 0;
            foreach (Point position in GetBoardPositions())
            {
                if (IsLegalMove(position, playerColor)) ++numLegalMoves;
            }
            return numLegalMoves;
        }

        public bool IsLegalMove(Point move, PlayerColor playerColor)
        {
            if (OutOfBounds(move))
            {
                return false;
            }
            if (Board[move.X, move.Y] != BoardSquareState.Empty)
            {
                return false;
            }

            BoardSquareState movingPlayerTile = GetTile(playerColor);
            BoardSquareState opponentTile = GetTile(OtherPlayerColor(playerColor));

            foreach (Point direction in directions)
            {
                Point position = move;
                bool encounteredOpponent = false;
                while (true)
                {
                    position.Offset(direction);
                    if (OutOfBounds(position))
                    {
                        break;
                    }
                    if (Board[position.X, position.Y] == BoardSquareState.Empty)
                    {
                        break;
                    }
                    if (Board[position.X, position.Y] == opponentTile)
                    {
                        encounteredOpponent = true;
                    }
                    if (Board[position.X, position.Y] == movingPlayerTile)
                    {
                        if (encounteredOpponent)
                        {
                            return true;
                        }
                        break;
                    }
                }
            }

            return false;
        }

        public static bool OutOfBounds(Point position)
        {
            return (position.X < 0 || position.Y < 0 ||
                position.X >= Constants.BOARD_LENGTH || position.Y >= Constants.BOARD_LENGTH);
        }

        public BoardSquareState GetBoardState(Point position)
        {
            return Board[position.X, position.Y];
        }

        public static BoardSquareState GetTile(PlayerColor playerColor)
        {
            return playerColor == PlayerColor.Black ? BoardSquareState.Black : BoardSquareState.White;
        }

        public static PlayerColor OtherPlayerColor(PlayerColor playerColor)
        {
            return playerColor == PlayerColor.Black ? PlayerColor.White : PlayerColor.Black;
        }

        private static List<Point> MakeDirections()
        {
            List<Point> directions = new List<Point>();
            for (int x = -1; x <= 1; ++x)
                for (int y = -1; y <= 1; ++y)
                {
                    if (x == 0 & y == 0) continue;
					var direction = new Point
					{
						X = x,
						Y = y
					};
					directions.Add(direction);
                }
            return directions;
        }

        public static List<Point> GetBoardPositions()
        {
            List<Point> boardPositions = new List<Point>();
            for (int x = 0; x < Constants.BOARD_LENGTH; ++x)
                for (int y = 0; y < Constants.BOARD_LENGTH; ++y)
                {
                    boardPositions.Add(new Point(x, y));
                }
            return boardPositions;
        }

    }

    public enum PlayerColor { Black, White }
    public enum BoardSquareState { Black, White, Empty }
}
