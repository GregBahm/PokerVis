using System;
using System.Collections.Generic;

public class Card : IComparable<Card>
{
    public int Value { get; }
    public int Suit { get; }
    public int DeckIndex
    {
        get
        {
            int val = Value - 2;
            val = val * 4;
            return val + Suit;
        }
    }

    public Card(int value, int suit)
    {
        Value = value;
        Suit = suit;
    }

    private static readonly Dictionary<int, string> ValueNames = new Dictionary<int, string>
    {
        {2, "Two" },
        {3, "Three" },
        {4, "Four" },
        {5, "Five" },
        {6, "Six" },
        {7, "Seven" },
        {8, "Eight" },
        {9, "Nine" },
        {10, "Ten" },
        {11, "Jack" },
        {12, "Queen" },
        {13, "King" },
        {14, "Ace" }
    };

    private static readonly Dictionary<int, string> PluralValueNames = new Dictionary<int, string>
    {
        {2, "Deuces" },
        {3, "Treys" },
        {4, "Fours" },
        {5, "Fives" },
        {6, "Sixes" },
        {7, "Sevens" },
        {8, "Eights" },
        {9, "Nines" },
        {10, "Tens" },
        {11, "Jacks" },
        {12, "Queens" },
        {13, "Kings" },
        {14, "Aces" }
    };

    private static readonly Dictionary<int, string> SuitNames = new Dictionary<int, string>
    {
        {0, "Spades" },
        {1, "Clubes" },
        {2, "Hearts" },
        {3, "Dimonds" },
    };

    public static string GetPluralCardValueName(int value)
    {
        return PluralValueNames[value];
    }
    public static string GetCardValueName(int value)
    {
        return ValueNames[value];
    }

    private string GetName()
    {
        return ValueNames[Value] + " of " + SuitNames[Suit]; 
    }

    public int CompareTo(Card other)
    {
        if (other == null) return 1;
        return Value.CompareTo(other.Value);
    }

    public override string ToString()
    {
        return GetName();
    }
}
