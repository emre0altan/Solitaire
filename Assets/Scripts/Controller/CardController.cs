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
    public static CardController Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
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

    #region Callbacks

    public void OnMove(TouchInput touchInput)
    {
        if (holdingCard != null)
        {
            MoveWithChildren(touchInput);
        }
    }
    
    public void OnDown(TouchInput touchInput)
    {
        GameObject result = RaycastUI(touchInput.ScreenPosition);
        if(result == null) return;

        if (result.CompareTag("Card"))
        {
            HoldWithChildren(result.GetComponent<Card>());
        }
        else if (result.CompareTag("ClosedCards"))
        {
            CardDealer.Instance.OpenClosedCard();
        }
    }
    
    public void OnUp(TouchInput touchInput)
    {
        if (holdingCard != null)
        {
            Card tmpCard = holdingCard;
            holdingCard = null;
            GameObject result = RaycastUI(touchInput.ScreenPosition);
            if (result != null && result.CompareTag("CardSlot"))
            {
                CardSlot _cardSlot = result.GetComponent<CardSlot>();
                if (!TryToPutCard(tmpCard, _cardSlot)) SendHoldingCardToOriginalPosition(tmpCard);
            }
            else
                SendHoldingCardToOriginalPosition(tmpCard);

            tmpCard.selectionBorder.SetActive(false);
        }
        UpdateCardSlots(false);
    }

    #endregion
    
    #region Card Movements

    void MoveWithChildren(TouchInput touchInput)
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
    void HoldWithChildren(Card _card)
    {
        UpdateCardSlots(true);
        holdingCard = _card;
        Card tempCard = holdingCard;
        tempCard.transform.SetAsLastSibling();
        while (tempCard.child != null)
        {
            tempCard = tempCard.child;
            tempCard.transform.SetAsLastSibling();
            tempCard.m_CardImage.raycastTarget = false;
        }
            
        holdingCardOriginalSlot = holdingCard.m_AllocatedSlot;
        holdingCard.m_CardImage.raycastTarget = false;
        holdingCard.m_ChildSlot.image.raycastTarget = false;
        holdingCard.selectionBorder.SetActive(true);
        if(holdingCard.m_AllocatedSlot.image != null) holdingCard.m_AllocatedSlot.image.raycastTarget = false;
    }

    public bool TryToPutCard(Card _card, CardSlot _cardSlot)
    {
        if (_cardSlot.cardSlotType == CardSlotType.AceBase)
        {
            if (CheckAceBase(_card, _cardSlot))
            {
                //Relocation(_card, _cardSlot);
                MoveController.Instance.AddCommand(new MoveCommand(_card,_cardSlot));
                UpdateToAceBaseType(_card.m_ChildSlot, true);
                return true;
            }
            else return false;
        }
        else if (_cardSlot.cardSlotType == CardSlotType.EmptySlot)
        {
            if (CheckEmptySlot(_card, _cardSlot))
            {
                if (_card.m_ChildSlot.atAceBase) UpdateToAceBaseType(_card.m_ChildSlot, false);
                //Relocation(_card, _cardSlot);
                MoveController.Instance.AddCommand(new MoveCommand(_card,_cardSlot));
                return true;
            }
            else return false;
        }
        else if (CheckChildSlot(_card, _cardSlot))
        {
            if (_card.m_ChildSlot.atAceBase) UpdateToAceBaseType(_card.m_ChildSlot, false);
            //Relocation(_card, _cardSlot);
            MoveController.Instance.AddCommand(new MoveCommand(_card,_cardSlot));
            return true;
        }
        return false;
    }

    void UpdateToAceBaseType(CardSlot _cardSlot, bool isAceBase)
    {
        _cardSlot.atAceBase = isAceBase;
        _cardSlot.cardSlotType = isAceBase? CardSlotType.AceBase: CardSlotType.ChildSlot;
        _cardSlot.transform.localPosition = isAceBase? Vector3.zero : new Vector3(0, -50, 0);
    }
    
    public GameObject RaycastUI(Vector2 _pos)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = _pos;
        m_Raycaster.Raycast(m_PointerEventData, results);
        if (results.Count == 0) return null;
        else return results[0].gameObject;
    }
    
    void UpdateCardSlots(bool isOpen)
    {
        for (int i = 0; i < cardSlots.Count; i++)
        {
            cardSlots[i].raycastTarget = isOpen;
        }
    }
    
    public void SendHoldingCardToOriginalPosition(Card _tmpCard)
    {
        _tmpCard.transform.DOMove(holdingCardOriginalSlot.transform.position, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            _tmpCard.m_CardImage.raycastTarget = true;
        });
        RelocateChildren(_tmpCard, holdingCardOriginalSlot);
    }

    void Relocation(Card _card, CardSlot newCardSlot)
    {
        if (_card.m_AllocatedSlot.cardSlotType == CardSlotType.OpenedCardsRightTop) CardDealer.Instance.RemoveCard(_card);
        else _card.m_AllocatedSlot.DeAllocatted();
        
        _card.m_AllocatedSlot = newCardSlot;
        newCardSlot.Allocatted(_card);
        _card.transform.DOMove(newCardSlot.transform.position, 0.3f).OnComplete(() =>
        {
            _card.m_CardImage.raycastTarget = true;
        });
        
        if (_card.parent != null) _card.parent.child = null;
        _card.parent = newCardSlot.parentCard;
        if(newCardSlot.parentCard != null) newCardSlot.parentCard.child = _card;
        
        RelocateChildren(_card,newCardSlot);
    }

    void RelocateChildren(Card _parent, CardSlot _newParentSlot)
    {
        Card temporaryCard = _parent.child;
        int i = 1;
        while (temporaryCard != null && i < 20)
        {
            Card tmpp = temporaryCard;
            temporaryCard.transform.DOMove(_newParentSlot.transform.position + _parent.m_ChildSlot.transform.localPosition * i, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                tmpp.m_CardImage.raycastTarget = true;
            });
            temporaryCard = temporaryCard.child;
            i++;
        }
        
    }

    #endregion

    #region Slot Checks

    bool CheckAceBase(Card _card, CardSlot _cardSlot)
    {

        if (_cardSlot.parentCard != null)
        {
            Debug.Log("A-"+(_cardSlot.parentCard.m_CardValue == _card.m_CardValue - 1));
            return (_cardSlot.parentCard.m_CardValue == _card.m_CardValue - 1) && _card.m_CardType == _cardSlot.cardType;
        }
        else
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
        
        return canGo;
    }

    #endregion
    
}