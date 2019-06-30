public class ScoreProbabilities
{
    public int BetterHandsCount { get; }
    public int Rank { get; }
    public int WorseHandsCount { get; }

    public ScoreProbabilities(int betterHandsCount, int worseHandsCount, int rank)
    {
        Rank = rank;
        BetterHandsCount = betterHandsCount;
        WorseHandsCount = worseHandsCount;
    }
}
