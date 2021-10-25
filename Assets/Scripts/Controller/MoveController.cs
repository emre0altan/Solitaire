using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Command{
    
}

public class MoveCommand : Command{
    private Card card;
    private CardSlot previousSlot, nextSlot;
    private bool parentCardOpened = false;
    
    public MoveCommand(Card _card, CardSlot _nextSlot){
        card = _card;
        previousSlot = _card.m_AllocatedSlot;
        nextSlot = _nextSlot;
    }

    public void Execute(){
        if (previousSlot.cardSlotType == CardSlotType.OpenedCardsRightTop) CardDealer.Instance.RemoveCard(card);
        else parentCardOpened = previousSlot.DeAllocatted();
        
        if (card.parent != null) card.parent.child = null;
        card.parent = nextSlot.parentCard;
        card.m_AllocatedSlot = nextSlot;
        if(nextSlot.cardSlotType == CardSlotType.ChildSlot) nextSlot.parentCard.child = card;
        nextSlot.Allocatted(card);
        card.transform.DOMove(nextSlot.transform.position, 0.3f).OnComplete(() =>
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

    public void Undo(){
        if (previousSlot.cardSlotType == CardSlotType.OpenedCardsRightTop) CardDealer.Instance.UndoRemoveCard(card);
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

    }
}

public class MoveController : MonoBehaviour
{
    public static MoveController Instance;
    public List<MoveCommand> commands;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        commands = new List<MoveCommand>();
    }

    public int currentCommand = -1;

    public void AddCommand(MoveCommand newCommand){
        newCommand.Execute();
        commands.Add(newCommand);
        currentCommand++;
    }

    public void UndoCommand(){
        if(currentCommand < 0) return; 
        commands[currentCommand].Undo();
        currentCommand--;
    }

}
