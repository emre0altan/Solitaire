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
    public Card parentCard, currentCard;
    public bool atAceBase = false;
    public Image image;
    private void Awake()
    {
        if (CardController.cardSlots == null) CardController.cardSlots = new List<Image>();
        CardController.cardSlots.Add(image);
        image.raycastTarget = false;
    }

    public void Allocatted(Card _card)
    {
        CardController.cardSlots.Remove(image);
        image.raycastTarget = false;
        currentCard = _card;
    }
    
    public void DeAllocatted()
    {
        CardController.cardSlots.Add(image);
        image.raycastTarget = false;
        currentCard = null;
        if (parentCard != null && !parentCard.isOpened)
        {
            parentCard.m_Animator.SetTrigger("Open");
        }
    }
    
}
