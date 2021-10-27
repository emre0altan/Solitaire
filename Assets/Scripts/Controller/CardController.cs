using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardController : MonoBehaviour{
    public List<Card> allCards;
    public List<CardSlot> cardSlots;
    public static CardController Instance;
    public static int points = 0;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    public Text pointsText;
    public Card holdingCard;
    public GraphicRaycaster m_Raycaster;
    public EventSystem m_EventSystem;
    PointerEventData m_PointerEventData;

    private CardSlot holdingCardOriginalSlot;

    private Card tempChildCard;
    public Card latestHeartAceBase, latestDiamondAceBase, latestClubAceBase, latestSpadeAceBase;
    public CardSlot latestHeartAceBaseSlot, latestDiamondAceBaseSlot, latestClubAceBaseSlot, latestSpadeAceBaseSlot;
    

    private void Start(){
        allCards = new List<Card>();
        TouchManager.Instance.onTouchBegan += OnDown;
        TouchManager.Instance.onTouchMoved += OnMove;
        TouchManager.Instance.onTouchEnded += OnUp;
    }

    private void OnDestroy(){
        TouchManager.Instance.onTouchBegan -= OnDown;
        TouchManager.Instance.onTouchMoved -= OnMove;
        TouchManager.Instance.onTouchEnded -= OnUp;
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
                if (TryToPutCard(tmpCard, _cardSlot)) CheckGameCompleted();
                else SendHoldingCardToOriginalPosition(tmpCard);
            }
            else
                SendHoldingCardToOriginalPosition(tmpCard);

        }
        UpdateCardSlots(false);
    }

    #endregion

    public void UpdatePoints(int point){
        points += point;
        pointsText.text = points.ToString();
    }

    #region Complete

    

    
    public void CheckGameCompleted(){
        bool isCompleted = true, oneCardIgnored = false;
        int atAceBaseCount = 0;
        for (int i = 0; i < allCards.Count; i++){
            
            if (!allCards[i].isOpened || allCards[i].m_AllocatedSlot.cardSlotType == CardSlotType.OpenedCardsRightTop){
                isCompleted = false;
                break;
            };
            
            if (allCards[i].m_CardValue != CardValue.King &&
                allCards[i].m_AllocatedSlot.cardSlotType == CardSlotType.EmptySlot){
                if (oneCardIgnored){
                    isCompleted = false;
                    break;
                }
                else oneCardIgnored = true;
            }
            
            if (allCards[i].m_AllocatedSlot.cardSlotType == CardSlotType.AceBase) atAceBaseCount++;
        }

        if(isCompleted) StartCoroutine(CompletedCollectRoutine(atAceBaseCount));
    }

    IEnumerator CompletedCollectRoutine(int atAceBaseCount){
        for (int i = 0; i < allCards.Count - atAceBaseCount; i++){
            if (TryTakeForHeart())
                yield return new WaitForSeconds(0.1f);
            if (TryTakeForDiamond()) 
                yield return new WaitForSeconds(0.1f);
            if (TryTakeForClub()) 
                yield return new WaitForSeconds(0.1f);
            if (TryTakeForSpade()) 
                yield return new WaitForSeconds(0.1f);
        }

        GameManager.Instance.Completed(MoveController.Instance.totalMoves,0, points);
    }

    bool TryTakeForHeart(){ 
        Debug.Log("CHECKING HEART");
        if (latestHeartAceBase == null){
            if (allCards[26].child == null){
                CompleteMove(allCards[26],latestHeartAceBaseSlot);
                latestHeartAceBase = allCards[26];
                Debug.Log("CHECKING HEART - NO CARD TAKING ACE");
                return true;
            }
        }
        else if(latestSpadeAceBase.m_CardValue != CardValue.King && allCards[26 + (int) latestHeartAceBase.m_CardValue + 1].child == null){
                CompleteMove(allCards[26 + (int) latestHeartAceBase.m_CardValue + 1],latestHeartAceBaseSlot);
                latestHeartAceBase = allCards[26 + (int) latestHeartAceBase.m_CardValue + 1];
                Debug.Log("CHECKING HEART - TAKING CARD");
                return true;
        }
        return false;
    }
    
    bool TryTakeForDiamond(){
        Debug.Log("CHECKING DIAMOND");
        if (latestDiamondAceBase == null){
            if ( allCards[13].child == null){
                CompleteMove(allCards[13],latestDiamondAceBaseSlot);
                latestDiamondAceBase = allCards[13];
                return true;
            }
        }
        else if(latestSpadeAceBase.m_CardValue != CardValue.King && allCards[13 + (int) latestDiamondAceBase.m_CardValue + 1].child == null){
            CompleteMove(allCards[13 + (int) latestDiamondAceBase.m_CardValue + 1],latestDiamondAceBaseSlot);
            latestDiamondAceBase = allCards[13 + (int) latestDiamondAceBase.m_CardValue + 1];
            return true;
        }
        return false;
    }
    
    bool TryTakeForClub(){
        Debug.Log("CHECKING CLUB");

        if (latestClubAceBase == null){
            if (allCards[0].child == null){
                CompleteMove(allCards[0],latestClubAceBaseSlot);
                latestClubAceBase = allCards[0];
                return true;
            }
        }
        else  if(latestSpadeAceBase.m_CardValue != CardValue.King && allCards[0 + (int) latestClubAceBase.m_CardValue + 1].child == null){
            CompleteMove(allCards[0 + (int) latestClubAceBase.m_CardValue + 1],latestClubAceBaseSlot);
            latestClubAceBase = allCards[0 + (int) latestClubAceBase.m_CardValue + 1];
            return true;
        }
        return false;
    }
    
    bool TryTakeForSpade(){
        Debug.Log("CHECKING SPADE");

        if (latestSpadeAceBase == null){
            if (allCards[39].child == null){
                CompleteMove(allCards[39],latestSpadeAceBaseSlot);
                latestSpadeAceBase = allCards[39];
                return true;
            }
        }
        else if(latestSpadeAceBase.m_CardValue != CardValue.King && allCards[39 + (int) latestSpadeAceBase.m_CardValue + 1].child == null){
            CompleteMove(allCards[39 + (int) latestSpadeAceBase.m_CardValue + 1],latestSpadeAceBaseSlot);
            latestSpadeAceBase = allCards[39 + (int) latestSpadeAceBase.m_CardValue + 1];
            return true;
        }
        return false;
    }

    void CompleteMove(Card _card, CardSlot _cardSlot){
        _card.transform.DOMove(_cardSlot.transform.position, 1f).SetEase(Ease.OutCubic);
        _card.m_AllocatedSlot = _cardSlot;
        _cardSlot.currentCard = _card;
        _card.OpenCard();
        _card.transform.SetAsLastSibling();
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
        if(holdingCard.m_AllocatedSlot.image != null) holdingCard.m_AllocatedSlot.image.raycastTarget = false;
    }

    public bool TryToPutCard(Card _card, CardSlot _cardSlot)
    {
        if (_cardSlot.cardSlotType == CardSlotType.AceBase)
        {
            if (CheckAceBase(_card, _cardSlot)){
                MoveController.Instance.AddCommand(new MoveCommand(_card,_cardSlot));
                return true;
            }
            else return false;
        }
        else if (_cardSlot.cardSlotType == CardSlotType.EmptySlot)
        {
            if (CheckEmptySlot(_card, _cardSlot))
            {
                MoveController.Instance.AddCommand(new MoveCommand(_card,_cardSlot));
                return true;
            }
            else return false;
        }
        else if (CheckChildSlot(_card, _cardSlot))
        {
            MoveController.Instance.AddCommand(new MoveCommand(_card,_cardSlot));
            return true;
        }
        return false;
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
            cardSlots[i].image.raycastTarget = isOpen;
            cardSlots[i].GetComponent<CardSlot>().availableImage.SetActive(isOpen);
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

    public static bool CheckAceBase(Card _card, CardSlot _cardSlot){
        if (_card.child != null) return false;
        
        if (_cardSlot.parentCard != null)
        {
            Debug.Log("A-"+(_cardSlot.parentCard.m_CardValue == _card.m_CardValue - 1));
            return (_cardSlot.parentCard.m_CardValue == _card.m_CardValue - 1) && _card.m_CardType == _cardSlot.cardType;
        }
        else
            return _card.m_CardValue == CardValue.Ace && _card.m_CardType == _cardSlot.cardType;
    }
    
    public static bool CheckEmptySlot(Card _card, CardSlot _cardSlot)
    {
        return _card.m_CardValue == CardValue.King;
    }

    public static bool CheckChildSlot(Card _card, CardSlot _cardSlot)
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
