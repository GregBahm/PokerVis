using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class HandState
{
    public bool PlayerGeneratorUpToDate { get; private set; }
    public bool OpponentGeneratorUpToDate { get; private set; }
    private Card _holeA;
    public Card HoleA
    {
        get => _holeA;
        set
        {
            if (value != _holeA)
            {
                _holeA = value;
                PlayerGeneratorUpToDate = false;
            }
        }
    }
    private Card _holeB;
    public Card HoleB
    {
        get => _holeB;
        set
        {
            if (value != _holeB)
            {
                _holeB = value;
                PlayerGeneratorUpToDate = false;
            }
        }
    }
    private Card _flopA;
    public Card FlopA
    {
        get => _flopA;
        set
        {
            if (value != _flopA)
            {
                _flopA = value;
                PlayerGeneratorUpToDate = false;
                OpponentGeneratorUpToDate = false;
            }
        }
    }
    private Card _flopB;
    public Card FlopB
    {
        get => _flopB;
        set
        {
            if (value != _flopB)
            {
                _flopB = value;
                PlayerGeneratorUpToDate = false;
                OpponentGeneratorUpToDate = false;
            }
        }
    }
    private Card _flopC;
    public Card FlopC
    {
        get => _flopC;
        set
        {
            if (value != _flopC)
            {
                _flopC = value;
                PlayerGeneratorUpToDate = false;
                OpponentGeneratorUpToDate = false;
            }
        }
    }
    private Card _turn;
    public Card Turn
    {
        get => _turn;
        set
        {
            if (value != _turn)
            {
                _turn = value;
                PlayerGeneratorUpToDate = false;
                OpponentGeneratorUpToDate = false;
            }
        }
    }
    private Card _river;
    public Card River
    {
        get => _river;
        set
        {
            if (value != _river)
            {
                _river = value;
                PlayerGeneratorUpToDate = false;
                OpponentGeneratorUpToDate = false;
            }
        }
    }

    public IEnumerable<Card> Cards
    {
        get
        {
            yield return HoleA;
            yield return HoleB;
            foreach (Card card in OpponentCards)
            {
                yield return card;
            }
        }
    }

    public IEnumerable<Card> OpponentCards
    {
        get
        {
            yield return FlopA;
            yield return FlopB;
            yield return FlopC;
            yield return Turn;
            yield return River;
        }
    }

    public RandomHandGenerator GetPlayerHandGenerator()
    {
        PlayerGeneratorUpToDate = true;
        return new RandomHandGenerator(Cards);
    }

    public RandomHandGenerator GetOpponentHandGenerator()
    {
        OpponentGeneratorUpToDate = true;
        return new RandomHandGenerator(OpponentCards);
    }
}
