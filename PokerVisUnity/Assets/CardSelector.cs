using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardSelector : MonoBehaviour
{
    private HandState state;
    public Transform SlotAPos;
    public Transform SlotBPos;
    public Transform SlotCPos;
    public Transform SlotDPos;
    public Transform SlotEPos;
    public Transform SlotFPos;
    public Transform SlotGPos;

    private CardInteractor SelectedCard;
    private IEnumerable<CardSlot> slots;

    public Transform CardOptionsPool;
    private IReadOnlyList<CardInteractor> cards;

    private bool wasSelecting;
    private Vector3 cursorStartDragPoint;
    private Vector3 objStartDragPoint;

    private void Start()
    {
        state = new HandState();
        cards = CreateCards();
        slots = CreateSlots().ToArray();
    }

    private IEnumerable<CardSlot> CreateSlots()
    {
        yield return new CardSlot(SlotAPos, state, (handState, card) => handState.CardA = card);
        yield return new CardSlot(SlotBPos, state, (handState, card) => handState.CardB = card);
        yield return new CardSlot(SlotCPos, state, (handState, card) => handState.CardC = card);
        yield return new CardSlot(SlotDPos, state, (handState, card) => handState.CardD = card);
        yield return new CardSlot(SlotEPos, state, (handState, card) => handState.CardE = card);
        yield return new CardSlot(SlotFPos, state, (handState, card) => handState.CardF = card);
        yield return new CardSlot(SlotGPos, state, (handState, card) => handState.CardG = card);
    }

    private void Update()
    {
        Vector3 cursorPos = GetMousePos();
        bool isSelecting = Input.GetMouseButton(0);
        if (isSelecting)
        {
            if (wasSelecting)
            {
                UpdateDrag(cursorPos);
            }
            else
            {
                SelectedCard = GetHoveredCard(cursorPos);
                if (SelectedCard != null)
                {
                    cursorStartDragPoint = cursorPos;
                    objStartDragPoint = SelectedCard.CardObject.transform.position;
                }
            }
        }
        else
        {
            if(wasSelecting && SelectedCard != null)
            {
                DoRelease(cursorPos);
            }
            SettleCardPositions();
        }
        wasSelecting = isSelecting;

        RandomHandGenerator generator = state.GetRandomHandGenerator();
        generator.GetRandomHand();
    }

    private void SettleCardPositions()
    {
        HashSet<CardInteractor> cardHash = new HashSet<CardInteractor>(cards);
        foreach (CardSlot slot in slots.Where(item => item.Occupant != null))
        {
            slot.Occupant.ReturnToPosition(slot);
            cardHash.Remove(slot.Occupant);
        }
        foreach (CardInteractor card in cardHash)
        {
            card.ReturnToPosition(null);
        }
    }

    private void DoRelease(Vector3 cursorPos)
    {
        CardSlot closestSlot = GetHoveredSlot(cursorPos);
        CardSlot currentSlot = slots.FirstOrDefault(item => item.Occupant == SelectedCard);
        if(currentSlot != null)
        {
            currentSlot.Occupant = null;
        }
        if (closestSlot != null)
        {
            closestSlot.Occupant = SelectedCard;
        }
    }

    private void UpdateDrag(Vector3 cursorPos)
    {
        if(SelectedCard == null)
        {
            return;
        }
        Vector3 drag = cursorPos - cursorStartDragPoint;
        SelectedCard.CardObject.transform.position = objStartDragPoint + drag;
    }

    private IReadOnlyList<CardInteractor> CreateCards()
    {
        List<CardInteractor> ret = new List<CardInteractor>();
        for (int value = 0; value < 52; value++)
        {
            Card card = Deck.Cards[value];
            CardInteractor option = CreateCard(card);
            ret.Add(option);
        }
        return ret;
    }

    private CardInteractor CreateCard(Card card)
    {
        GameObject cardObj = Instantiate(Mainscript.Instance.CardOptionPrefab);
        cardObj.name = card.ToString();
        cardObj.transform.parent = CardOptionsPool;
        Material mat = cardObj.GetComponent<MeshRenderer>().material;
        mat.SetFloat("_CardIndex", card.Value - 1);
        mat.SetFloat("_SuitIndex", card.Suit);
        Transform basePosition = new GameObject("Option for " + card.ToString()).transform;
        basePosition.parent = CardOptionsPool;
        basePosition.localPosition = GetBasePosition(card);
        cardObj.transform.localPosition = basePosition.localPosition;
        return new CardInteractor(card, basePosition, cardObj);
    }

    private Vector3 GetBasePosition(Card card)
    {
        float x = (card.Value - 2) * (1 + Mainscript.Instance.CardOptionMargin);
        float maxX = 13f * (1 + Mainscript.Instance.CardOptionMargin);
        x -= maxX / 2;
        x += .5f;
        
        float y = card.Suit * (Mainscript.CardHeight + Mainscript.Instance.CardOptionMargin);
        float maxY = 4f * (Mainscript.CardHeight + Mainscript.Instance.CardOptionMargin);
        y -= maxY / 2;
        y += Mainscript.CardHeight / 2;

        return new Vector3(x, y, 0);
    }

    Vector3 GetMousePos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(transform.forward, transform.position);
        float enter = 0;
        plane.Raycast(ray, out enter);
        return ray.GetPoint(enter);
    }

    private CardSlot GetHoveredSlot(Vector3 cursorPos)
    {
        float minDist = (SelectedCard.BasePosition.position - cursorPos).sqrMagnitude;
        CardSlot ret = null;
        foreach (CardSlot slot in slots)
        {
            float dist = (slot.SlotPosition.position - cursorPos).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                ret = slot;
            }
        }
        return ret;
    }
    
    private CardInteractor GetHoveredCard(Vector3 cursorPos)
    {
        float minDist = float.MaxValue;
        CardInteractor ret = SelectedCard;
        foreach (CardInteractor item in cards)
        {
            float dist = (item.CardObject.transform.position - cursorPos).sqrMagnitude;
            if(dist < minDist)
            {
                minDist = dist;
                ret = item;
            }
        }
        return ret;
    }

    private class CardSlot
    {
        private readonly HandState state;
        private readonly Action<HandState, Card> stateSetter;
        public Transform SlotPosition { get; }

        private CardInteractor occupant;
        public CardInteractor Occupant
        {
            get { return occupant; }
            set
            {
                occupant = value;
                stateSetter(state, occupant?.Basis);
            }
        }

        public CardSlot(Transform slotPosition, HandState state, Action<HandState, Card> stateSetter)
        {
            this.state = state;
            this.stateSetter = stateSetter;
            SlotPosition = slotPosition;
        }
    }

    private class CardInteractor
    {
        public Card Basis { get; }
        public Transform BasePosition { get; }
        public GameObject CardObject { get; }

        private float selectedness;

        public CardInteractor(Card basis, Transform basePosition, GameObject cardObject)
        {
            Basis = basis;
            BasePosition = basePosition;
            CardObject = cardObject;
        }

        public void ReturnToPosition(CardSlot slotPosition)
        {
            Vector3 targetPosition = slotPosition == null ? BasePosition.position : slotPosition.SlotPosition.position;
            CardObject.transform.position = Vector3.Lerp(CardObject.transform.position, targetPosition, Time.deltaTime * 10);
        }

        public override string ToString()
        {
            return Basis.ToString();
        }
    }
}