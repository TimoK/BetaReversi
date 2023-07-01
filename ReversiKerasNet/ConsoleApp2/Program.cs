namespace ReversiNeuralNet
{
    class Program
    {
        static void Main(string[] args)
        {
            var expertPredictionModel = new ExpertPredictionModel("neuralNetworkInputs1999.txt", 50);
            expertPredictionModel.SaveModel("simpleModelWithLegals");

            //var expertPredictionModel = new ExpertPredictionModel("simpleModel");
            expertPredictionModel.SavePredicitonsToText(testData: true);
        }
    }
}
