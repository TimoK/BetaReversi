namespace ReversiNeuralNet
{
    class Program
    {
        static void Main(string[] args)
        {
            //var expertPredictionModel = new ExpertPredictionModel("neuralNetworkInputsSmall.txt", 100);
            //expertPredictionModel.SaveModel("simpleModel");

            var expertPredictionModel = new ExpertPredictionModel("simpleModel");
            expertPredictionModel.SavePredicitonsToText(testData: true);
        }
    }
}
