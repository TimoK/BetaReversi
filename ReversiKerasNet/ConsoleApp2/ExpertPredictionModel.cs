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
    internal class ExpertPredictionModel
    {
        private const int MAX_ROWS_OUTPUT = 500;


        internal Sequential Model { get; set; }
        internal TrainingData TrainingData { get; set; }

        internal void TrainModel()
        {
            TrainingData = new TrainingData();

            var model = new Sequential();
            model.Add(new Dense(64, activation: "relu", input_shape: new Shape(64)));
            model.Add(new Dense(256, activation: "sigmoid"));
            model.Add(new Dense(256, activation: "sigmoid"));
            model.Add(new Dense(64, activation: "softmax"));

            //Compile and train
            model.Compile(optimizer: "sgd", loss: "binary_crossentropy", metrics: new string[] { "accuracy" });
            model.Fit(np.array(TrainingData.inputBoardTrainData), np.array(TrainingData.outputPositionTrainData), batch_size: 100000, epochs: 100, verbose: 1);

            var outputPositionTrainPredictions = model.Predict(np.array(TrainingData.inputBoardTrainData));
            Console.WriteLine(outputPositionTrainPredictions);

            model.Evaluate(np.array(TrainingData.inputBoardTestData), np.array(TrainingData.outputPositionTestData));

            Model = model;
        }

        internal void SaveModel()
        {
            string json = Model.ToJson();
            File.WriteAllText("model.json", json);
            Model.SaveWeight("model.h5");
        }

        internal void LoadModel()
        {
            //Load model and weight
            Model = (Sequential)Sequential.ModelFromJson(File.ReadAllText("model.json"));
            Model.LoadWeight("model.h5");
        }

        internal void SavePredicitons(bool testData = true)
        {
            var outputPredictions = Model.Predict(testData ? TrainingData.inputBoardTestData : TrainingData.inputBoardTrainData);
            var outputPredictionsArray = outputPredictions.GetData<float>();

            using (var writer = new StreamWriter(TrainingData.FILE_PATH + "predictions.txt"))
            {
                var testSetSize = Math.Min(MAX_ROWS_OUTPUT, testData ? TrainingData.inputBoardTestData.GetLength(0) : TrainingData.inputBoardTrainData.GetLength(0));

                for (int i = 0; i < testSetSize; ++i)
                {
                    WriteArray(testData ? TrainingData.inputBoardTestData : TrainingData.inputBoardTrainData, i, writer);
                    WriteArray(testData ? TrainingData.outputPositionTestData : TrainingData.outputPositionTrainData, i, writer);
                    WriteArray(Convert(outputPredictionsArray, 64), i, writer);
                }
            }
        }

        internal float[,] Convert(float[] singleDimensionArray, int dimSize)
        {
            var doubleDimensionArray = new float[singleDimensionArray.Length / dimSize, dimSize];
            for (int i = 0; i < singleDimensionArray.Length; ++i)
            {
                doubleDimensionArray[i / dimSize, i % dimSize] = singleDimensionArray[i];
            }
            return doubleDimensionArray;
        }

        private void WriteArray(float[,] array, int index, StreamWriter writer)
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
