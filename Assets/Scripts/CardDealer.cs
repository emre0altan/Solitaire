using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CardDealer : MonoBehaviour
{
    public static CardDealer Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public ClosedCards closedCards;
    public RectTransform coloredGameBG;
    public CardSlot[] deckSlots;
    public Card cardPrefab;
    
    
    private float cardOpenTimer = 0;
    
    private void Start()
    {
        closedCards.leftSlotStack = new List<Card>();
        closedCards.untakenCards = new List<Card>();
        SpawnCards();
        DistributeCards();
    }

    private void Update()
    {
        if (cardOpenTimer > 0) cardOpenTimer -= Time.deltaTime;
    }

    #region Initial
    void SpawnCards()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                Card newCard = Instantiate(cardPrefab, closedCards.closedCardsLocation.rectTransform.position, closedCards.closedCardsLocation.rectTransform.rotation, coloredGameBG);
                newCard.m_CardType = (CardType) i;
                newCard.m_CardValue = (CardValue) j;
                newCard.gameObject.name = "Card" + (i * 13 + j);
                closedCards.untakenCards.Add(newCard);
            }
        }
    }
    
    public void DistributeCards()
    {
        closedCards.untakenCards.Shuffle();
        StartCoroutine(DisributeRoutine());
    }

    IEnumerator DisributeRoutine()
    {
        closedCards.availableToOpen = false;
        //WaitForSeconds row = new WaitForSeconds(0.07f), column = new WaitForSeconds(0.3f);
        WaitForSeconds row = new WaitForSeconds(0.01f), column = new WaitForSeconds(0.03f);

        Card previous = null;
        for (int i = 0; i < deckSlots.Length; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                Card latest = closedCards.untakenCards[0];
                if (j == 0)
                {
                    latest.m_AllocatedSlot = deckSlots[i];
                    deckSlots[i].currentCard = latest;
                }
                else
                {
                    latest.m_AllocatedSlot = previous.m_ChildSlot;
                    latest.parent = previous;
                    previous.m_ChildSlot.currentCard = latest;
                    previous.child = latest;
                }
                
                bool isOpen = j == i;
                
                latest.m_ChildSlot.AddToSlotsList();
                latest.transform.SetAsLastSibling();
                latest.transform.DOMove(deckSlots[i].transform.position - Vector3.up * 50 * j, 1f).OnComplete(
                    () =>
                    {
                        if (isOpen)
                        {
                            latest.OpenCard();
                            latest.isOpened = true;
                        }
                    });
                closedCards.untakenCards.RemoveAt(0);
                previous = latest;
                yield return row;
            }

            yield return column;
        }

        closedCards.availableToOpen = true;
    }
    

    #endregion
    
    public void OpenClosedCard()
    {
        if(!closedCards.availableToOpen) return;
        
        if (closedCards.closedCardsEnded)
        {
            closedCards.closedCardsLocation.raycastTarget = false;
            MoveController.Instance.AddCommand(new CloseDeckCommand(closedCards));
        }
        else if(cardOpenTimer <= 0)
        {
            cardOpenTimer = 0.3f;
            MoveController.Instance.AddCommand(new OpenDeckCommand(closedCards));
        }
    }
    
    void Relocate(Card _card, CardSlot newCardSlot)
    {
        _card.transform.DOMove(newCardSlot.transform.position, 1f).SetEase(Ease.OutCubic);
        _card.m_AllocatedSlot = newCardSlot;
        newCardSlot.currentCard = _card;
    }

    public void RemoveCard(Card _card)
    {
        if (closedCards.leftSlotStack.Count > 1)
        {
            if (closedCards.openedCardRight.currentCard == _card)
            {
                closedCards.openedCardMiddle.currentCard.m_CardImage.raycastTarget = true;
                Relocate(closedCards.openedCardMiddle.currentCard,closedCards.openedCardRight);
                Relocate(closedCards.openedCardLeft.currentCard,closedCards.openedCardMiddle);
                Relocate(closedCards.leftSlotStack[closedCards.leftSlotStack.Count-2], closedCards.openedCardLeft);
                closedCards.leftSlotStack.Remove(closedCards.openedCardMiddle.currentCard);
            }
            else _card.m_AllocatedSlot.currentCard = null;
        }
        else
        {
            if (closedCards.openedCardMiddle.currentCard == _card) closedCards.openedCardLeft.currentCard.m_CardImage.raycastTarget = true;
            else if (closedCards.openedCardRight.currentCard == _card) closedCards.openedCardMiddle.currentCard.m_CardImage.raycastTarget = true;
            else closedCards.leftSlotStack.Remove(_card);
            _card.m_AllocatedSlot.currentCard = null;
        }
        closedCards.untakenCards.Remove(_card);
        _card.m_ChildSlot.AddToSlotsList();
        closedCards.openIndex--;
    }

    public void UndoRemoveCard(Card _card){
        if (closedCards.openedCardLeft.currentCard == null)
        {
            closedCards.leftSlotStack.Add(_card);
        }
        else if (closedCards.openedCardMiddle.currentCard == null)
        {
            closedCards.openedCardLeft.currentCard.m_CardImage.raycastTarget = false;
        }
        else if (closedCards.openedCardRight.currentCard == null)
        {
            closedCards.openedCardMiddle.currentCard.m_CardImage.raycastTarget = false;
        }
        else
        {
            closedCards.openedCardRight.currentCard.m_CardImage.raycastTarget = false;
            closedCards.leftSlotStack.Add(closedCards.openedCardMiddle.currentCard);
            Relocate(closedCards.openedCardMiddle.currentCard, closedCards.openedCardLeft);
            Relocate(closedCards.openedCardRight.currentCard, closedCards.openedCardMiddle);
        }
        closedCards.untakenCards.Insert(closedCards.openIndex,_card);
        _card.m_ChildSlot.RemoveToSlotsList();
        closedCards.openIndex++;
    }

}
