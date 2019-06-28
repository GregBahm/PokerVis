using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class HandState
{
    public Card CardA { get; set; }
    public Card CardB { get; set; }
    public Card CardC { get; set; }
    public Card CardD { get; set; }
    public Card CardE { get; set; }
    public Card CardF { get; set; }
    public Card CardG { get; set; }

    public IEnumerable<Card> Cards
    {
        get
        {
            yield return CardA;
            yield return CardB;
            yield return CardC;
            yield return CardD;
            yield return CardE;
            yield return CardF;
            yield return CardG;
        }
    }
    
    public RandomHandGenerator GetRandomHandGenerator()
    {
        return new RandomHandGenerator(Cards);
    }
}

public class RandomHandGenerator
{
    private Random random = new Random();
    private static ReadOnlyCollection<int> BaseIndices;
    static RandomHandGenerator()
    {
        BaseIndices = GetBaseIndices();
    }

    private static ReadOnlyCollection<int> GetBaseIndices()
    {
        List<int> baseIndices = new List<int>();
        for (int i = 0; i < 52; i++)
        {
            baseIndices.Add(i);
        }
        return baseIndices.AsReadOnly();
    }

    private readonly List<Card> BaseCards;
    private readonly ReadOnlyCollection<int> AvailableIndices;

    public RandomHandGenerator(IEnumerable<Card> cards)
    {
        BaseCards = cards.Where(item => item != null).ToList();
        
        AvailableIndices = GetAvailableIndices();
    }

    private ReadOnlyCollection<int> GetAvailableIndices()
    {
        HashSet<int> availableIndices = new HashSet<int>(BaseIndices);
        foreach (int val in BaseCards.Select(item => item.DeckIndex))
        {
            availableIndices.Remove(val);
        }
        return availableIndices.ToList().AsReadOnly();
    }

    public Hand GetRandomHand()
    {
        List<Card> cards = new List<Card>(7);
        List<int> availableIndices = new List<int>(AvailableIndices);
        cards.AddRange(BaseCards);
        for (int remainingCards = BaseCards.Count; remainingCards < 7; remainingCards++)
        {
            int availableIndicesIndex = random.Next(availableIndices.Count);
            int deckIndex = availableIndices[availableIndicesIndex];
            availableIndices.RemoveAt(availableIndicesIndex);
            Card card = Deck.Cards[deckIndex];
            cards.Add(card);
        }
        return Hand.GetBestHandFromSevenCards(cards);
    }
}