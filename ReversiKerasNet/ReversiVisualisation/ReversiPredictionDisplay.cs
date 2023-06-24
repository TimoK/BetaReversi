using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ReversiNeuralNet;

namespace ReversiVisualisation
{
    public partial class ReversiPredictionDisplay : Form
    {
        private const int OFFSET = 10;
        private const int GRID_SIZE = 8;
        private const int TILE_SIZE = 40;

        List<PredictionDataPoint> PredictionsDataPoints = new();
        private int currentPredictionCounter = 0;

        public ReversiPredictionDisplay()
        {
            LoadData();

            this.Paint += ReversiPredictionDisplay_Paint;
            this.KeyPress += ReversiPredictionDisplay_KeyPress;

            InitializeComponent();
        }

        private void ReversiPredictionDisplay_KeyPress(object? sender, KeyPressEventArgs e)
        {
            var keyChar = (int)e.KeyChar;

            if(keyChar == 97) // 'a'
            {
                currentPredictionCounter = Math.Max(0, currentPredictionCounter - 1);
            }
            if (keyChar == 100) // 'd'
            {
                currentPredictionCounter = Math.Min(PredictionsDataPoints.Count - 1, currentPredictionCounter + 1);
            }
            Invalidate();
        }

        private void ReversiPredictionDisplay_Paint(object? sender, PaintEventArgs e)
        {
            var borderPen = Pens.Black;
            var activePlayerPen = new Pen(Color.Blue, 5);
            var passivePlayerPen = new Pen(Color.Red, 5);
            var actualmovePen = new Pen(Color.Green, 5);
            var predictionPen = new Pen(Color.Purple, 3);


            var graphics = e.Graphics;

            for (int i = 1; i < GRID_SIZE; ++i)
            {
                var variablePosition = OFFSET + (i * TILE_SIZE);

                graphics.DrawLine(borderPen, variablePosition, OFFSET, variablePosition, OFFSET + (TILE_SIZE * GRID_SIZE));
                graphics.DrawLine(borderPen, OFFSET, variablePosition, OFFSET + (TILE_SIZE * GRID_SIZE), variablePosition);
            }

            var currentPrediction = PredictionsDataPoints[currentPredictionCounter];
            for(int i = 0; i < currentPrediction.boardState.Length; ++i)
            {
                bool paintBlack;
                if(currentPrediction.boardState[i] < 0.25)
                {
                    paintBlack = false;
                }
                else if(currentPrediction.boardState[i] > 0.75)
                {
                    paintBlack = true;
                }
                else
                {
                    continue;
                }
                DrawTile(graphics, paintBlack ? activePlayerPen : passivePlayerPen, i);
            }
            DrawTile(graphics, actualmovePen, (int)Math.Round(currentPrediction.actualMove));
            DrawTile(graphics, predictionPen, (int)Math.Round(currentPrediction.prediction1));
            DrawTile(graphics, predictionPen, (int)Math.Round(currentPrediction.prediction2));
            DrawTile(graphics, predictionPen, (int)Math.Round(currentPrediction.prediction3));

        }

        private void DrawTile(Graphics g, Pen pen, int position)
        {
            var (x, y) = MapTo2D(position);
            g.DrawEllipse(pen, OFFSET + (x * TILE_SIZE), OFFSET + (y * TILE_SIZE), TILE_SIZE, TILE_SIZE);
        }

        private void LoadData()
        {
            var filepath = TrainingData.FILE_PATH + "predictions.txt";
            var lines = File.ReadAllLines(filepath);
            for (int i = 0; i < lines.Length; i += 3)
            {
                var predictionDataPoint = new PredictionDataPoint();
                predictionDataPoint.boardState = ReadString(lines[i]);
                var actualMoveArray = ReadString(lines[i + 1]);
                for (var j = 0; j < actualMoveArray.Length; j++)
                {
                    if (actualMoveArray[j] == 1)
                    {
                        predictionDataPoint.actualMove = j;
                        break;
                    }
                }
                var predictions = ReadString(lines[i + 2]);
                var predictionsDict = new Dictionary<int, float>();
                for(var j = 0; j < predictions.Length; ++j)
                {
                    predictionsDict[j] = predictions[j];
                }

                var top3 = predictionsDict.OrderByDescending(x => x.Value).Take(3).ToList();
                predictionDataPoint.predictions = predictions;
                predictionDataPoint.prediction1 = top3[0].Key;
                predictionDataPoint.prediction2 = top3[1].Key;
                predictionDataPoint.prediction3 = top3[2].Key;

                PredictionsDataPoints.Add(predictionDataPoint);
            }
        }

        private (int, int) MapTo2D(int coordinate)
        {
            return (coordinate % GRID_SIZE, coordinate / GRID_SIZE);
        }

        private float[] ReadString(string s)
        {
            var split = s.Split(' ');
            var result = new float[split.Length];
            for (int i = 0; i < split.Length; i++)
            {
                if(!string.IsNullOrWhiteSpace(split[i]))
                    result[i] = float.Parse(split[i]);
            }
            return result;
        }

        private class PredictionDataPoint
        {
            internal float[] boardState;
            internal float actualMove;
            internal float[] predictions;
            internal float prediction1;
            internal float prediction2;
            internal float prediction3;
        }
    }
}
