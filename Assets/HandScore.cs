using System.Collections.Generic;
using System.Linq;

public class HandScore
{
    public string HandScoreKey { get; }
    public string FriendlyDescription { get; }

    public HandScore(string friendlyDescription, string handScoreKey)
    {
        FriendlyDescription = friendlyDescription;
        HandScoreKey = handScoreKey;
    }

    public static IEnumerable<HandScore> GetAllPossibleHandScores()
    {
        List<HandScore> ret = new List<HandScore>();
        ret.AddRange(GetStraightFlushes());
        ret.AddRange(GetFourOfAKinds());
        ret.AddRange(GetFullHouses());
        ret.AddRange(GetFlushes());
        ret.AddRange(GetStraights());
        ret.AddRange(GetThreeOfAKinds());
        ret.AddRange(GetTwoPairs());
        ret.AddRange(GetPairs());
        ret.AddRange(GetHighCards());
        return ret;
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

    private static IEnumerable<HandScore> GetHighCards()
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
                            string friendlyLabel = GetHighCardLabel(valA);
                            string key = GetHighCardKey(valA, valB, valC, valD, valE);
                            yield return new HandScore(friendlyLabel, key);
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

    private static IEnumerable<HandScore> GetPairs()
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
                            yield return new HandScore(label, key);
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

    private static IEnumerable<HandScore> GetTwoPairs()
    {
        for (int pairA = 14; pairA > 1; pairA--)
        {
            for (int pairB = 14; pairB > 1; pairB--)
            {
                if (pairA != pairB)
                {
                    for (int kicker = 14; kicker > 1; kicker--)
                    {
                        if(pairB != kicker && pairA != kicker)
                        {
                            string label = GetTwoPairsLabel(pairA, pairB);
                            string key = GetTwoPairsKey(pairA, pairB, kicker);
                            yield return new HandScore(label, key);
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

    private static IEnumerable<HandScore> GetThreeOfAKinds()
    {
        for (int i = 14; i > 1; i--)
        {
            for (int j = 14; j > 1; j--)
            {
                if(i != j)
                {
                    for (int k = j - 1; k > 1; k--)
                    {
                        string label = GetThreeOfAKindFriendlyName(i);
                        string key = GetThreeOfAKindKey(i, j, k);
                        yield return new HandScore(label, key);
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

    private static IEnumerable<HandScore> GetStraights()
    {
        for (int i = 14; i > 4; i--)
        {
            string straightKey = GetStraightKey(i);
            yield return new HandScore(straightKey, straightKey);
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

    private static IEnumerable<HandScore> GetFlushes()
    {
        for (int cardOne = 2; cardOne < 14; cardOne++)
        {
            for (int cardTwo = cardOne + 1; cardTwo < 14; cardTwo++)
            {
                for (int cardThree = cardTwo + 1; cardThree < 14; cardThree++)
                {
                    for (int cardFour = cardThree + 1; cardFour < 14; cardFour++)
                    {
                        for (int cardFive = cardFour + 1; cardFive < 14; cardFive++)
                        {
                            string label = GetFlushFriendlyName(cardFive);
                            string key = GetFlushKey(cardFive, cardFour, cardThree, cardTwo, cardOne);
                            yield return new HandScore(label, key);
                        }
                    }
                }
            }
        }
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

    private static IEnumerable<HandScore> GetFullHouses()
    {
        for (int fullHouseThreeCard = 14; fullHouseThreeCard > 1; fullHouseThreeCard--)
        {
            for (int fullHouseTwoCard = 14; fullHouseTwoCard > 1; fullHouseTwoCard--)
            {
                if(fullHouseThreeCard != fullHouseTwoCard)
                {
                    string key = GetFullHouseKey(fullHouseThreeCard, fullHouseTwoCard);
                    yield return new HandScore(key, key);
                }
            }
        }
    }

    private static string GetFullHouseKey(int fullHouseThreeCard, int fullHouseTwoCard)
    {
        string partA = Card.GetPluralCardValueName(fullHouseThreeCard);
        string partB = Card.GetPluralCardValueName(fullHouseTwoCard);
        return partA + " Full over " + fullHouseTwoCard;
    }

    private static string GetFourOfAKindKey(Hand hand)
    {
        int otherCard = hand.Cards.First(item => item.Value != hand.FourOfAKindValue).Value;
        return GetFourOfAKindKey(hand.FourOfAKindValue, otherCard);
    }

    private static IEnumerable<HandScore> GetFourOfAKinds()
    {
        for (int mainValue = 14; mainValue > 1; mainValue--)
        {
            for(int minorValue = 14; minorValue > 1; minorValue--)
            {
                string key = GetFourOfAKindKey(mainValue, minorValue);
                string friendlyName = GetFourOfAKindFriendlyName(mainValue);
                yield return new HandScore(friendlyName, key);
            }
        }
    }

    private static string GetFourOfAKindKey(int mainValue, int minorValue)
    {
        string addition = GetFourOfAKindFriendlyName(mainValue);
        string friendlyName = Card.GetCardValueName(minorValue);
        return friendlyName + " with " + addition;
    }

    private static string GetFourOfAKindFriendlyName(int fourOfAKindValue)
    {
        return "Four " + Card.GetPluralCardValueName(fourOfAKindValue);
    }

    private static IEnumerable<HandScore> GetStraightFlushes()
    {
        for (int i = 14; i > 4; i--)
        {
            string key = GetStraightFlushKey(i);
            yield return new HandScore(key, key);
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
}
