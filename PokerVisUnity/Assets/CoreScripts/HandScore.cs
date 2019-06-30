using System;
using System.Collections.Generic;
using System.Linq;

public class HandScore
{
    public const int StraightFlushRepeats = 4;
    public const int FourOfAKindRepeats = 4;
    public const int FullHouseRepeats = 24;
    public const int FlushRepeats = 4;
    public const int StraightRepeats = 1020;
    public const int ThreeOfAKindRepeats = 64;
    public const int TwoPairRepeats = 144;
    public const int PairRepeats = 384;
    public const int HighCardRepeats = 1020;

    public string Key { get; }
    public string FriendlyDescription { get; }

    public int Repeats { get; }
    
    public HandScore(string friendlyDescription, string key, int repeats)
    {
        FriendlyDescription = friendlyDescription;
        Key = key;
        Repeats = repeats;
    }

    public static string GetHandScoreKey(Hand hand)
    {
        if (hand.StraightFlushValue > 0)
        {
            return GetStraightFlushKey(hand.StraightFlushValue);
        }
        if (hand.FourOfAKindValue > 0)
        {
            return GetFourOfAKindKey(hand);
        }
        if (hand.FullHouseThreeCardValue > 0)
        {
            return GetFullHouseKey(hand.FullHouseThreeCardValue, hand.FullHouseTwoCardValue);
        }
        if(hand.IsFlush)
        {
            return GetFlushKey(hand);
        }
        if(hand.StraightValue > 0)
        {
            return GetStraightKey(hand.StraightValue);
        }
        if(hand.ThreeOfAKindValue > 0)
        {
            return GetThreeOfAKindKey(hand);
        }
        if(hand.TwoPairValue > 0)
        {
            return GetTwoPairsKey(hand);
        }
        if(hand.PairValue > 0)
        {
            return GetPairKey(hand);
        }
        return GetHighCardKey(hand);
    }

    public static IEnumerable<HandScore> GetHighCards()
    {
        for (int valA = 14; valA > 1; valA--)
        {
            for (int valB = valA - 1; valB > 1; valB--)
            {
                for (int valC = valB - 1; valC > 1; valC--)
                {
                    for (int valD = valC - 1; valD > 1; valD--)
                    {
                        for (int valE = valD - 1; valE > 1; valE--)
                        {
                            if(!IsStraight(valA, valB, valC, valD, valE))
                            {
                                string friendlyLabel = GetHighCardLabel(valA);
                                string key = GetHighCardKey(valA, valB, valC, valD, valE);
                                yield return new HandScore(friendlyLabel, key, HighCardRepeats);
                            }
                        }
                    }
                }
            }
        }
    }

    private static string GetHighCardKey(Hand hand)
    {
        return GetHighCardKey(hand.Cards[0].Value,
            hand.Cards[1].Value,
            hand.Cards[2].Value,
            hand.Cards[3].Value,
            hand.Cards[4].Value);
    }

    private static string GetHighCardLabel(int valA)
    {
        string label = Card.GetCardValueName(valA);
        return label + "-high";
    }

    private static string GetHighCardKey(int valA, int valB, int valC, int valD, int valE)
    {
        string label = GetHighCardLabel(valA);
        string cardB = Card.GetCardValueName(valB);
        string cardC = Card.GetCardValueName(valC);
        string cardD = Card.GetCardValueName(valD);
        string cardE = Card.GetCardValueName(valE);
        return label + " (then " + cardB + " " + cardC + " " + cardD + " " + cardE + ")"; 
    }

    private static string GetPairKey(Hand hand)
    {
        int valC = 0;
        int valD = 0;
        int valE = 0;
        foreach (Card card in hand.Cards)
        {
            int cardVal = card.Value;
            if(cardVal != hand.PairValue)
            {
                if(valC == 0)
                {
                    valC = cardVal;
                }
                else
                {
                    if(valD == 0)
                    {
                        valD = cardVal;
                    }
                    else
                    {
                        valE = cardVal;
                    }
                }
            }
        }
        return GetPairKey(hand.PairValue, valC, valD, valE);
    }

    public static IEnumerable<HandScore> GetPairs()
    {
        for (int pairVal = 14; pairVal > 1; pairVal--)
        {
            for (int valC = 14; valC > 1; valC--)
            {
                for (int valD = valC - 1; valD > 1; valD--)
                {
                    for (int valE = valD - 1; valE > 1; valE--)
                    {
                        if(valC != pairVal && valD != pairVal && valE != pairVal)
                        {
                            string label = GetPairLabel(pairVal);
                            string key = GetPairKey(pairVal, valC, valD, valE);
                            yield return new HandScore(label, key, PairRepeats);
                        }
                    }
                }
            }
        }
    }

    private static string GetPairKey(int pairVal, int valC, int valD, int valE)
    {
        string label = GetPairLabel(pairVal);
        string cardC = Card.GetCardValueName(valC);
        string cardD = Card.GetCardValueName(valD);
        string cardE = Card.GetCardValueName(valE);
        return label + " (then " + cardC + " " + cardD + " " + cardE + ")";
    }

    private static string GetPairLabel(int pairVal)
    {
        string label = Card.GetPluralCardValueName(pairVal);
        return "pair of " + label;
    }

    private static string GetTwoPairsKey(Hand hand)
    {
        int valA = hand.TwoPairValue;
        int valB = hand.PairValue;
        int valC = hand.Cards.First(item => item.Value != valA && item.Value != valB).Value;
        return GetTwoPairsKey(valA, valB, valC);
    }

    public static IEnumerable<HandScore> GetTwoPairs()
    {
        for (int pairA = 14; pairA > 1; pairA--)
        {
            for (int pairB = pairA - 1; pairB > 1; pairB--)
            {
                if (pairA != pairB)
                {
                    for (int kicker = 14; kicker > 1; kicker--)
                    {
                        if(pairB != kicker && pairA != kicker)
                        {
                            string label = GetTwoPairsLabel(pairA, pairB);
                            string key = GetTwoPairsKey(pairA, pairB, kicker);
                            yield return new HandScore(label, key, TwoPairRepeats);
                        }
                    }
                }
            }
        }
    }

    private static string GetTwoPairsLabel(int pairA, int pairB)
    {
        string labelA = Card.GetPluralCardValueName(pairA);
        string labelB = Card.GetPluralCardValueName(pairB);
        return labelA + " and " + labelB;
    }

    private static string GetTwoPairsKey(int pairA, int pairB, int kicker)
    {
        string label = GetTwoPairsLabel(pairA, pairB);
        string kickerName = Card.GetPluralCardValueName(kicker);
        return label + " (kicker " + kicker + ")";
    }

    public static IEnumerable<HandScore> GetThreeOfAKinds()
    {
        for (int firstThree = 14; firstThree > 1; firstThree--)
        {
            for (int nextHighest = 14; nextHighest > 1; nextHighest--)
            {
                if(firstThree != nextHighest)
                {
                    for (int lowest = nextHighest - 1; lowest > 1; lowest--)
                    {
                        if(lowest != firstThree)
                        {
                            string label = GetThreeOfAKindFriendlyName(firstThree);
                            string key = GetThreeOfAKindKey(firstThree, nextHighest, lowest);
                            yield return new HandScore(label, key, ThreeOfAKindRepeats);
                        }
                    }
                }
            }
        }
    }

    private static string GetThreeOfAKindKey(Hand hand)
    {
        int valB = 0;
        int valC = 0;
        foreach (Card card in hand.Cards)
        {
            int val = card.Value;
            if(val != hand.ThreeOfAKindValue)
            {
                if(valB == 0)
                {
                    valB = val;
                }
                else
                {
                    valC = val;
                }
            }
        }
        return GetThreeOfAKindKey(hand.ThreeOfAKindValue, valB, valC);
    }

    private static string GetThreeOfAKindKey(int valA, int valB, int valC)
    {
        string key = GetThreeOfAKindFriendlyName(valA);
        string nameB = Card.GetCardValueName(valB);
        string nameC = Card.GetCardValueName(valC);
        return key + " (then " + nameB + " " + nameC + ")";
    }

    private static string GetThreeOfAKindFriendlyName(int val)
    {
        string name = Card.GetPluralCardValueName(val);
        return "Three " + name;
    }

    public static IEnumerable<HandScore> GetStraights()
    {
        for (int i = 14; i > 4; i--)
        {
            string straightKey = GetStraightKey(i);
            yield return new HandScore(straightKey, straightKey, StraightRepeats);
        }
    }

    private static string GetStraightKey(int straightValue)
    {
        string name = Card.GetCardValueName(straightValue);
        return name + " straight";
    }

    private static string GetFlushKey(Hand hand)
    {
        int cardA = hand.Cards[0].Value;
        int cardB = hand.Cards[1].Value;
        int cardC = hand.Cards[2].Value;
        int cardD = hand.Cards[3].Value;
        int cardE = hand.Cards[4].Value;
        return GetFlushKey(cardA, cardB, cardC, cardD, cardE);
    }

    public static IEnumerable<HandScore> GetFlushes()
    {
        for (int cardOne = 14; cardOne > 1; cardOne--)
        {
            for (int cardTwo = cardOne - 1; cardTwo > 1; cardTwo--)
            {
                for (int cardThree = cardTwo - 1; cardThree > 1; cardThree--)
                {
                    for (int cardFour = cardThree - 1; cardFour > 1; cardFour--)
                    {
                        for (int cardFive = cardFour - 1; cardFive > 1; cardFive--)
                        {
                            if (!IsStraight(cardOne, cardTwo, cardThree, cardFour, cardFive))
                            {
                                string label = GetFlushFriendlyName(cardOne);
                                string key = GetFlushKey(cardOne, cardTwo, cardThree, cardFour, cardFive);
                                yield return new HandScore(label, key, FlushRepeats);
                            }
                        }
                    }
                }
            }
        }
    }

    private static bool IsStraight(int cardOne, int cardTwo, int cardThree, int cardFour, int cardFive)
    {
        if(IsLowAceStraight(cardOne, cardTwo, cardThree, cardFour, cardFive))
        {
            return true;
        }
        return cardOne == cardTwo + 1
            && cardTwo == cardThree + 1
            && cardThree == cardFour + 1
            && cardFour == cardFive + 1;
    }

    private static bool IsLowAceStraight(int cardOne, int cardTwo, int cardThree, int cardFour, int cardFive)
    {
        return cardOne == 14
            && cardTwo == 5
            && cardThree == 4
            && cardFour == 3
            && cardFive == 2;
    }

    private static string GetFlushKey(int cardA, int cardB, int cardC, int cardD, int cardE)
    {
        string baseName = GetFlushFriendlyName(cardA);
        string bName = Card.GetCardValueName(cardB);
        string cName = Card.GetCardValueName(cardC);
        string dName = Card.GetCardValueName(cardD);
        string eName = Card.GetCardValueName(cardE);
        return baseName + " (then " + bName + " " + cName + " " + dName + " " + eName + ")";
    }

    private static string GetFlushFriendlyName(int highCard)
    {
        string cardName = Card.GetCardValueName(highCard);
        return cardName + " flush";
    }

    public static IEnumerable<HandScore> GetFullHouses()
    {
        for (int fullHouseThreeCard = 14; fullHouseThreeCard > 1; fullHouseThreeCard--)
        {
            for (int fullHouseTwoCard = 14; fullHouseTwoCard > 1; fullHouseTwoCard--)
            {
                if(fullHouseThreeCard != fullHouseTwoCard)
                {
                    string key = GetFullHouseKey(fullHouseThreeCard, fullHouseTwoCard);
                    yield return new HandScore(key, key, FullHouseRepeats);
                }
            }
        }
    }

    private static string GetFullHouseKey(int fullHouseThreeCard, int fullHouseTwoCard)
    {
        string partA = Card.GetPluralCardValueName(fullHouseThreeCard);
        string partB = Card.GetPluralCardValueName(fullHouseTwoCard);
        return partA + " Full over " + partB;
    }

    private static string GetFourOfAKindKey(Hand hand)
    {
        int otherCard = hand.Cards[0].Value == hand.FourOfAKindValue ? hand.Cards[4].Value : hand.Cards[0].Value;
        return GetFourOfAKindKey(hand.FourOfAKindValue, otherCard);
    }

    public static IEnumerable<HandScore> GetFourOfAKinds()
    {
        for (int mainValue = 14; mainValue > 1; mainValue--)
        {
            for(int minorValue = 14; minorValue > 1; minorValue--)
            {
                if(mainValue != minorValue)
                {
                    string key = GetFourOfAKindKey(mainValue, minorValue);
                    string friendlyName = GetFourOfAKindFriendlyName(mainValue);
                    yield return new HandScore(friendlyName, key, FourOfAKindRepeats);
                }
            }
        }
    }

    private static string GetFourOfAKindKey(int mainValue, int minorValue)
    {
        string label = GetFourOfAKindFriendlyName(mainValue);
        string addition = Card.GetCardValueName(minorValue);
        return label + " with " + addition;
    }

    private static string GetFourOfAKindFriendlyName(int fourOfAKindValue)
    {
        return "Four " + Card.GetPluralCardValueName(fourOfAKindValue);
    }

    public static IEnumerable<HandScore> GetStraightFlushes()
    {
        for (int i = 14; i > 4; i--)
        {
            string key = GetStraightFlushKey(i);
            yield return new HandScore(key, key, StraightFlushRepeats);
        }
    }

    private static string GetStraightFlushKey(int straightFlushValue)
    {
        if(straightFlushValue == 14)
        {
            return "Royal Flush";
        }
        return Card.GetCardValueName(straightFlushValue) + " High Straight Flush";
    }

    public override string ToString()
    {
        return Key;
    }
}
