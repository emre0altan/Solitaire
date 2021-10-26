using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CloseDeckCommand : Command{
    private ClosedCards closedCards;

    public CloseDeckCommand(ClosedCards _closedCards){
        closedCards = _closedCards;
    }
    
    public override void Execute(){
        closedCards.availableToOpen = false;
        if (closedCards.openedCardRight.currentCard != null)
        {
            closedCards.openedCardRight.currentCard.CloseCard();
            closedCards.openedCardRight.currentCard.isOpened = false;
            closedCards.openedCardRight.currentCard.transform.DOMove(closedCards.closedCardsLocation.rectTransform.position, 0.5f);
            closedCards.openedCardRight.currentCard.m_AllocatedSlot = null;
            closedCards.openedCardRight.currentCard = null;
        }

        if (closedCards.openedCardMiddle.currentCard != null)
        {
            closedCards.openedCardMiddle.currentCard.CloseCard();
            closedCards.openedCardMiddle.currentCard.isOpened = false;
            closedCards.openedCardMiddle.currentCard.transform.DOMove(closedCards.closedCardsLocation.rectTransform.position, 0.5f);
            closedCards.openedCardMiddle.currentCard.m_AllocatedSlot = null;
            closedCards.openedCardMiddle.currentCard = null;
        }

        while (closedCards.leftSlotStack.Count > 0)
        {
            Card tmp = closedCards.leftSlotStack[closedCards.leftSlotStack.Count-1];
            closedCards.leftSlotStack.Remove(tmp);
            tmp.CloseCard();
            tmp.isOpened = false;
            tmp.transform.DOMove(closedCards.closedCardsLocation.rectTransform.position, 0.5f);
            tmp.m_AllocatedSlot = null;
        }
        closedCards.openedCardLeft.currentCard = null;
        closedCards.openIndex = 0;
        closedCards.closedCardsLocation.StartCoroutine(OpenCardsDelay());
    }
    
    

    public override void Undo(){
        closedCards.availableToOpen = false;
        Card tmp;
        
        for (int i = 0; i < closedCards.untakenCards.Count; i++){
            tmp = closedCards.untakenCards[i];
            tmp.OpenCard();
            tmp.isOpened = true;
            tmp.transform.SetAsLastSibling();
            if (i < closedCards.untakenCards.Count - 3){
                tmp.transform.DOMove(closedCards.openedCardLeft.transform.position, 0.5f);
                tmp.m_AllocatedSlot = closedCards.openedCardLeft;
                closedCards.leftSlotStack.Add(tmp);
            }
            else if (i == closedCards.untakenCards.Count - 3){
                tmp.transform.DOMove(closedCards.openedCardLeft.transform.position, 0.5f);
                tmp.m_AllocatedSlot = closedCards.openedCardLeft;
                closedCards.openedCardLeft.currentCard = tmp;
                closedCards.leftSlotStack.Add(tmp);
            }
            else if (i == closedCards.untakenCards.Count - 2){
                tmp.transform.DOMove(closedCards.openedCardMiddle.transform.position, 0.5f);
                tmp.m_AllocatedSlot = closedCards.openedCardMiddle;
                closedCards.openedCardMiddle.currentCard = tmp;
            }
            else if (i == closedCards.untakenCards.Count - 1){
                tmp.transform.DOMove(closedCards.openedCardRight.transform.position, 0.5f);
                tmp.m_AllocatedSlot = closedCards.openedCardRight;
                closedCards.openedCardRight.currentCard = tmp;
            }
        }
        closedCards.openIndex = closedCards.untakenCards.Count;
        closedCards.closedCardsLocation.StartCoroutine(UndoOpenCardsDelay());
    }
    
    public override string myToString(){
        return "CLOSE DECK";
    }
    
    IEnumerator OpenCardsDelay()
    {
        yield return new WaitForSeconds(1f);
        closedCards.closedCardsEnded = false;
        closedCards.availableToOpen = true;
        closedCards.closedCardsLocation.raycastTarget = true;
    }
    
    IEnumerator UndoOpenCardsDelay()
    {
        yield return new WaitForSeconds(1f);
        closedCards.closedCardsEnded = true;
        closedCards.availableToOpen = true;
        closedCards.closedCardsLocation.raycastTarget = true;
    }
}
