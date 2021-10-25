using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CardSlotType{AceBase, EmptySlot, ChildSlot, OpenedCardsRightTop}

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
        if (image != null && cardSlotType != CardSlotType.ChildSlot)
        {
            AddToSlotsList();
        }
    }

    public void AddToSlotsList()
    {
        CardController.cardSlots.Add(image);
        image.raycastTarget = false;
    }
    
    public void RemoveToSlotsList()
    {
        CardController.cardSlots.Remove(image);
        image.raycastTarget = true;
    }

    public void Allocatted(Card _card)
    {
        if (image != null)
        {
            CardController.cardSlots.Remove(image);
            image.raycastTarget = false;
        }
        currentCard = _card;
    }
    
    public bool DeAllocatted()
    {
        if (image != null)
        {
            CardController.cardSlots.Add(image);
            image.raycastTarget = false;
        }
        currentCard = null;
        if (parentCard != null && !parentCard.isOpened){
            parentCard.OpenCard();
            return true;
        }
        return false;
    }

    public void UndoDeLocatted(Card _card){
        if (image != null)
        {
            CardController.cardSlots.Remove(image);
            image.raycastTarget = false;
        }
        currentCard = _card;
        if (parentCard != null && parentCard.isOpened)
        {
            parentCard.CloseCard();
        }
    }
    
}
