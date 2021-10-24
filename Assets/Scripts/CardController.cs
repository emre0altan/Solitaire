using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardController : MonoBehaviour,IPointerDownHandler,IDragHandler,IPointerUpHandler
{
    public static List<Image> cardSlots;
    
    [SerializeField] Card holdingCard;
    [SerializeField] GraphicRaycaster m_Raycaster;
    [SerializeField] EventSystem m_EventSystem;
    PointerEventData m_PointerEventData;

    private Vector3 holdingCardOriginalPos;
    
    
    public void OnDrag(PointerEventData eventData)
    {
        if (holdingCard != null)
        {
            holdingCard.transform.position = eventData.position;
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
         List<RaycastResult> results = RaycastUI(eventData.position);

        if (results.Count > 0 && results[0].gameObject.CompareTag("Card"))
        {
            holdingCard = results[0].gameObject.GetComponent<Card>();
            holdingCard.transform.SetAsLastSibling();
            if(holdingCard.inOriginalPosition) holdingCardOriginalPos = holdingCard.transform.position;
            holdingCard.inOriginalPosition = false;
            holdingCard.m_CardImage.raycastTarget = false;
            UpdateCardSlots(true);
            holdingCard.m_CardSlot.image.raycastTarget = false;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (holdingCard != null)
        {
            Card tmpCard = holdingCard;
            holdingCard = null;
            
             List<RaycastResult>results = RaycastUI(eventData.position);
            if (results.Count > 0 && results[0].gameObject.CompareTag("CardSlot"))
            {
                if (CanCardGoThere(tmpCard, results[0].gameObject.GetComponent<CardSlot>()))
                {
                    tmpCard.transform.DOMove(results[0].gameObject.transform.position, 0.3f).OnComplete(() =>
                    {
                        tmpCard.inOriginalPosition = true;
                    });
                    tmpCard.m_CardImage.raycastTarget = true;
                }
                else
                {
                    SendHoldingCardToOriginalPosition(tmpCard);
                }
            }
            else
            {
                SendHoldingCardToOriginalPosition(tmpCard);
            }
        }
        UpdateCardSlots(false);
    }

    public bool CanCardGoThere(Card holdingCard, CardSlot _cardSlot)
    {
        if (_cardSlot.cardSlotType == CardSlotType.AceBase)
        {
            if (holdingCard.m_CardValue == CardValue.Ace && holdingCard.m_CardType == _cardSlot.cardType) return true;
            else return false;
        }
        else if (_cardSlot.cardSlotType == CardSlotType.EmptySlot)
        {
            if (holdingCard.m_CardValue == CardValue.King) return true;
            else return false;
        }
        else //if (_cardSlot.cardSlotType == CardSlotType.AboveCard)
        {
            if ((holdingCard.m_CardType == CardType.CLUB || holdingCard.m_CardType == CardType.SPADE) &&
                (_cardSlot.belowCard.m_CardType == CardType.HEART || _cardSlot.belowCard.m_CardType == CardType.DIAMOND))
                return true;
            else if ((_cardSlot.belowCard.m_CardType == CardType.CLUB || _cardSlot.belowCard.m_CardType == CardType.SPADE) &&
                     (holdingCard.m_CardType == CardType.HEART || holdingCard.m_CardType == CardType.DIAMOND))
                return true;
            else
                return false;
        }
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
        _tmpCard.transform.DOMove(holdingCardOriginalPos, 0.3f).OnComplete(() =>
        {
            _tmpCard.inOriginalPosition = true;
        });
        _tmpCard.m_CardImage.raycastTarget = true;
    }

    void UpdateCardSlots(bool isOpen)
    {
        for (int i = 0; i < cardSlots.Count; i++)
        {
            cardSlots[i].raycastTarget = isOpen;
        }
    }
}
