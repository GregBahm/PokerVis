using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelector : MonoBehaviour
{
    [Range(0, 1)]
    public float Progression;
    private CardSelectorOption CurrentCard;

    public bool ShowOptions;

    private Transform optionsMaster;
    private IReadOnlyList<CardSelectorOption> options;

    private bool wasSelecting;

    private void Start()
    {
        optionsMaster = new GameObject("Options").transform;
        optionsMaster.parent = transform.parent;
        options = CreateOptions();
        CurrentCard = options[0];
    }

    private IReadOnlyList<CardSelectorOption> CreateOptions()
    {
        List<CardSelectorOption> ret = new List<CardSelectorOption>();
        for (int value = 0; value < 52; value++)
        {
            Card card = Deck.Cards[value];
            CardSelectorOption option = CreateOption(card);
            ret.Add(option);
        }
        return ret;
    }

    private CardSelectorOption CreateOption(Card card)
    {
        GameObject optionObj = Instantiate(Mainscript.Instance.CardOptionPrefab);
        optionObj.name = card.ToString();
        optionObj.transform.parent = optionsMaster;
        Material mat = optionObj.GetComponent<MeshRenderer>().material;
        mat.SetFloat("_CardIndex", card.Value - 1);
        mat.SetFloat("_SuitIndex", card.Suit);
        float x = card.Value * (1 + Mainscript.Instance.CardOptionMargin);
        float y = card.Suit * (Mainscript.CardHeight + Mainscript.Instance.CardOptionMargin);
        Transform target = new GameObject("Option for " + card.ToString()).transform;
        target.parent = optionsMaster;
        target.localPosition = new Vector3(x, y, 0);
        return new CardSelectorOption(card, target, optionObj);
    }

    private void Update()
    {
        bool isSelecting = Input.GetMouseButton(0);

        PlaceMain();
        UpdateOptionPositions(isSelecting);
        UpdateSelectedness(isSelecting);
    }

    private void UpdateSelectedness(bool isSelecting)
    {
        CardSelectorOption hoveredCard = GetHoveredCard();
        foreach (CardSelectorOption option in options)
        {
            if(isSelecting)
            {
                option.UpdateSelectedness(option == hoveredCard);
            }
            else
            {
                option.UpdateSelectedness(option == CurrentCard);
            }
        }
        if(!isSelecting && wasSelecting)
        {
            CurrentCard = hoveredCard;
        }
        wasSelecting = isSelecting;
    }

    Vector3 GetMousePos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, 0);
        float enter = 0;
        plane.Raycast(ray, out enter);
        return ray.GetPoint(enter);
    }
    
    private CardSelectorOption GetHoveredCard()
    {
        float minDist = float.MaxValue;
        CardSelectorOption ret = CurrentCard;
        Vector3 mousePos = GetMousePos();
        foreach (CardSelectorOption item in options)
        {
            float dist = (item.Target.position - mousePos).sqrMagnitude;
            if(dist < minDist)
            {
                minDist = dist;
                ret = item;
            }
        }
        return ret;
    }

    private void UpdateOptionPositions(bool isSelecting)
    {
        float progressionTarget = isSelecting ? 0f : 1f;
        Progression = Mathf.Lerp(Progression, progressionTarget, Time.deltaTime * 5);
        foreach (CardSelectorOption option in options)
        {
            option.UpdateOptionPosition(CurrentCard.Target.localPosition, Progression);
        }
    }

    private void PlaceMain()
    {
        optionsMaster.localPosition = Vector3.Lerp(optionsMaster.localPosition, -CurrentCard.Target.localPosition, Time.deltaTime * 5);
    }
}


public class CardSelectorOption
{
    public Card Basis { get; }
    public Transform Target { get; }
    public GameObject OptionObject { get; }

    private float selectedness;

    public CardSelectorOption(Card basis, Transform target, GameObject optionObject)
    {
        Basis = basis;
        Target = target;
        OptionObject = optionObject;
    }

    public void UpdateOptionPosition(Vector3 selectedPosition, float progression)
    {
        Vector3 pos = Vector3.Lerp(Target.localPosition, selectedPosition, progression);
        OptionObject.transform.localPosition = pos;
    }

    public void UpdateSelectedness(bool isSelected)
    {
        float selectednessTarget = isSelected ? 1 : 0;
        selectedness = Mathf.Lerp(selectedness, selectednessTarget, Time.deltaTime * 5);
        float xScale = 1f + selectedness / 10;
        float yScale = Mainscript.CardHeight + selectedness * Mainscript.CardHeight / 10;
        OptionObject.transform.localScale = new Vector3(xScale, yScale, 1);
    }
}