using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    
    public Hand(Card cardA, Card cardB, Card cardC, Card cardD, Card cardE)
        :this(new List<Card> { cardA, cardB, cardC, cardD, cardE})
    { }
    public Hand(List<Card> cards)
    {
        cards.Sort();
        Cards = cards.AsReadOnly();

        IsFlush = GetIsFlush();
        StraightValue = GetStraightValue();
        StraightFlushValue = GetStraightFlushValue();

        IGrouping<int, Card>[] grouped = cards.GroupBy(item => item.Value).ToArray();

        FourOfAKindValue = GetFourOfAKindValue(grouped);
        ThreeOfAKindValue = GetThreeOfAKindValue(grouped);

        IGrouping<int, Card>[] pairs = grouped.Where(item => item.Count() == 2).ToArray();
        
        if(pairs.Length == 2)
        {
            int pairA = pairs[0].First().Value;
            int pairB = pairs[1].First().Value;
            TwoPairValue = Mathf.Max(pairA, pairB);
            PairValue = Mathf.Min(pairA, pairB);
        }
        else if(pairs.Length == 1)
        {
            if (ThreeOfAKindValue > 0)
            {
                FullHouseThreeCardValue = ThreeOfAKindValue;
                ThreeOfAKindValue = 0;
                FullHouseTwoCardValue = pairs[0].First().Value;
            }
            else
            {
                PairValue = pairs[0].First().Value;
            }
        }
    }

    private int GetThreeOfAKindValue(IGrouping<int, Card>[] grouped)
    {
        IGrouping<int, Card> grouping = grouped.FirstOrDefault(item => item.Count() == 3);
        if (grouping != null)
        {
            return grouping.First().Value;
        }
        return 0;
    }

    private int GetFourOfAKindValue(IGrouping<int, Card>[] grouped)
    {
        IGrouping<int, Card> grouping = grouped.FirstOrDefault(item => item.Count() == 4);
        if(grouping != null)
        {
            return grouping.First().Value;
        }
        return 0;
    }

    private int GetStraightFlushValue()
    {
        if(StraightValue > 0 && IsFlush)
        {
            return StraightValue;
        }
        return 0;
    }

    private int GetStraightValue()
    {
        bool isLowAceStraight = GetIsLowAceStraight();
        if(isLowAceStraight)
        {
            return 5;
        }
        for (int i = 1; i < 5; i++)
        {
            int neededValue = Cards[i - 1].Value + 1;
            if(Cards[i].Value != neededValue)
            {
                return 0;
            }
        }
        return Cards[4].Value;
    }

    private bool GetIsLowAceStraight()
    {
        return Cards[4].Value == 14 && Cards[3].Value == 5 && Cards[2].Value == 4 && Cards[1].Value == 3 && Cards[0].Value == 2;
    }

    private bool GetIsFlush()
    {
        return Cards.All(item => item.Suit == Cards[0].Suit);
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
        if(fourOfAKind != 0)
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
        if(isFlush != 0)
        {
            return isFlush;
        }

        if(IsFlush && other.IsFlush)
        {
             return CompareHighCard(other);
        }

        int straight = StraightValue.CompareTo(other.StraightValue);
        if(straight != 0)
        {
            return straight;
        }

        int threeOfAKind = ThreeOfAKindValue.CompareTo(other.ThreeOfAKindValue);
        if(threeOfAKind != 0)
        {
            return threeOfAKind;
        }

        int twoPair = TwoPairValue.CompareTo(other.TwoPairValue);
        if(twoPair != 0)
        {
            return twoPair;
        }

        int pairValue = PairValue.CompareTo(other.PairValue);
        if(pairValue != 0)
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
