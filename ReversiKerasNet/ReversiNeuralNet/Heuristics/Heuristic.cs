namespace ReversiNeuralNet
{
    public interface Heuristic
    {
        double GetScore(ReversiBoard board, PlayerColor playerColor);
    }
}
