namespace ReversiNeuralNet
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

        internal TrainingData(string filename)
        {
            var lines = File.ReadAllLines(Constants.FILE_PATH + filename);
            var numberOfTrainingLines = (int)(lines.Length * TRAINING_FRACTION);
            var trainingLines = lines.Take(numberOfTrainingLines).ToArray();
            var testLines = lines.Skip(numberOfTrainingLines).ToArray();

            (inputBoardTrainData, outputPositionTrainData) = GetInputAndOutput(trainingLines);
            (inputBoardTestData, outputPositionTestData) = GetInputAndOutput(testLines);
        }

        internal (float[,], float[,]) GetInputAndOutput(string[] lines)
        {
            var inputBoardDataArray = new float[lines.Length, Constants.BOARD_TILES * 3];
            var outputPositionDataArray = new float[lines.Length, Constants.BOARD_TILES];

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

                    var adaptValueIndex = boardCharacter switch
                    {
                        'e' => boardIndex,
                        'b' => blackIsActive ? boardIndex + Constants.BOARD_TILES : boardIndex + Constants.BOARD_TILES * 2,
                        'w' => blackIsActive ? boardIndex + Constants.BOARD_TILES * 2 : boardIndex + Constants.BOARD_TILES,
                        _ => throw new NotImplementedException("Board character not recognized")
                    };

                    inputBoardDataArray[lineIndex, adaptValueIndex] = 1;
                }

                var playPosX = int.Parse(lineSplit[1]);
                var playPosY = int.Parse(lineSplit[2]);
                outputPositionDataArray[lineIndex, playPosX + playPosY * 8] = 1;
            }

            return (inputBoardDataArray, outputPositionDataArray);
        }
    }
}
