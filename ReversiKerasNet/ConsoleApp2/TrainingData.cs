using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Keras;
using Keras.Datasets;
using Keras.Layers;
using Keras.Models;
using Keras.Utils;
using Numpy;

namespace ReversiNeuralNet
{
    public class TrainingData
    {
        private const int BOARD_LENGTH = 64;
        private const double TRAINING_FRACTION = 0.8;

        public const string FILE_PATH = "..\\..\\..\\..\\..\\Gamefiles\\";

        internal float[,] inputBoardTrainData;
        internal float[,] outputPositionTrainData;

        internal float[,] inputBoardTestData;
        internal float[,] outputPositionTestData;

        int testLines;

        internal TrainingData()
        {
            var lines = File.ReadAllLines(FILE_PATH + "neuralNetworkInputs1999.txt");
            var numberOfTrainingLines = (int)(lines.Length * TRAINING_FRACTION);
            var trainingLines = lines.Take(numberOfTrainingLines).ToArray();
            var testLines = lines.Skip(numberOfTrainingLines).ToArray();

            (inputBoardTrainData, outputPositionTrainData) = GetInputAndOutput(trainingLines);
            (inputBoardTestData, outputPositionTestData) = GetInputAndOutput(testLines);
        }

        internal (float[,], float[,]) GetInputAndOutput(string[] lines)
        {
            var inputBoardDataArray = new float[lines.Length, BOARD_LENGTH];
            var outputPositionDataArray = new float[lines.Length, BOARD_LENGTH];

            for (var lineIndex = 0; lineIndex < lines.Length; ++lineIndex)
            {
                var line = lines[lineIndex];
                var lineSplit = line.Split(' ');

                var boardState = lineSplit[0];

                var playerTurn = lineSplit[3];
                var blackIsActive = playerTurn[0] switch
                {
                    'b' => true,
                    'w' => false,
                    _ => throw new NotImplementedException("Player character not recognized")
                };

                for (var boardIndex = 0; boardIndex < boardState.Length; ++boardIndex)
                {
                    var boardCharacter = boardState[boardIndex];

                    var boardPositionNumber = boardCharacter switch
                    {
                        'e' => 0.5f,
                        'b' => blackIsActive ? 1f : 0f,
                        'w' => blackIsActive ? 0f : 1f,
                        _ => throw new NotImplementedException("Board character not recognized")
                    };

                    inputBoardDataArray[lineIndex, boardIndex] = boardPositionNumber;
                }

                var playPosX = int.Parse(lineSplit[1]);
                var playPosY = int.Parse(lineSplit[2]);
                outputPositionDataArray[lineIndex, playPosX + playPosY * 8] = 1;
            }

            return (inputBoardDataArray, outputPositionDataArray);
        }
    }
}
