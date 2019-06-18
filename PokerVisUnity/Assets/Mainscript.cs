using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class Mainscript : MonoBehaviour
{
    [SerializeField]
    private TextAsset AllHandIds;
    private HandsTable handsTable;

    private void Start()
    {
        HandScore[] scores = HandScore.GetAllPossibleHandScores().ToArray();
        Debug.Log(scores.Length);
    }

    public static List<Hand> LoadHands(string handIds)
    {
        string[] lines = handIds.Split('\n');
        List<Hand> ret = new List<Hand>(lines.Length);
        foreach (string line in lines)
        {
            string[] ids = line.Split(' ');
            int idA = Convert.ToInt32(ids[0]);
            int idB = Convert.ToInt32(ids[1]);
            int idC = Convert.ToInt32(ids[2]);
            int idD = Convert.ToInt32(ids[3]);
            int idE = Convert.ToInt32(ids[4]);
            Card cardA = Deck.Cards[idA];
            Card cardB = Deck.Cards[idB];
            Card cardC = Deck.Cards[idC];
            Card cardD = Deck.Cards[idD];
            Card cardE = Deck.Cards[idE];
            List<Card> cards = new List<Card> { cardA, cardB, cardC, cardD, cardE };
            ret.Add(new Hand(cards));
        }
        return ret;
    }

    public static void WriteSortedDeckIds()
    {
        List<Hand> hands = new List<Hand>(2598960);
        for (int cardOne = 0; cardOne < 52; cardOne++)
        {
            for (int cardTwo = cardOne + 1; cardTwo < 52; cardTwo++)
            {
                for (int cardThree = cardTwo + 1; cardThree < 52; cardThree++)
                {
                    for (int cardFour = cardThree + 1; cardFour < 52; cardFour++)
                    {
                        for (int cardFive = cardFour + 1; cardFive < 52; cardFive++)
                        {
                            Card cardA = Deck.Cards[cardOne];
                            Card cardB = Deck.Cards[cardTwo];
                            Card cardC = Deck.Cards[cardThree];
                            Card cardD = Deck.Cards[cardFour];
                            Card cardE = Deck.Cards[cardFive];
                            List<Card> cards = new List<Card> { cardA, cardB, cardC, cardD, cardE };
                            Hand hand = new Hand(cards);
                            hands.Add(hand);
                        }
                    }
                }
            }
        }
        hands.Sort();
        SaveIds(hands);
    }

    private static void SaveIds(List<Hand> hands)
    {
        StringBuilder builder = new StringBuilder();
        foreach (Hand hand in hands)
        {
            int cardA = hand.Cards[0].DeckIndex;
            int cardB = hand.Cards[1].DeckIndex;
            int cardC = hand.Cards[2].DeckIndex;
            int cardD = hand.Cards[3].DeckIndex;
            int cardE = hand.Cards[4].DeckIndex;
            builder.AppendLine(cardA + " " + cardB + " " + cardC + " " + cardD + " " + cardE);
        }
        File.WriteAllText(@"C:\Users\Lisa\Documents\PokerVis\Assets\allHandIds.txt", builder.ToString().Trim());
    }
}
