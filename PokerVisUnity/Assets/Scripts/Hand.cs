using System;
using System.Collections.Generic;
using System.Linq;

public class Hand : IComparable<Hand>
{
    public IReadOnlyList<Card> Cards { get; }

    public int StraightFlushValue { get; }
    public int FourOfAKindValue { get; }
    public int FullHouseThreeCardValue { get; }
    public int FullHouseTwoCardValue { get; }
    public bool IsFlush { get; }
    public int StraightValue { get; }
    public int ThreeOfAKindValue { get; }
    public int TwoPairValue { get; }
    public int PairValue { get; }

    public HandScore Score { get; }
    public ScoreProbabilities Probabilities { get; }

    public Hand(Card cardA, Card cardB, Card cardC, Card cardD, Card cardE)
        : this(new List<Card> { cardA, cardB, cardC, cardD, cardE })
    { }
    public Hand(List<Card> cards)
    {
        cards.Sort();
        Cards = cards.AsReadOnly();

        IsFlush = GetIsFlush();
        StraightValue = GetStraightValue();
        StraightFlushValue = GetStraightFlushValue();

        int[] groupingsArray = GetGroupingsArray();

        FourOfAKindValue = GetFourOfAKindValue(groupingsArray);
        ThreeOfAKindValue = GetThreeOfAKindValue(groupingsArray);

        for (int i = 14; i > 1; i--)
        {
            if (groupingsArray[i] == 2)
            {
                if(PairValue > 0)
                {
                    TwoPairValue = PairValue;
                }
                PairValue = i;
            }
        }
        if (ThreeOfAKindValue > 0 && PairValue > 0)
        {
            FullHouseThreeCardValue = ThreeOfAKindValue;
            FullHouseTwoCardValue = PairValue;
        }

        string scoreTablesKey = HandScore.GetHandScoreKey(this);
        Score = HandScoresTable.All[scoreTablesKey];
        Probabilities = HandScoresTable.Probabilities[scoreTablesKey];
    }

    private int[] GetGroupingsArray()
    {
        int[] ret = new int[15];
        ret[Cards[0].Value]++;
        ret[Cards[1].Value]++;
        ret[Cards[2].Value]++;
        ret[Cards[3].Value]++;
        ret[Cards[4].Value]++;
        return ret;
    }
    
    private int GetThreeOfAKindValue(int[] groupingsArray)
    {
        for (int i = 1; i < 15; i++)
        {
            if (groupingsArray[i] == 3)
            {
                return i;
            }
        }
        return 0;
    }

    private int GetFourOfAKindValue(int[] groupingsArray)
    {
        for (int i = 1; i < 15; i++)
        {
            if(groupingsArray[i] == 4)
            {
                return i;
            }
        }
        return 0;
    }

    private int GetStraightFlushValue()
    {
        if (StraightValue > 0 && IsFlush)
        {
            return StraightValue;
        }
        return 0;
    }

    private int GetStraightValue()
    {
        bool isLowAceStraight = GetIsLowAceStraight();
        if (isLowAceStraight)
        {
            return 5;
        }
        for (int i = 1; i < 5; i++)
        {
            int neededValue = Cards[i - 1].Value - 1;
            if (Cards[i].Value != neededValue)
            {
                return 0;
            }
        }
        return Cards[0].Value;
    }

    private bool GetIsLowAceStraight()
    {
        return Cards[0].Value == 14 && Cards[1].Value == 5 && Cards[2].Value == 4 && Cards[3].Value == 3 && Cards[4].Value == 2;
    }

    private bool GetIsFlush()
    {
        int suit = Cards[0].Suit;
        return Cards[1].Suit == suit
            && Cards[2].Suit == suit
            && Cards[3].Suit == suit
            && Cards[4].Suit == suit;
    }

    public int CompareTo(Hand other)
    {
        if (other == null) return 1;

        int straightFlush = StraightFlushValue.CompareTo(other.StraightFlushValue);
        if (straightFlush != 0)
        {
            return straightFlush;
        }

        int fourOfAKind = FourOfAKindValue.CompareTo(other.FourOfAKindValue);
        if (fourOfAKind != 0)
        {
            return fourOfAKind;
        }

        int fullHouseThreeCard = FullHouseThreeCardValue.CompareTo(other.FullHouseThreeCardValue);
        if (fullHouseThreeCard != 0)
        {
            return fullHouseThreeCard;
        }

        int fullHouseTwoCard = FullHouseTwoCardValue.CompareTo(other.FullHouseTwoCardValue);
        if (fullHouseTwoCard != 0)
        {
            return fullHouseTwoCard;
        }

        int isFlush = IsFlush.CompareTo(other.IsFlush);
        if (isFlush != 0)
        {
            return isFlush;
        }

        if (IsFlush && other.IsFlush)
        {
            return CompareHighCard(other);
        }

        int straight = StraightValue.CompareTo(other.StraightValue);
        if (straight != 0)
        {
            return straight;
        }

        int threeOfAKind = ThreeOfAKindValue.CompareTo(other.ThreeOfAKindValue);
        if (threeOfAKind != 0)
        {
            return threeOfAKind;
        }

        int twoPair = TwoPairValue.CompareTo(other.TwoPairValue);
        if (twoPair != 0)
        {
            return twoPair;
        }

        int pairValue = PairValue.CompareTo(other.PairValue);
        if (pairValue != 0)
        {
            return pairValue;
        }

        return CompareHighCard(other);

    }

    private int CompareHighCard(Hand other)
    {
        int highCardA = Cards[0].CompareTo(other.Cards[0]);
        if (highCardA != 0)
        {
            return highCardA;
        }

        int highCardB = Cards[1].CompareTo(other.Cards[1]);
        if (highCardB != 0)
        {
            return highCardB;
        }

        int highCardC = Cards[2].CompareTo(other.Cards[2]);
        if (highCardC != 0)
        {
            return highCardC;
        }

        int highCardD = Cards[3].CompareTo(other.Cards[3]);
        if (highCardD != 0)
        {
            return highCardD;
        }

        int highCardE = Cards[4].CompareTo(other.Cards[4]);
        return highCardE;
    }

    public override string ToString()
    {
        string ret = "(" + Cards.First().ToString();
        foreach (Card item in Cards.Skip(1))
        {
            ret += "," + item.ToString();
        }
        ret += ")";
        return ret;
    }
}
