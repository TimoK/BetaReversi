using Keras;
using Keras.Layers;
using Keras.Models;
using Numpy;
using ReversiNeuralNet.TrainingDataDefinition;
using System;
using System.Collections.Generic;
using System.IO;

namespace ReversiNeuralNet
{
    internal class ExpertPredictionModel
    {
        private const int MAX_ROWS_OUTPUT = 500;
        private const int BATCH_SIZE = 5;
        private const int VERBOSE = 1;

        internal BaseModel Model { get; set; }
        internal TrainingData TrainingData { get; set; }
        internal string TrainingDataFilename { get; set; }

        /// <summary>
        /// Train new model
        /// </summary>
        internal ExpertPredictionModel(string trainingDataFilename, int epochs)
        {
            TrainingData = new TrainingData(trainingDataFilename, GetInputSettings());
            TrainingDataFilename = trainingDataFilename;

            var model = new Sequential();
            model.Add(new Dense(Constants.BOARD_TILES * 6, activation: "relu", input_shape: new Shape(TrainingData.InputVectorLength)));
            model.Add(new Dense(Constants.BOARD_TILES * 12, activation: "sigmoid"));
            model.Add(new Dense(Constants.BOARD_TILES * 12, activation: "sigmoid"));
            model.Add(new Dense(64, activation: "softmax"));

            //Compile and train
            model.Compile(optimizer: "sgd", loss: "binary_crossentropy", metrics: new string[] { "accuracy" });
            model.Fit(np.array(TrainingData.inputBoardTrainData), np.array(TrainingData.outputPositionTrainData), batch_size: BATCH_SIZE, epochs: epochs, verbose: VERBOSE);

            var outputPositionTrainPredictions = model.Predict(np.array(TrainingData.inputBoardTrainData));
            Console.WriteLine(outputPositionTrainPredictions);

            model.Evaluate(np.array(TrainingData.inputBoardTestData), np.array(TrainingData.outputPositionTestData));

            Model = model;
        }

        private Dictionary<TrainingDataSetting, bool> GetInputSettings()
        {
			var inputSettings = new Dictionary<TrainingDataSetting, bool>
			{
				[TrainingDataSetting.EmptyBoardState] = true,
				[TrainingDataSetting.ActivePlayerBoardState] = true,
				[TrainingDataSetting.PassivePlayerBoardState] = true
			};
			return inputSettings;
        }

        /// <summary>
        /// Load existing model from files. Assumes data file used for training still exists and remains unchanged
        /// </summary>
        internal ExpertPredictionModel(string modelName)
        {
            Model = BaseModel.ModelFromJson(File.ReadAllText(Constants.FILE_PATH + modelName + ".json"));
            Model.LoadWeight(Constants.FILE_PATH + modelName + ".h5");
            TrainingDataFilename = File.ReadAllText(Constants.FILE_PATH + modelName + "_training_data_filename.txt");

            TrainingData = new TrainingData(TrainingDataFilename, GetInputSettings());
        }

        internal void SaveModel(string name)
        {
            string json = Model.ToJson();
            File.WriteAllText(Constants.FILE_PATH + name + ".json", json);
            Model.SaveWeight(Constants.FILE_PATH + name + ".h5");
            File.WriteAllText(Constants.FILE_PATH + name + "_training_data_filename.txt", TrainingDataFilename);
        }

        internal void SavePredicitonsToText(bool testData = true)
        {
            var outputPredictions = Model.Predict(testData ? TrainingData.inputBoardTestData : TrainingData.inputBoardTrainData);
            var outputPredictionsArray = outputPredictions.GetData<float>();

            using (var writer = new StreamWriter(Constants.FILE_PATH + "predictions.txt"))
            {
                var testSetSize = Math.Min(MAX_ROWS_OUTPUT, testData ? TrainingData.inputBoardTestData.GetLength(0) : TrainingData.inputBoardTrainData.GetLength(0));

                for (int i = 0; i < testSetSize; ++i)
                {
                    WriteArray(testData ? TrainingData.inputBoardTestData : TrainingData.inputBoardTrainData, i, writer);
                    WriteArray(testData ? TrainingData.outputPositionTestData : TrainingData.outputPositionTrainData, i, writer);
                    WriteArray(Convert(outputPredictionsArray, Constants.BOARD_TILES), i, writer);
                }
            }
        }

        internal static float[,] Convert(float[] singleDimensionArray, int dimSize)
        {
            var doubleDimensionArray = new float[singleDimensionArray.Length / dimSize, dimSize];
            for (int i = 0; i < singleDimensionArray.Length; ++i)
            {
                doubleDimensionArray[i / dimSize, i % dimSize] = singleDimensionArray[i];
            }
            return doubleDimensionArray;
        }

        private static void WriteArray(float[,] array, int index, StreamWriter writer)
        {
            for(int i = 0; i < array.GetLength(1); ++i)
            {
                writer.Write(array[index, i]);
                if(i != array.GetLength(1) - 1)
                {
                    writer.Write(" ");
                }
            }
            writer.WriteLine();
        }

    }
}
