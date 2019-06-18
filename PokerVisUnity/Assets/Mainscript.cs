using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class Mainscript : MonoBehaviour
{
    private HandState state;
    
    private void Start()
    {
        state = new HandState();
        state.CardA = Deck.Cards[0];
        Hand[] hands = state.GetAllPotentialHands().ToArray();
        Debug.Log(hands.Length);
    }
    
}

public class HandState
{
    private static IReadOnlyList<int> BaseDeckIndexes;

    static HandState()
    {
        BaseDeckIndexes = GetBaseDeckIndexes();
    }

    private static IReadOnlyList<int> GetBaseDeckIndexes()
    {
        List<int> ret = new List<int>();
        for (int i = 0; i < 52; i++)
        {
            ret.Add(i);
        }
        return ret;
    }

    public Card CardA { get; set; }
    public Card CardB { get; set; }
    public Card CardC { get; set; }
    public Card CardD { get; set; }
    public Card CardE { get; set; }

    public IEnumerable<Card> Cards
    {
        get
        {
            yield return CardA;
            yield return CardB;
            yield return CardC;
            yield return CardD;
            yield return CardE;
        }
    }

    public IEnumerable<Hand> GetAllPotentialHands()
    {
        bool[] indexesToSkip = GetIndexesToSkip();
        return CardAHands(indexesToSkip);
    }

    private bool[] GetIndexesToSkip()
    {
        bool[] ret = new bool[52];
        foreach (Card card in Cards)
        {
            if (card != null)
            {
                ret[card.DeckIndex] = true;
            }
        }
        return ret;
    }

    private IEnumerable<Hand> CardAHands(bool[] indexesToSkip)
    {
        if(CardA == null)
        {
            List<Hand> ret = new List<Hand>();
            for (int i = 0; i < 52; i++)
            {
                if(!indexesToSkip[i])
                {
                    IEnumerable<Hand> retItems = CardBHands(i, indexesToSkip);
                    ret.AddRange(retItems);
                }
            }
            return ret;
        }
        else
        {
            return CardBHands(CardA.DeckIndex, indexesToSkip);
        }
    }

    private IEnumerable<Hand> CardBHands(int cardAIndex, bool[] indexesToSkip)
    {
        if (CardB == null)
        {
            List<Hand> ret = new List<Hand>();
            for (int i = 0; i < 52; i++)
            {
                if (!indexesToSkip[i] && i != cardAIndex)
                {
                    IEnumerable<Hand> retItems = CardCHands(cardAIndex, i, indexesToSkip);
                    ret.AddRange(retItems);
                }
            }
            return ret;
        }
        else
        {
            return CardCHands(cardAIndex, CardB.DeckIndex, indexesToSkip);
        }
    }
    private IEnumerable<Hand> CardCHands(int cardAIndex, int cardBIndex, bool[] indexesToSkip)
    {
        if (CardC == null)
        {
            List<Hand> ret = new List<Hand>();
            for (int i = 0; i < 52; i++)
            {
                if (!indexesToSkip[i] && i != cardAIndex && i != cardBIndex)
                {
                    IEnumerable<Hand> retItems = CardDHands(cardAIndex, cardBIndex, i, indexesToSkip);
                    ret.AddRange(retItems);
                }
            }
            return ret;
        }
        else
        {
            return CardDHands(cardAIndex, cardBIndex, CardC.DeckIndex, indexesToSkip);
        }
    }
    private IEnumerable<Hand> CardDHands(int cardAIndex, int cardBIndex, int cardCIndex, bool[] indexesToSkip)
    {
        if (CardD == null)
        {
            List<Hand> ret = new List<Hand>();
            for (int i = 0; i < 52; i++)
            {
                if (!indexesToSkip[i] && i != cardAIndex && i != cardBIndex && i != cardCIndex)
                {
                    IEnumerable<Hand> retItems = CardEHands(cardAIndex, cardBIndex, cardCIndex, i);
                    ret.AddRange(retItems);
                }
            }
            return ret;
        }
        else
        {
            return CardEHands(cardAIndex, cardBIndex, cardCIndex, CardD.DeckIndex);
        }
    }

    private IEnumerable<Hand> CardEHands(int cardAIndex, int cardBIndex, int cardCIndex, int cardDIndex)
    {
        if (CardE == null)
        {
            for (int i = 0; i < 52; i++)
            {
                if (i != cardAIndex && i != cardBIndex && i != cardCIndex && i != cardDIndex)
                {
                    yield return new Hand(Deck.Cards[cardAIndex], Deck.Cards[cardBIndex], Deck.Cards[cardCIndex], Deck.Cards[cardDIndex], Deck.Cards[i]);
                }
            }
        }
        else
        {
            yield return new Hand(Deck.Cards[cardAIndex], Deck.Cards[cardBIndex], Deck.Cards[cardCIndex], Deck.Cards[cardDIndex], Deck.Cards[CardE.DeckIndex]);
        }
    }

}