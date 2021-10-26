using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveCommand : Command{
    private Card card;
    private CardSlot previousSlot, nextSlot;
    private bool parentCardOpened = false;
    
    public MoveCommand(Card _card, CardSlot _nextSlot){
        card = _card;
        previousSlot = _card.m_AllocatedSlot;
        nextSlot = _nextSlot;
    }

    public override void Execute(){
        if (previousSlot.cardSlotType == CardSlotType.OpenedCardsRightTop) CardDealer.Instance.RemoveCard(card);
        else parentCardOpened = previousSlot.DeAllocatted();
        
        if (card.m_ChildSlot.atAceBase) UpdateToAceBaseType(card.m_ChildSlot, false);
        
        if (card.parent != null) card.parent.child = null;
        card.parent = nextSlot.parentCard;
        card.m_AllocatedSlot = nextSlot;
        nextSlot.Allocatted(card);
        if(nextSlot.cardSlotType == CardSlotType.ChildSlot) nextSlot.parentCard.child = card;
        else if(nextSlot.cardSlotType == CardSlotType.AceBase) UpdateToAceBaseType(card.m_ChildSlot, true);
        
        card.transform.DOMove(nextSlot.transform.position, 0.2f).OnComplete(() =>
        {
            card.m_CardImage.raycastTarget = true;
        });
        
        Card temporaryCard = card.child;
        int i = 1;
        while (temporaryCard != null && i < 20)
        {
            Card tmpp = temporaryCard;
            temporaryCard.transform.DOMove(nextSlot.transform.position + card.m_ChildSlot.transform.localPosition * i, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                tmpp.m_CardImage.raycastTarget = true;
            });
            temporaryCard = temporaryCard.child;
            i++;
        }
    }

    public override void Undo(){
        if (previousSlot.cardSlotType == CardSlotType.OpenedCardsRightTop){
            CardDealer.Instance.UndoRemoveCard(card);
            previousSlot.currentCard = card;
        }
        else{
            if(parentCardOpened) previousSlot.UndoDeLocatted(card);
            else previousSlot.DeAllocatted();
        }
        if (previousSlot.cardSlotType == CardSlotType.AceBase) UpdateToAceBaseType(card.m_ChildSlot, true);

        if (previousSlot.cardSlotType == CardSlotType.ChildSlot) previousSlot.parentCard.child = card;
        card.parent = previousSlot.parentCard;
        card.m_AllocatedSlot = previousSlot;
        nextSlot.DeAllocatted();
        if (nextSlot.cardSlotType == CardSlotType.ChildSlot) nextSlot.parentCard.child = null;
        else if (nextSlot.cardSlotType == CardSlotType.AceBase) UpdateToAceBaseType(card.m_ChildSlot, false);
        card.transform.DOMove(previousSlot.transform.position, 0.3f).OnComplete((() => {
            card.m_CardImage.raycastTarget = true;
        }));
        
        card.transform.SetAsLastSibling();

        Card temporaryCard = card.child;
        int i = 1;
        while (temporaryCard != null && i < 20)
        {
            Card tmpp = temporaryCard;
            temporaryCard.transform.SetAsLastSibling();
            temporaryCard.transform.DOMove(previousSlot.transform.position + card.m_ChildSlot.transform.localPosition * i, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                tmpp.m_CardImage.raycastTarget = true;
            });
            temporaryCard = temporaryCard.child;
            i++;
        }
    }

    public override string myToString(){
        return "MC-" + card.gameObject.name + "_FROM-" + previousSlot.gameObject.name + "_TO-" +
               nextSlot.gameObject.name;
    }
    
    void UpdateToAceBaseType(CardSlot _cardSlot, bool isAceBase)
    {
        _cardSlot.atAceBase = isAceBase;
        _cardSlot.cardSlotType = isAceBase? CardSlotType.AceBase: CardSlotType.ChildSlot;
        _cardSlot.transform.localPosition = isAceBase? Vector3.zero : new Vector3(0, -50, 0);
    }
    
}
