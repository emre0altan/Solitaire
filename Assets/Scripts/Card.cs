using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using Random = UnityEngine.Random;

[System.Serializable]
public class ClosedCards{
    public int openIndex;
    public bool availableToOpen, closedCardsEnded;
    public List<Card> untakenCards, leftSlotStack;
    public CardSlot openedCardLeft, openedCardMiddle, openedCardRight;
    public Image closedCardsLocation;
}

public enum CardType
{
    CLUB,
    DIAMOND,
    HEART,
    SPADE
}

public enum CardValue
{
    Ace,
    Deuce,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
}

public class Card : MonoBehaviour
{
    
    public CardType m_CardType;
    public CardValue m_CardValue;
    public Animator m_Animator;
    public Image m_CardImage;
    public CardSlot m_ChildSlot, m_AllocatedSlot;
    public Card parent, child;
    public bool isOpened = false, inOriginalPosition = true;
    public GameObject selectionBorder, backImage;
    
    [Space(20),SerializeField] Image leftTop;
    [SerializeField] Image rightTop, body;
    [SerializeField] Sprite[] cardTypeSprites, cardValueSprites;
    [SerializeField] Color red, black;

    private RectTransform rectTransform;
    private Vector3 originalPosition;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.position;
        UpdateTheCard();
    }

    void UpdateTheCard()
    {
        leftTop.sprite = cardValueSprites[(int) m_CardValue];
        int cardType = (int) m_CardType;
        m_ChildSlot.cardType = m_CardType;
        rightTop.sprite = cardTypeSprites[cardType];
        body.sprite = cardTypeSprites[cardType];
        if (cardType == 1 || cardType == 2) leftTop.color = red;
        else leftTop.color = black;
    }

    public void CloseCard()
    {
        m_CardImage.raycastTarget = false;
        m_Animator.SetTrigger("Close");
        isOpened = false;
    }

    public void OpenCard()
    {
        m_Animator.SetTrigger("Open");
        //DELAY WOULD BE GOOD
        m_CardImage.raycastTarget = true;
        isOpened = true;
    }
}
