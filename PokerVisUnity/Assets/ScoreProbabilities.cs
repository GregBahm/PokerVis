public class ScoreProbabilities
{
    public int BetterHandsCount { get; }
    public int WorseHandsCount { get; }

    public ScoreProbabilities(int betterHandsCount, int worseHandsCount)
    {
        BetterHandsCount = betterHandsCount;
        WorseHandsCount = worseHandsCount;
    }
}
