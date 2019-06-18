using System.Collections.Generic;
using System.Linq;

public static class Deck
{
    private static System.Random random = new System.Random();
    public static IReadOnlyList<Card> Cards { get; } = GetCards();

    private static IReadOnlyList<Card> GetCards()
    {
        List<Card> ret = new List<Card>();
        for (int value = 2; value < 15; value++)
        {
            for (int suit = 0; suit < 4; suit++)
            {
                ret.Add(new Card(value, suit));
            }
        }
        return ret.AsReadOnly();
    }

    public static IReadOnlyList<Card> GetShuffledDeck()
    {
        List<Card> baseDeck = Cards.ToList();
        List<Card> ret = new List<Card>();
        for (int i = 0; i < 52; i++)
        {
            int nextIndex = random.Next(0, 52 - i);
            ret.Add(baseDeck[nextIndex]);
            baseDeck.RemoveAt(nextIndex);
        }
        return ret.AsReadOnly();
    }
}
