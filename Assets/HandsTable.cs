using System;
using System.Collections.Generic;
using System.Linq;

public class HandsTable
{
    public HandsTable(string handsTableData)
    {
        Hand[] hands = GetHands(handsTableData).ToArray();
    }

    private IEnumerable<Hand> GetHands(string handsTableData)
    {
        string[] lines = handsTableData.Split('\n');
        foreach (string line in lines)
        {
            yield return GetHand(line);
        }
    }

    private Hand GetHand(string handData)
    {
        List<Card> cards = new List<Card>();
        string[] cardStrings = handData.Split(' ');
        foreach (string cardString in cardStrings)
        {
            int cardValue = Convert.ToInt32(cardString);
            Card card = new Card(cardValue, 0);
            cards.Add(card);
        }
        return new Hand(cards);
    }
}
