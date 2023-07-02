using ReversiNeuralNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ReversiAI
{
    public partial class ReversiVisualiser : Form
    {
        ReversiBoard board;
        ReversiGame game;

        public ReversiVisualiser(ReversiGame game)
        {
            game = game;
            board = game.board;
            boardLength = tileLength * Constants.BOARD_LENGTH;
            Size = new Size(offset.X + boardLength + 100, offset.Y + boardLength + 100);
            InitializeComponent();

            this.MouseClick += new System.Windows.Forms.MouseEventHandler(OnClick);
			game.BoardChanged += Game_BoardChanged;
        }

		private void Game_BoardChanged(object sender, EventArgs e)
		{
            Invalidate();
            Update();
		}

		Point offset = new Point(20, 50);
        int tileLength = 50;
        int boardLength;

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            Font drawFont = new System.Drawing.Font("Arial", 16);
            g.DrawString("Red score: " + board.WhiteScore, drawFont, Brushes.Black, new Point(0, 0));
            g.DrawString("Black score: " + board.BlackScore, drawFont, Brushes.Black, new Point(0, 20));

            for (int x = 0; x <= Constants.BOARD_LENGTH; ++x)
            {
                g.DrawLine(Pens.Black, new Point(offset.X, offset.Y + tileLength * x), new Point(offset.X + boardLength, offset.Y + tileLength * x));
                g.DrawLine(Pens.Black, new Point(offset.X + tileLength * x, offset.Y), new Point(offset.X + tileLength * x, offset.Y + boardLength));
            }
            List<Point> boardPositions = ReversiBoard.GetBoardPositions();
            foreach (Point boardPosition in boardPositions)
            {
                BoardSquareState boardSquareState = board.GetBoardState(boardPosition);
                Rectangle positionRectangle = new Rectangle(new Point(offset.X + tileLength * boardPosition.X, offset.Y + tileLength * boardPosition.Y), new Size(tileLength, tileLength));

                if (boardSquareState == BoardSquareState.Empty)
                {
                    if (board.IsLegalMove(boardPosition, board.CurrentPlayerColor))
                    {
                        g.DrawEllipse(Pens.Blue, positionRectangle);
                    }
                }
                else
                {
                    Brush playerColor = null;
                    if (boardSquareState == BoardSquareState.White) playerColor = Brushes.Red;
                    if (boardSquareState == BoardSquareState.Black) playerColor = Brushes.Black;
                    g.FillEllipse(playerColor, positionRectangle);
                }

            }


            if (board.GameEnded)
            {
                string gameEndedString = "";
                if (board.WhiteScore > board.BlackScore) gameEndedString = "Red has won.";
                else if (board.BlackScore > board.WhiteScore) gameEndedString = "Black has won.";
                else gameEndedString = "It's a tie.";
                g.DrawString(gameEndedString, drawFont, Brushes.Black, new Point(0, offset.Y + boardLength + 10));
            }

            base.OnPaint(e);
        }

        private void OnClick(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;
            Console.WriteLine(x);

            if (!(x < offset.X || y < offset.Y || x >= tileLength * Constants.BOARD_LENGTH + offset.X || y >= tileLength * Constants.BOARD_LENGTH + offset.Y))
            {
                game.MakeHumanMove(new Point((x - offset.X) / tileLength, (y - offset.Y) / tileLength));
            }
            Invalidate();
            base.OnClick(e);
        }
    }


}
