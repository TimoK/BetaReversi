using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ReversiNeuralNet.AIs
{
	public class NeuralNetworkAI : AI
	{
		public ExpertPredictionModel ExpertPredictionModel { get; }

		public NeuralNetworkAI()
		{
			ExpertPredictionModel = new ExpertPredictionModel("simpleModelWithLegals");
		}

		public Point GetMove(ReversiBoard board, PlayerColor color)
		{
			var inputStringBuilder = new StringBuilder();
			foreach(var boardPos in board.Board)
			{
				var boardChar = boardPos switch
				{
					BoardSquareState.White => 'w',
					BoardSquareState.Black => 'b',
					_ => 'e'
				};
				inputStringBuilder.Append(boardChar);
			}
			// There is no actual move to input, so this will be ignored when predicting
			inputStringBuilder.Append(" 1 1 ");

			inputStringBuilder.Append(color == PlayerColor.Black ? 'b' : 'w');

			var (inputData, _) = ExpertPredictionModel.TrainingData.GetInputAndOutput(new string[] { inputStringBuilder.ToString() });
			var inputData1D = new float[inputData.GetLength(1)];
			for (int i = 0; i < inputData1D.Length; ++i)
			{
				inputData1D[i] = inputData[0, i];
			}

			var prediction = ExpertPredictionModel.GetPrediction(inputData);

			var predictionsDict = new Dictionary<int, float>();
			for (var j = 0; j < prediction.Length; ++j)
			{
				predictionsDict[j] = prediction[j];
			}

			foreach (var position in predictionsDict.OrderByDescending(x => x.Value))
			{
				var x = position.Key % 8;
				var y = position.Key / 8;
				var point = new Point(x, y);
				if (board.IsLegalMove(point, color))
				{
					return point;
				}
			}
			throw new KeyNotFoundException("Unable to find legal move");
		}
	}
}
