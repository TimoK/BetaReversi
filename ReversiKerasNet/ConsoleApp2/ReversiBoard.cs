using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiNeuralNet
{
	public class ReversiBoard
	{
		public int[,] BoardState { get; }

		private readonly List<(int, int)> directions = new()
		{
			(-1, 0),
			(-1, -1),
			(-1, 1),
			(0, -1),
			(0, 1),
			(1, -1),
			(1, 0),
			(1, 1),
		};

		public ReversiBoard(int[,] boardState)
		{
			BoardState = boardState;
		}

		public bool IsLegalMove(int x, int y)
		{

			if (BoardState[x, y] != 0)
				return false;
			foreach(var direction in directions)
			{
				var x_trans = x;
				var y_trans = y;

				var encounteredOpponent = false;
				while(true)
				{
					x_trans += direction.Item1;
					y_trans += direction.Item2;

					if( x_trans < 0 || x_trans >= Constants.BOARD_LENGTH ||
						y_trans < 0 || y_trans >= Constants.BOARD_LENGTH)
					{
						break;
					}

					if(BoardState[x_trans, y_trans] == -1)
					{
						encounteredOpponent = true;
					}
					if(BoardState[x_trans, y_trans] == 1)
					{
						if (encounteredOpponent)
							return true;
						break;
					}
					if(BoardState[x_trans, y_trans] == 0)
					{
						break;
					}
				}
			}
			return false;

		}
	}

}
