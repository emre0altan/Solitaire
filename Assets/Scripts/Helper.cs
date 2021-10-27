using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Helper : MonoBehaviour{
    
    public static Helper Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public List<Card> holdableCards;

    /*
    private void Start(){
        //StartCoroutine(Automatic());
    }

    IEnumerator Automatic(){
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < 300; i++){
            Help();
            yield return new WaitForSeconds(1.2f);
        }
    }
    
    private void Update(){
        if (Input.GetMouseButtonDown(0)){
            for (int i = 0; i < holdableCards.Count; i++){
                holdableCards[i].selectionBorder.SetActive(false);
            }
        }
        else if (Input.GetMouseButtonUp(0)){
            for (int i = 0; i < holdableCards.Count; i++){
                holdableCards[i].selectionBorder.SetActive(true);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Space)){
            Help();
        }
    }
    */

    public GameObject indicator;
    private List<CardSlot> tempSlots;
    public void Help(){
        tempSlots = CardController.Instance.cardSlots;
        for (int i = 0; i < holdableCards.Count; i++){
            if (holdableCards[i].m_ChildSlot.cardSlotType != CardSlotType.AceBase){
                for (int j = 0; j < tempSlots.Count; j++){
                    if (tempSlots[j].cardSlotType == CardSlotType.AceBase)
                    {
                        if (CardController.CheckAceBase(holdableCards[i], tempSlots[j]) && holdableCards[i].child == null){
                            indicator.gameObject.SetActive(true);
                            indicator.transform.position = holdableCards[i].transform.position;
                            indicator.transform.DOMove(tempSlots[j].transform.position, 1f)
                                .OnComplete(() => indicator.gameObject.SetActive(false));
                            return;
                        }
                    }
                    else if (tempSlots[j].cardSlotType == CardSlotType.EmptySlot)
                    {
                        if (CardController.CheckEmptySlot(holdableCards[i], tempSlots[j]) && holdableCards[i].m_AllocatedSlot.cardSlotType != CardSlotType.EmptySlot)
                        {
                            indicator.gameObject.SetActive(true);
                            indicator.transform.position = holdableCards[i].transform.position;
                            indicator.transform.DOMove(tempSlots[j].transform.position, 1f).OnComplete(() => indicator.gameObject.SetActive(false));
                            return;
                        }
                    }
                    else if (tempSlots[j].cardSlotType == CardSlotType.ChildSlot && CardController.CheckChildSlot(holdableCards[i], tempSlots[j]) && !(holdableCards[i].m_AllocatedSlot.cardSlotType == CardSlotType.ChildSlot && holdableCards[i].parent.isOpened))
                    {
                        indicator.gameObject.SetActive(true);
                        indicator.transform.position = holdableCards[i].transform.position;
                        indicator.transform.DOMove(tempSlots[j].transform.position, 1f).OnComplete(() => indicator.gameObject.SetActive(false));
                        return;
                    }
                }
            }
        }
        
        DeckOpenIndicator();
        
    }

    void DeckOpenIndicator(){
        if (CardDealer.Instance.closedCards.untakenCards.Count == 0) return;
        indicator.gameObject.SetActive(true);
        if (CardDealer.Instance.closedCards.closedCardsEnded){
            indicator.transform.position = CardDealer.Instance.closedCards.openedCardMiddle.transform.position;
            indicator.transform.DOMove(CardDealer.Instance.closedCards.closedCardsLocation.transform.position, 1f).OnComplete(() => indicator.gameObject.SetActive(false));
        }
        else{
            indicator.transform.position = CardDealer.Instance.closedCards.closedCardsLocation.transform.position;
            indicator.transform.DOMove(CardDealer.Instance.closedCards.openedCardMiddle.transform.position, 1f).OnComplete(() => indicator.gameObject.SetActive(false));
        }
    }
}
