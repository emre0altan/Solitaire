using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
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
        
        if (card.parent != null) card.parent.child = null;
        card.parent = nextSlot.parentCard;
        card.m_AllocatedSlot = nextSlot;
        if(nextSlot.cardSlotType == CardSlotType.ChildSlot) nextSlot.parentCard.child = card;
        nextSlot.Allocatted(card);
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

        if (previousSlot.cardSlotType == CardSlotType.ChildSlot) previousSlot.parentCard.child = card;
        card.parent = previousSlot.parentCard;
        card.m_AllocatedSlot = previousSlot;
        if (nextSlot.cardSlotType == CardSlotType.ChildSlot) nextSlot.parentCard.child = null;
        nextSlot.DeAllocatted();
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
}
