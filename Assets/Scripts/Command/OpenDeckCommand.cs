using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class OpenDeckCommand : Command{
    private ClosedCards closedCards;

    public OpenDeckCommand(ClosedCards _closedCards){
        closedCards = _closedCards;
    }
    public override void Execute(){

        int tempInd = closedCards.openIndex++;
        if (closedCards.openIndex == closedCards.untakenCards.Count)
        {
            closedCards.closedCardsEnded = true;
            closedCards.closedCardsLocation.color = new Color(1, 1, 1, 0);
            closedCards.closedCardsLocation.StartCoroutine(CloseDelay());
        }
        Card temp = closedCards.untakenCards[tempInd];
        temp.OpenCard();
        temp.isOpened = true;
        temp.transform.SetAsLastSibling();
        temp.m_CardImage.raycastTarget = true;
        Helper.Instance.holdableCards.Add(temp);
        

        if (closedCards.openedCardLeft.currentCard == null)
        {
            Relocate(temp, closedCards.openedCardLeft);
            closedCards.leftSlotStack.Add(temp);
        }
        else if (closedCards.openedCardMiddle.currentCard == null)
        {
            Helper.Instance.holdableCards.Remove(closedCards.openedCardLeft.currentCard);
            closedCards.openedCardLeft.currentCard.m_CardImage.raycastTarget = false;
            Relocate(temp, closedCards.openedCardMiddle);
        }
        else if (closedCards.openedCardRight.currentCard == null)
        {
            Helper.Instance.holdableCards.Remove(closedCards.openedCardMiddle.currentCard);
            closedCards.openedCardMiddle.currentCard.m_CardImage.raycastTarget = false;
            Relocate(temp, closedCards.openedCardRight);
        }
        else
        {
            Helper.Instance.holdableCards.Remove(closedCards.openedCardRight.currentCard);
            closedCards.openedCardRight.currentCard.m_CardImage.raycastTarget = false;
            closedCards.leftSlotStack.Add(closedCards.openedCardMiddle.currentCard);
            Relocate(closedCards.openedCardMiddle.currentCard, closedCards.openedCardLeft);
            Relocate(closedCards.openedCardRight.currentCard, closedCards.openedCardMiddle);
            Relocate(temp, closedCards.openedCardRight);
        }
    }

    public override void Undo(){
        if (closedCards.openIndex == closedCards.untakenCards.Count){
            closedCards.closedCardsEnded = false;
            closedCards.closedCardsLocation.color = Color.white;
            closedCards.closedCardsLocation.StartCoroutine(CloseDelay());
        }

        int temp = --closedCards.openIndex;
        Card tempCard = closedCards.untakenCards[temp];
        tempCard.CloseCard();
        tempCard.isOpened = false;
        tempCard.m_CardImage.raycastTarget = false;
        tempCard.transform.DOMove(closedCards.closedCardsLocation.transform.position, 1f).SetEase(Ease.OutCubic); 
        tempCard.m_AllocatedSlot.currentCard = null;
        Helper.Instance.holdableCards.Remove(tempCard);

        if (tempCard.m_AllocatedSlot == closedCards.openedCardLeft){
            closedCards.leftSlotStack.Remove(tempCard);
        }
        else if (tempCard.m_AllocatedSlot == closedCards.openedCardMiddle){
            closedCards.openedCardLeft.currentCard.m_CardImage.raycastTarget = true;
            Helper.Instance.holdableCards.Add(closedCards.openedCardLeft.currentCard);
        }
        else if (tempCard.m_AllocatedSlot == closedCards.openedCardRight){
            if (closedCards.leftSlotStack.Count > 1){
                Relocate(closedCards.openedCardMiddle.currentCard,closedCards.openedCardRight);
                Relocate(closedCards.openedCardLeft.currentCard,closedCards.openedCardMiddle);
                Relocate(closedCards.leftSlotStack[closedCards.leftSlotStack.Count-2], closedCards.openedCardLeft);
                closedCards.leftSlotStack.Remove(closedCards.openedCardMiddle.currentCard);
                closedCards.openedCardRight.currentCard.m_CardImage.raycastTarget = true;
                Helper.Instance.holdableCards.Add(closedCards.openedCardRight.currentCard);
            }
            else{
                closedCards.openedCardMiddle.currentCard.m_CardImage.raycastTarget = true;
                Helper.Instance.holdableCards.Add(closedCards.openedCardMiddle.currentCard);
            }
        }
    }
    
    public override string myToString(){
        return "OPEN DECK";
    }
    
    void Relocate(Card _card, CardSlot newCardSlot)
    {
        _card.transform.DOMove(newCardSlot.transform.position, 1f).SetEase(Ease.OutCubic);
        _card.m_AllocatedSlot = newCardSlot;
        newCardSlot.currentCard = _card;
    }
    
    IEnumerator CloseDelay()
    {
        closedCards.availableToOpen = false;
        yield return new WaitForSeconds(1f);
        closedCards.availableToOpen = true;
    }
}
