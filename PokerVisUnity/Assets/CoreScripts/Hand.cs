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

    public HandScore Score { get; }
    public int Rank { get; }

    public Hand(Card cardA, Card cardB, Card cardC, Card cardD, Card cardE)
        : this(new List<Card> { cardA, cardB, cardC, cardD, cardE })
    { }
    public Hand(List<Card> cards, bool cardsAreUnsorted = true)
    {
        if(cardsAreUnsorted)
        {
            cards.Sort();
        }
        Cards = cards.AsReadOnly();
        Debug.Assert(Cards.Count == 5, "Creating hand with " + Cards.Count + " cards instead of 5 cards");

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
        Rank = HandScoresTable.Ranks[scoreTablesKey];
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
        return Rank.CompareTo(other.Rank);
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
    
    public static Hand GetBestHandFromSevenCards(List<Card> cards)
    {
        List<Hand> hands = new List<Hand>();
        cards.Sort();
        for (int cardA = 0; cardA < 7; cardA++)
        {
            for (int cardB = cardA + 1; cardB < 7; cardB++)
            {
                for (int cardC = cardB + 1; cardC < 7; cardC++)
                {
                    for (int cardD = cardC + 1; cardD < 7; cardD++)
                    {
                        for (int cardE = cardD + 1; cardE < 7; cardE++)
                        {
                            List<Card> hand = new List<Card> { cards[cardA], cards[cardB], cards[cardC], cards[cardD], cards[cardE] };
                            hands.Add(new Hand(hand, false));
                        }
                    }
                }
            }
        }
        hands.Sort();
        return hands[0];
    }
}