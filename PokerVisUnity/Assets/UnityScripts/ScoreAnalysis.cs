using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class ScoreAnalysisTable
{
    public const int TotalPossibleHands = 133784560; //133,784,560 possible combinations of 7 out of 52
    public const int UniqueSevenCardHands = 4824;

    private readonly ReadOnlyCollection<ScoreAnalysis> allScoreAnalysis;
    private readonly ReadOnlyCollection<int> fiveToSevenCardRerankings;

    public ScoreAnalysis GetScoreAnalysis(Hand hand)
    {
        return allScoreAnalysis[hand.Rank];
    }
    public int GetSevenCardRanking(Hand hand)
    {
        return fiveToSevenCardRerankings[hand.Rank];
    }

    public ScoreAnalysisTable(TextAsset frequencyTableData)
    {
        allScoreAnalysis = GetAnalysisTable(frequencyTableData);
        fiveToSevenCardRerankings = GetFiveToSevenCardRerankings();
    }

    private ReadOnlyCollection<int> GetFiveToSevenCardRerankings()
    {
        List<int> ret = new List<int>(allScoreAnalysis.Count);
        int sevenCardRank = 0;
        for (int i = 0; i < allScoreAnalysis.Count; i++)
        {
            ret.Add(sevenCardRank);
            ScoreAnalysis currentItem = allScoreAnalysis[i];
            if(currentItem.Frequency != 0)
            {
                sevenCardRank++;
            }
        }
        return ret.AsReadOnly();
    }

    public static ReadOnlyCollection<ScoreAnalysis> GetAnalysisTable(TextAsset frequencyTableData)
    {
        string[] lines = frequencyTableData.text.Trim().Split('\n');
        ScoreAnalysis[] ret = new ScoreAnalysis[lines.Length];

        int betterHandsCount = 0;
        int worseHandsCount = TotalPossibleHands;

        for (int i = 0; i < lines.Length; i++)
        {
            int frequency = Convert.ToInt32(lines[i]);

            worseHandsCount -= frequency;
            ScoreAnalysis analysis = new ScoreAnalysis(betterHandsCount, worseHandsCount, i, frequency);
            ret[i] = analysis;
            betterHandsCount += frequency;
        }
        return ret.ToList().AsReadOnly();
    }
}

public class ScoreAnalysis
{
    public int BetterHandsCount { get; }
    public int WorseHandsCount { get; }
    public int Rank { get; }
    public int Frequency { get; }

    public ScoreAnalysis(int betterHandsCount, int worseHandsCount, int rank, int frequency)
    {
        Rank = rank;
        BetterHandsCount = betterHandsCount;
        WorseHandsCount = worseHandsCount;
        Frequency = frequency;
    }
}
