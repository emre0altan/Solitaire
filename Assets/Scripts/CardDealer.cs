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

    public RectTransform coloredGameBG;
    public Image closedCardsLocation;
    public CardSlot openedCardLeft, openedCardMiddle, openedCardRight;
    public CardSlot[] deckSlots;
    public Card cardPrefab;
    

    private List<Card> untakenCards,leftSlotStack;
    private int openIndex = 0;
    private bool closedCardsEnded, availableToOpen = true;
    private float cardOpenTimer = 0;
    
    private void Start()
    {
        leftSlotStack = new List<Card>();
        untakenCards = new List<Card>();
        SpawnCards();
        DistributeCards();
    }

    private void Update()
    {
        if (cardOpenTimer > 0) cardOpenTimer -= Time.deltaTime;
    }


    void SpawnCards()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                Card newCard = Instantiate(cardPrefab, closedCardsLocation.rectTransform.position, closedCardsLocation.rectTransform.rotation, coloredGameBG);
                newCard.m_CardType = (CardType) i;
                newCard.m_CardValue = (CardValue) j;
                untakenCards.Add(newCard);
            }
        }
    }
    
    public void DistributeCards()
    {
        untakenCards.Shuffle();
        StartCoroutine(DisributeRoutine());
    }

    IEnumerator DisributeRoutine()
    {
        availableToOpen = false;
        //WaitForSeconds row = new WaitForSeconds(0.07f), column = new WaitForSeconds(0.3f);
        WaitForSeconds row = new WaitForSeconds(0.01f), column = new WaitForSeconds(0.03f);

        Card previous = null;
        for (int i = 0; i < deckSlots.Length; i++)
        {
            for (int j = 0; j <= i; j++)
            {
                Card latest = untakenCards[0];
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
                untakenCards.RemoveAt(0);
                previous = latest;
                yield return row;
            }

            yield return column;
        }

        availableToOpen = true;
    }
    
    public void OpenClosedCard()
    {
        if(!availableToOpen) return;
        
        if (closedCardsEnded)
        {
            closedCardsLocation.raycastTarget = false;
            CloseAllOpenedCards();
        }
        else if(cardOpenTimer <= 0)
        {
            cardOpenTimer = 0.1f;
            int tempInd = openIndex++;
            if (openIndex == untakenCards.Count)
            {
                closedCardsLocation.color = new Color(1, 1, 1, 0);
                closedCardsEnded = true;
                StartCoroutine(CloseDelay());
            }
            
            untakenCards[tempInd].OpenCard();
            untakenCards[tempInd].isOpened = true;
            untakenCards[tempInd].transform.SetAsLastSibling();
            untakenCards[tempInd].m_CardImage.raycastTarget = true;
            

            if (openedCardLeft.currentCard == null)
            {
                Allocate(untakenCards[tempInd], openedCardLeft);
                leftSlotStack.Add(untakenCards[tempInd]);
            }
            else if (openedCardMiddle.currentCard == null)
            {
                openedCardLeft.currentCard.m_CardImage.raycastTarget = false;
                Allocate(untakenCards[tempInd], openedCardMiddle);
            }
            else if (openedCardRight.currentCard == null)
            {
                openedCardMiddle.currentCard.m_CardImage.raycastTarget = false;
                Allocate(untakenCards[tempInd], openedCardRight);
            }
            else
            {
                openedCardRight.currentCard.m_CardImage.raycastTarget = false;
                leftSlotStack.Add(openedCardMiddle.currentCard);
                Allocate(openedCardMiddle.currentCard, openedCardLeft);
                Allocate(openedCardRight.currentCard, openedCardMiddle);
                Allocate(untakenCards[tempInd], openedCardRight);
            }

        }
    }
    
    void Allocate(Card _card, CardSlot newCardSlot)
    {
        _card.transform.DOMove(newCardSlot.transform.position, 1f).SetEase(Ease.OutCubic);
        _card.m_AllocatedSlot = newCardSlot;
        newCardSlot.currentCard = _card;
    }

    public void RemoveCard(Card _card)
    {
        if (leftSlotStack.Count > 1)
        {
            if (openedCardRight.currentCard == _card)
            {
                openedCardMiddle.currentCard.m_CardImage.raycastTarget = true;
                Allocate(openedCardMiddle.currentCard,openedCardRight);
                Allocate(openedCardLeft.currentCard,openedCardMiddle);
                leftSlotStack.Remove(openedCardMiddle.currentCard);
            }
            else _card.m_AllocatedSlot.currentCard = null;
        }
        else
        {
            if (openedCardMiddle.currentCard == _card) openedCardLeft.currentCard.m_CardImage.raycastTarget = true;
            else if (openedCardRight.currentCard == _card) openedCardMiddle.currentCard.m_CardImage.raycastTarget = true;
            else leftSlotStack.Remove(_card);
            _card.m_AllocatedSlot.currentCard = null;
        }
        untakenCards.Remove(_card);
        _card.m_ChildSlot.AddToSlotsList();
        openIndex--;
    }

    public void UndoRemoveCard(Card _card){
        if (openedCardLeft.currentCard == null)
        {
            Allocate(_card, openedCardLeft);
            leftSlotStack.Add(_card);
        }
        else if (openedCardMiddle.currentCard == null)
        {
            openedCardLeft.currentCard.m_CardImage.raycastTarget = false;
            Allocate(_card, openedCardMiddle);
        }
        else if (openedCardRight.currentCard == null)
        {
            openedCardMiddle.currentCard.m_CardImage.raycastTarget = false;
            Allocate(_card, openedCardRight);
        }
        else
        {
            openedCardRight.currentCard.m_CardImage.raycastTarget = false;
            leftSlotStack.Add(openedCardMiddle.currentCard);
            Allocate(openedCardMiddle.currentCard, openedCardLeft);
            Allocate(openedCardRight.currentCard, openedCardMiddle);
            Allocate(_card, openedCardRight);
        }
    }

    void CloseAllOpenedCards()
    {
        availableToOpen = false;
        if (openedCardRight.currentCard != null)
        {
            openedCardRight.currentCard.CloseCard();
            openedCardRight.currentCard.isOpened = false;
            openedCardRight.currentCard.transform.DOMove(closedCardsLocation.rectTransform.position, 0.5f);
            openedCardRight.currentCard.m_AllocatedSlot = null;
            openedCardRight.currentCard = null;
        }

        if (openedCardMiddle.currentCard != null)
        {
            openedCardMiddle.currentCard.CloseCard();
            openedCardMiddle.currentCard.isOpened = false;
            openedCardMiddle.currentCard.transform.DOMove(closedCardsLocation.rectTransform.position, 0.5f);
            openedCardMiddle.currentCard.m_AllocatedSlot = null;
            openedCardMiddle.currentCard = null;
        }

        while (leftSlotStack.Count > 0)
        {
            Card tmp = leftSlotStack[leftSlotStack.Count-1];
            leftSlotStack.Remove(tmp);
            tmp.CloseCard();
            tmp.isOpened = false;
            tmp.transform.DOMove(closedCardsLocation.rectTransform.position, 0.5f);
            tmp.m_AllocatedSlot = null;
        }
        openedCardLeft.currentCard = null;
        openIndex = 0;
        StartCoroutine(OpenCardsDelay());
    }

    IEnumerator CloseDelay()
    {
        availableToOpen = false;
        yield return new WaitForSeconds(1f);
        availableToOpen = true;
    }
    
    IEnumerator OpenCardsDelay()
    {
        yield return new WaitForSeconds(1f);
        closedCardsEnded = false;
        availableToOpen = true;
        closedCardsLocation.raycastTarget = true;
    }
}
