﻿namespace ReversiNeuralNet
{
    class Program
    {
        static void Main(string[] args)
        {
            var expertPredictionModel = new ExpertPredictionModel("neuralNetworkInputs1999.txt", 15);
            //expertPredictionModel.SaveModel("simpleModel");

            //var expertPredictionModel = new ExpertPredictionModel("simpleModel");
            expertPredictionModel.SavePredicitonsToText(testData: true);
        }
    }
}
