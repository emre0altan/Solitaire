using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CardSlotType{AceBase, EmptySlot, AboveCard}

public class CardSlot : MonoBehaviour
{
    public CardSlotType cardSlotType;
    public CardType cardType;
    public Card belowCard;
    public Image image;
    private void Awake()
    {
        if (CardController.cardSlots == null) CardController.cardSlots = new List<Image>();
        CardController.cardSlots.Add(image);
        image.raycastTarget = false;
    }
}
