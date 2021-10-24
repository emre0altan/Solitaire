using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

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
    public Image m_CardImage;
    public CardSlot m_CardSlot;
    public bool isOpened = false, inOriginalPosition = true;
    
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
        rightTop.sprite = cardTypeSprites[cardType];
        body.sprite = cardTypeSprites[cardType];
        if (cardType == 1 || cardType == 2) leftTop.color = red;
        else leftTop.color = black;
    }
}
