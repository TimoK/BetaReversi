using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ReversiNeuralNet.TrainingDataDefinition
{
    public class TrainingData
    {
        /// <summary>
        /// The fraction of data used for training, as opposed to testing
        /// </summary>
        private const double TRAINING_FRACTION = 0.8;

        internal float[,] inputBoardTrainData;
        internal float[,] outputPositionTrainData;

        internal float[,] inputBoardTestData;
        internal float[,] outputPositionTestData;

        internal int InputVectorLength { get; private set; }

        internal TrainingData(string filename, Dictionary<TrainingDataSetting, bool> inputSettingUsed)
        {
            var lines = File.ReadAllLines(Constants.FILE_PATH + filename);
            var numberOfTrainingLines = (int)(lines.Length * TRAINING_FRACTION);
            var trainingLines = lines.Take(numberOfTrainingLines).ToArray();
            var testLines = lines.Skip(numberOfTrainingLines).ToArray();

            var inputSetingStartPositions = GetInputSettingStartPositions(inputSettingUsed);

            (inputBoardTrainData, outputPositionTrainData) = GetInputAndOutput(trainingLines, inputSetingStartPositions);
            (inputBoardTestData, outputPositionTestData) = GetInputAndOutput(testLines, inputSetingStartPositions);
        }

        internal Dictionary<TrainingDataSetting, int?> GetInputSettingStartPositions(Dictionary<TrainingDataSetting, bool> inputSettingUsed)
        {
            var result = new Dictionary<TrainingDataSetting, int?>();
            var position = 0;

            foreach (var option in Enum.GetValues(typeof(TrainingDataSetting)).Cast<TrainingDataSetting>())
            {
                if (inputSettingUsed[option])
                {
                    result[option] = position;
                    // Assumption for now is that each option adds boardTiles amount of variables to the input vector
                    position += Constants.BOARD_TILES;
                }
                else
                {
                    result[option] = null;
                }
            }

            InputVectorLength = position;
            return result;
        }


        internal (float[,], float[,]) GetInputAndOutput(string[] lines, Dictionary<TrainingDataSetting, int?> inputSettingStartPositions)
        {
            var inputBoardDataArray = new float[lines.Length, InputVectorLength];
            var outputPositionDataArray = new float[lines.Length, Constants.BOARD_TILES];

            for (var lineIndex = 0; lineIndex < lines.Length; ++lineIndex)
            {
                var line = lines[lineIndex];
                var lineSplit = line.Split(' ');

                var boardState = lineSplit[0];
                var boardState2D = new BoardSquareState[Constants.BOARD_LENGTH, Constants.BOARD_LENGTH];

                var playerTurn = lineSplit[3];
                var blackIsActive = playerTurn[0] switch
                {
                    'b' => true,
                    'w' => false,
                    _ => throw new NotImplementedException("Player character not recognized")
                };
                var playerColor = blackIsActive ? PlayerColor.Black : PlayerColor.White;

                for (var boardIndex = 0; boardIndex < boardState.Length; ++boardIndex)
                {
                    var x = boardIndex % 8;
                    var y = boardIndex / 8;
                    var boardCharacter = boardState[boardIndex];

                    if (boardCharacter == 'e' && inputSettingStartPositions[TrainingDataSetting.EmptyBoardState].HasValue)
                    {
                        inputBoardDataArray[lineIndex, boardIndex + inputSettingStartPositions[TrainingDataSetting.EmptyBoardState].Value] = 1;
                    }
                    if ((blackIsActive && boardCharacter == 'b' ||
                        !blackIsActive && boardCharacter == 'w')
                        && inputSettingStartPositions[TrainingDataSetting.ActivePlayerBoardState].HasValue)
                    {
                        inputBoardDataArray[lineIndex, boardIndex + InputSettingStartPositions[TrainingDataSetting.ActivePlayerBoardState].Value] = 1;
                    }
                    if ((blackIsActive && boardCharacter == 'w' ||
                        !blackIsActive && boardCharacter == 'b')
                        && inputSettingStartPositions[TrainingDataSetting.PassivePlayerBoardState].HasValue)
                    {
                        inputBoardDataArray[lineIndex, boardIndex + InputSettingStartPositions[TrainingDataSetting.PassivePlayerBoardState].Value] = 1;

                    }
                    boardState2D[x, y] = boardCharacter == 'b' ? BoardSquareState.Black : (boardCharacter == 'w' ? BoardSquareState.White : BoardSquareState.Empty);
                }
                if(inputSettingStartPositions[TrainingDataSetting.LegalMoves].HasValue)
				{
                    var reversiBoard = new ReversiBoard(boardState2D, playerColor);
                    for (var boardIndex = 0; boardIndex < boardState.Length; ++boardIndex)
                    {
                        var x = boardIndex % 8;
                        var y = boardIndex / 8;
                        var legalMoveValue = reversiBoard.IsLegalMove(new Point(x, y), playerColor) ? 1 : 0;

                        inputBoardDataArray[lineIndex, boardIndex + InputSettingStartPositions[TrainingDataSetting.LegalMoves].Value] = legalMoveValue;
                    }
                }

                var playPosX = int.Parse(lineSplit[1]);
                var playPosY = int.Parse(lineSplit[2]);
                outputPositionDataArray[lineIndex, playPosX + playPosY * 8] = 1;
            }

            return (inputBoardDataArray, outputPositionDataArray);
        }
    }
}
