using System;
using System.Collections.Generic;
using System.Linq;

public class HandScoresTable
{
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
    public static IReadOnlyDictionary<string, int> Ranks { get; }

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
        Ranks = GetScoreProbabilities();
    }

    private static Dictionary<string, int> GetScoreProbabilities()
    {
        Dictionary<string, int> ret = new Dictionary<string, int>();
        int rank = 0;
        foreach (KeyValuePair<string, HandScore> entry in All)
        {
            ret.Add(entry.Key, rank);
            rank++;
        }
        return ret;
    }
}
