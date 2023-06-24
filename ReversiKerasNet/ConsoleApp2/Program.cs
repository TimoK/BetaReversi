namespace ReversiNeuralNet
{
    class Program
    {
        static void Main(string[] args)
        {
            var expertPredictionModel = new ExpertPredictionModel();
            expertPredictionModel.TrainModel();
            expertPredictionModel.SavePredicitons(testData: true);
        }
    }
}
