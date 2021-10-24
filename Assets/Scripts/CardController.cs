using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardController : MonoBehaviour
{
    public static List<Image> cardSlots;
    
    [SerializeField] Card holdingCard;
    [SerializeField] GraphicRaycaster m_Raycaster;
    [SerializeField] EventSystem m_EventSystem;
    PointerEventData m_PointerEventData;

    private CardSlot holdingCardOriginalSlot;

    private Card tempChildCard;

    private void Start()
    {
        TouchManager.Instance.onTouchBegan += OnDown;
        TouchManager.Instance.onTouchMoved += OnMove;
        TouchManager.Instance.onTouchEnded += OnUp;
    }

    public void OnMove(TouchInput touchInput)
    {
        if (holdingCard != null)
        {
            holdingCard.transform.position = touchInput.ScreenPosition;
            tempChildCard = holdingCard.child;
            int i = 0;
            while (tempChildCard != null && i < 20)
            {
                tempChildCard.transform.position = Vector3.Lerp(tempChildCard.transform.position,
                    tempChildCard.parent.m_ChildSlot.transform.position, 0.2f);
                tempChildCard = tempChildCard.child;
                i++;
            }
        }
    }
    
    public void OnDown(TouchInput touchInput)
    {
         List<RaycastResult> results = RaycastUI(touchInput.ScreenPosition);

        if (results.Count > 0 && results[0].gameObject.CompareTag("Card"))
        {
            UpdateCardSlots(true);
            holdingCard = results[0].gameObject.GetComponent<Card>();
            Card tempCard = holdingCard;
            tempCard.transform.SetAsLastSibling();
            while (tempCard.child != null)
            {
                tempCard = tempCard.child;
                tempCard.transform.SetAsLastSibling();
                tempCard.m_CardImage.raycastTarget = false;
            }
            
            if(holdingCard.inOriginalPosition) holdingCardOriginalSlot = holdingCard.m_AllocatedSlot;
            holdingCard.inOriginalPosition = false;
            holdingCard.m_CardImage.raycastTarget = false;
            holdingCard.m_ChildSlot.image.raycastTarget = false;
            holdingCard.selectionBorder.SetActive(true);
            holdingCard.m_AllocatedSlot.image.raycastTarget = false;
        }
    }

    public void OnUp(TouchInput touchInput)
    {
        if (holdingCard != null)
        {
            Card tmpCard = holdingCard;
            holdingCard = null;
            
            List<RaycastResult>results = RaycastUI(touchInput.ScreenPosition);
            if (results.Count > 0 && results[0].gameObject.CompareTag("CardSlot"))
            {
                CardSlot _cardSlot = results[0].gameObject.GetComponent<CardSlot>();
                if (!CanCardGoThere(tmpCard, _cardSlot))
                {
                    SendHoldingCardToOriginalPosition(tmpCard);
                }
            }
            else
            {
                SendHoldingCardToOriginalPosition(tmpCard);
            }

            tmpCard.selectionBorder.SetActive(false);
        }
        UpdateCardSlots(false);
    }

    public bool CanCardGoThere(Card _card, CardSlot _cardSlot)
    {
        if (_cardSlot.cardSlotType == CardSlotType.AceBase)
        {
            if (CheckAceBase(_card, _cardSlot))
            {
                Debug.Log("ACE BASE SLOT");
                _card.m_ChildSlot.atAceBase = true;
                ReAllocation(_card, _cardSlot);
                return true;
            }
            else return false;
        }
        else if (_cardSlot.cardSlotType == CardSlotType.EmptySlot)
        {
            if (CheckEmptySlot(_card, _cardSlot))
            {
                Debug.Log("EMPTY SLOT");
                _card.m_ChildSlot.atAceBase = false;
                ReAllocation(_card, _cardSlot);
                return true;
            }
            else return false;
        }
        else if (CheckChildSlot(_card, _cardSlot))
        {
            Debug.Log("CHILD SLOT");
            _card.m_ChildSlot.atAceBase = false;
            ReAllocation(_card, _cardSlot);
            return true;
        }
        return false;
    }

    public List<RaycastResult> RaycastUI(Vector2 _pos)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = _pos;
        m_Raycaster.Raycast(m_PointerEventData, results);
        return results;
    }

    public void SendHoldingCardToOriginalPosition(Card _tmpCard)
    {
        _tmpCard.transform.DOMove(holdingCardOriginalSlot.transform.position, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            _tmpCard.m_CardImage.raycastTarget = true;
            _tmpCard.inOriginalPosition = true;
        });
        ReAllocateChildren(_tmpCard, holdingCardOriginalSlot);
    }

    void UpdateCardSlots(bool isOpen)
    {
        for (int i = 0; i < cardSlots.Count; i++)
        {
            cardSlots[i].raycastTarget = isOpen;
        }
    }

    void ReAllocation(Card _card, CardSlot newCardSlot)
    {
        _card.m_AllocatedSlot.DeAllocatted();
        _card.m_AllocatedSlot = newCardSlot;
        newCardSlot.Allocatted(_card);
        _card.transform.DOMove(newCardSlot.transform.position, 0.3f).OnComplete(() =>
        {
            _card.m_CardImage.raycastTarget = true;
            _card.inOriginalPosition = true;
        });
        
        if (_card.parent != null) _card.parent.child = null;
        _card.parent = newCardSlot.parentCard;
        if(newCardSlot.parentCard != null) newCardSlot.parentCard.child = _card;
        
        ReAllocateChildren(_card,newCardSlot);
    }

    void ReAllocateChildren(Card _parent, CardSlot _newParentSlot)
    {
        Card temporaryCard = _parent.child;
        int i = 1;
        while (temporaryCard != null && i < 20)
        {
            temporaryCard.transform.DOMove(_newParentSlot.transform.position + _parent.m_ChildSlot.transform.localPosition * i, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                temporaryCard.m_CardImage.raycastTarget = true;
                temporaryCard.inOriginalPosition = true;
            });
            temporaryCard = temporaryCard.child;
            i++;
        }
    }


    #region Slot Checks

    bool CheckAceBase(Card _card, CardSlot _cardSlot)
    {
        return _card.m_CardValue == CardValue.Ace && _card.m_CardType == _cardSlot.cardType;
    }
    
    bool CheckEmptySlot(Card _card, CardSlot _cardSlot)
    {
        return _card.m_CardValue == CardValue.King;
    }

    bool CheckChildSlot(Card _card, CardSlot _cardSlot)
    {
        bool canGo = (((_card.m_CardType == CardType.CLUB || _card.m_CardType == CardType.SPADE) &&
                      (_cardSlot.parentCard.m_CardType == CardType.HEART ||
                       _cardSlot.parentCard.m_CardType == CardType.DIAMOND)) ||
                     ((_cardSlot.parentCard.m_CardType == CardType.CLUB || _cardSlot.parentCard.m_CardType == CardType.SPADE) &&
                      (_card.m_CardType == CardType.HEART ||
                       _card.m_CardType == CardType.DIAMOND))) && _cardSlot.parentCard.m_CardValue == _card.m_CardValue + 1;
        
        bool aceBaseFit = _card.m_ChildSlot.atAceBase && _cardSlot.parentCard.m_CardType == _card.m_CardType &&
                          _cardSlot.parentCard.m_CardValue == _card.m_CardValue - 1;
        return canGo || aceBaseFit;
    }

    #endregion
    
}
