using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CardDealer : MonoBehaviour
{
    public RectTransform coloredGameBG, closedCardsLocation, openedCardLeft, openedCardMiddle, openedCardRight;
    public CardSlot[] deckSlots;
    public Card cardPrefab;

    private List<Card> allCards;

    private void Start()
    {
        allCards = new List<Card>();
        SpawnCards();
        DistributeCards();
    }

    void SpawnCards()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                Card newCard = Instantiate(cardPrefab, closedCardsLocation.position, closedCardsLocation.rotation, coloredGameBG);
                newCard.m_CardType = (CardType) i;
                newCard.m_CardValue = (CardValue) j;
                allCards.Add(newCard);
            }
        }
    }
    
    public void DistributeCards()
    {
        allCards.Shuffle();
        StartCoroutine(DisributeRoutine());
    }

    IEnumerator DisributeRoutine()
    {
        int index = 0;
        WaitForSeconds row = new WaitForSeconds(0.07f), column = new WaitForSeconds(0.3f);
        
        for (int i = 0; i < deckSlots.Length; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                if (j == 0)
                {
                    allCards[index].m_AllocatedSlot = deckSlots[i];
                    deckSlots[i].currentCard = allCards[index];
                }
                else
                {
                    allCards[index].m_AllocatedSlot = allCards[index - 1].m_ChildSlot;
                    allCards[index].parent = allCards[index - 1];
                    allCards[index - 1].m_ChildSlot.currentCard = allCards[index];
                    allCards[index - 1].child = allCards[index];
                }
                
                bool isOpen = j == i;
                int tempInd = index;
                allCards[tempInd].transform.SetAsLastSibling();
                allCards[tempInd].transform.DOMove(deckSlots[i].transform.position - Vector3.up * 50 * j, 1f).OnComplete(
                    () =>
                    {
                        if (isOpen)
                        {
                            allCards[tempInd].m_Animator.SetTrigger("Open");
                            allCards[tempInd].isOpened = true;
                        }
                    });
                yield return row;
                index++;
            }

            yield return column;
        }
    }
    
    

}
