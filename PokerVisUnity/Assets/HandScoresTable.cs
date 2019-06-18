using System.Collections.Generic;
using System.Linq;

public class HandScoresTable
{
    public const int TotalPossibleHands = 2598960;

    public IReadOnlyList<HandScore> StraightFlushes { get; }
    public IReadOnlyList<HandScore> FourOfAKinds { get; }
    public IReadOnlyList<HandScore> FullHouses { get; }
    public IReadOnlyList<HandScore> Flushes { get; }
    public IReadOnlyList<HandScore> Straights { get; }
    public IReadOnlyList<HandScore> ThreeOfAKinds { get; }
    public IReadOnlyList<HandScore> TwoPairs { get; }
    public IReadOnlyList<HandScore> Pairs { get; }
    public IReadOnlyList<HandScore> HighCards { get; }

    public IReadOnlyDictionary<string, HandScore> All { get; }
    public IReadOnlyDictionary<string, ScoreProbabilities> Probabilities { get; }

    public HandScoresTable()
    {
        StraightFlushes = HandScore.GetStraightFlushes().ToList().AsReadOnly();
        FourOfAKinds = HandScore.GetFourOfAKinds().ToList().AsReadOnly();
        FullHouses = HandScore.GetFullHouses().ToList().AsReadOnly();
        Flushes = HandScore.GetFlushes().ToList().AsReadOnly();
        Straights = HandScore.GetStraights().ToList().AsReadOnly();
        ThreeOfAKinds = HandScore.GetThreeOfAKinds().ToList().AsReadOnly();
        TwoPairs = HandScore.GetTwoPairs().ToList().AsReadOnly();
        Pairs = HandScore.GetPairs().ToList().AsReadOnly();
        HighCards = HandScore.GetHighCards().ToList().AsReadOnly();

        List<HandScore> all = new List<HandScore>();
        all.AddRange(StraightFlushes);
        all.AddRange(FourOfAKinds);
        all.AddRange(FullHouses);
        all.AddRange(Flushes);
        all.AddRange(Straights);
        all.AddRange(ThreeOfAKinds);
        all.AddRange(TwoPairs);
        all.AddRange(Pairs);
        all.AddRange(HighCards);
        All = all.ToDictionary(item => item.Key, item => item);
        Probabilities = GetScoreProbabilities();
    }

    private Dictionary<string, ScoreProbabilities> GetScoreProbabilities()
    {
        Dictionary<string, ScoreProbabilities> ret = new Dictionary<string, ScoreProbabilities>();
        int betterHands = 0;
        int worseHands = TotalPossibleHands;
        foreach (KeyValuePair<string, HandScore> entry in All)
        {
            worseHands -= entry.Value.Repeats;
            ScoreProbabilities probabilities = new ScoreProbabilities(betterHands, worseHands);
            ret.Add(entry.Key, probabilities);
            betterHands += entry.Value.Repeats;
        }
        return ret;
    }
}
