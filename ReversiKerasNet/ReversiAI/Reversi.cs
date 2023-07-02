using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using ReversiNeuralNet;

namespace ReversiAI
{
    public class ReversiGame
    {
        public ReversiBoard board = new();

        bool player1_human, player2_human;
        AI ai1, ai2;

        public event EventHandler BoardChanged;

        public ReversiGame()
        {
            double[] evoScoreWeights = { 9.9464677320544, 663.725223387073, 397.310830727661, 18.3461926578452, 41.8220717677244, 100.265637983259 };

            // Hardcoded for now, can later add it to initialize options or input it in console
            player1_human = false;
            player2_human = false;
            ai1 = new MinMax(new DynamicHeuristic(), 3, true);
            ai2 = new NeuralNetworkAI();
            //ai2 = new MinMax(new DynamicHeuristic(scoreWeights: evoScoreWeights), 3, true);

            if (!player1_human) player1Move();
        }

        public ReversiGame(AI ai1, AI ai2)
        {
            this.ai1 = ai1;
            this.ai2 = ai2;

            player1Move();
        }

        private void player1Move()
        {
            if (player1_human) return;
            MakeMove(ai1.GetMove(board, PlayerColor.White));
        }

        private void player2Move()
        {
            if (player2_human) return;
            MakeMove(ai2.GetMove(board, PlayerColor.Black));
        }

        private void currentPlayerMove()
        {
            if (board.CurrentPlayerColor == PlayerColor.White) player1Move();
            else player2Move();
        }

        public void MakeHumanMove(Point move)
        {
            if (board.CurrentPlayerColor == PlayerColor.White && !player1_human) return;
            if (board.CurrentPlayerColor == PlayerColor.Black && !player2_human) return;
            if (!board.IsLegalMove(move, board.CurrentPlayerColor)) return;

            MakeMove(move);
        }

        public void MakeMove(Point move)
        {
            if (board.GameEnded) return;

            if (!board.IsLegalMove(move, board.CurrentPlayerColor))
            {
                throw new Exception("Illegal move was made.");
            }
            board.MakeMove(move);
            BoardChanged?.Invoke(this, new EventArgs());
            //Nice for human vs fast ai games, to first see impact of own move before direct counter move
            //Thread.Sleep(2000);

            if (!board.GameEnded)
            {
                currentPlayerMove();
            }
        }
    }
}
