using System;
using System.Collections.Generic;
using System.Linq;

public static class HandScoresTable
{
    public const int TotalPossibleHands = 2598960;

    public static IReadOnlyList<HandScore> StraightFlushes { get; }
    public static IReadOnlyList<HandScore> FourOfAKinds { get; }
    public static IReadOnlyList<HandScore> FullHouses { get; }
    public static IReadOnlyList<HandScore> Flushes { get; }
    public static IReadOnlyList<HandScore> Straights { get; }
    public static IReadOnlyList<HandScore> ThreeOfAKinds { get; }
    public static IReadOnlyList<HandScore> TwoPairs { get; }
    public static IReadOnlyList<HandScore> Pairs { get; }

    public static IReadOnlyList<HandScore> HighCards { get; }

    public static IReadOnlyDictionary<string, HandScore> All { get; }
    public static IReadOnlyDictionary<string, ScoreProbabilities> Probabilities { get; }

    static HandScoresTable()
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

    private static Dictionary<string, ScoreProbabilities> GetScoreProbabilities()
    {
        Dictionary<string, ScoreProbabilities> ret = new Dictionary<string, ScoreProbabilities>();
        int betterHands = 0;
        int worseHands = TotalPossibleHands;
        int rank = 0;
        foreach (KeyValuePair<string, HandScore> entry in All)
        {
            worseHands -= entry.Value.Repeats;
            ScoreProbabilities probabilities = new ScoreProbabilities(betterHands, worseHands, rank);
            rank++;
            ret.Add(entry.Key, probabilities);
            betterHands += entry.Value.Repeats;
        }
        return ret;
    }
}
