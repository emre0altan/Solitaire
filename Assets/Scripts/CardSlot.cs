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
    public GameObject availableImage;
    private void Start()
    {
        if (CardController.Instance.cardSlots == null) CardController.Instance.cardSlots = new List<CardSlot>();
        if (image != null && cardSlotType != CardSlotType.ChildSlot)
        {
            AddToSlotsList();
        }
    }

    public void AddToSlotsList()
    {
        CardController.Instance.cardSlots.Add(this);
        image.raycastTarget = false;
    }
    
    public void RemoveToSlotsList()
    {
        CardController.Instance.cardSlots.Remove(this);
        availableImage.SetActive(false);
        image.raycastTarget = false;
    }

    public void Allocatted(Card _card)
    {
        if (image != null)
        {
            CardController.Instance.cardSlots.Remove(this);
            availableImage.SetActive(false);
            image.raycastTarget = false;
        }
        currentCard = _card;
    }
    
    public bool DeAllocatted()
    {
        if (image != null)
        {
            CardController.Instance.cardSlots.Add(this);
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
            CardController.Instance.cardSlots.Remove(this);
            image.raycastTarget = false;
        }
        currentCard = _card;
        if (parentCard != null && parentCard.isOpened)
        {
            parentCard.CloseCard();
        }
    }
    
}
