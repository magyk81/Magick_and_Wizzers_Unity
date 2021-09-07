using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScript : MonoBehaviour
{
    private int camWidth, camHeight;
    [SerializeField]
    private float cursorThickness;
    private float cardWidth, cardHeight,
        cardSlotWidth, cardSlotHeight;
    private RectTransform reticle, darkScreen, baseCard, cardCursor,
        cardBeingPlayed;
    public RectTransform Reticle { get { return reticle; } }
    public RectTransform DarkScreen { get { return darkScreen; } }

    private static readonly float CARD_DIM_RATIO = 1.4F;
    private static readonly int MAX_HAND_CARDS = 60, CARDS_PER_ROW = 5;
    private HandCard[] handCards;
    private RectTransform handCardsParent;
    private class CardPos
    {
        private bool inGridYet;
        public bool InGridYet { get { return inGridYet; } }
        private Coord prev, next, curr;
        public Coord Next { set { next = value.Copy(); prev = curr.Copy(); } }
        public Coord Curr { get { return curr; } }
        public bool Lerp(float dist)
        {
            curr = Coord.Lerp(prev, next, dist);
            if (curr == next)
            {
                prev = curr.Copy();
                inGridYet = true;
            }
            return inGridYet;
        }
        public CardPos(Coord coord)
        {
            prev = coord.Copy();
            curr = coord.Copy();
            next = coord.Copy();
            inGridYet = false;
        }
    }
    private class HandCard
    {
        private int idx, row, column;
        private CardPos cardPos;
        private Coord initPos;
        private readonly int MAX_TICK_COUNT = 50;
        public Coord Next { set
            {
                cardPos.Next = value;
                //if (tickCountdown <= 0 && value != Curr)
                    tickCountdown = MAX_TICK_COUNT;
            } }
        public Coord Curr { get { return cardPos.Curr; } }
        private RectTransform rectTran;
        private RawImage art;
        public Texture Art { get { return art.texture; } }
        private int tickCountdown = 0;
        public HandCard(int idx, RectTransform baseCard, RectTransform parent,
            float width, float height, Coord initPos)
        {
            this.idx = idx;

            rectTran = Instantiate(baseCard.gameObject, parent)
                .GetComponent<RectTransform>();
            rectTran.gameObject.name
                = "Card " + ((idx < 9) ? "0" : "") + (idx + 1);
            rectTran.sizeDelta = new Vector2(width, height);
            rectTran.gameObject.SetActive(false);

            art = rectTran.gameObject.GetComponent<RawImage>();

            cardPos = new CardPos(initPos);
            this.initPos = initPos;
            rectTran.anchoredPosition = Curr.ToVec2();
        }
        public void Show() { rectTran.gameObject.SetActive(true); }
        public void Hide()
        {
            if (rectTran.gameObject.activeSelf)
            {
                cardPos = new CardPos(initPos);
                rectTran.gameObject.SetActive(false);
            }
        }
        public void SetArt(Card card) { art.texture = card.Art; }
        public bool Update(bool otherOutGridAlreadyUpdated)
        {
            // Another hand card that's NOT in the grid yes has moved
            if (otherOutGridAlreadyUpdated) return false;

            // It's already in the grid and/or has not been assigned to move.
            if (tickCountdown <= 0) return true;

            tickCountdown--;
            int invTickCountdown = MAX_TICK_COUNT - tickCountdown;
            bool inGridYet = cardPos.Lerp(
                (float) invTickCountdown / (float) MAX_TICK_COUNT);
            rectTran.anchoredPosition = Curr.ToVec2();

            return inGridYet;
        }
    }
    private int cardCursorIdx = 0, cardCount = 0;
    private Piece handPiece;

    public void InitCanvObjs(int camWidth, int camHeight)
    {
        this.camWidth = camWidth;
        this.camHeight = camHeight;

        foreach (Transform child in GetComponent<Transform>())
        {
            if (child.gameObject.name == "Reticle")
                reticle = child.gameObject.GetComponent<RectTransform>();
            else if (child.gameObject.name == "Dark Screen")
                darkScreen = child.gameObject.GetComponent<RectTransform>();
            else if (child.gameObject.name == "Hand Cards")
            {
                handCardsParent = child.gameObject
                    .GetComponent<RectTransform>();
                foreach (
                    Transform handCardsChild in handCardsParent)
                {
                    if (handCardsChild.gameObject.name == "Card")
                    {
                        baseCard = handCardsChild.gameObject
                            .GetComponent<RectTransform>();
                    }
                }
            }
            else if (child.gameObject.name == "Card Cursor")
                cardCursor = child.gameObject.GetComponent<RectTransform>();
            else if (child.gameObject.name == "Card Being Played")
                cardBeingPlayed = child.gameObject.GetComponent<RectTransform>();
        }

        darkScreen.gameObject.SetActive(false);

        // Set up hand cards
        cardWidth = ((float) camWidth) / (CARDS_PER_ROW + 2);
        cardHeight = cardWidth * CARD_DIM_RATIO;

        cardCursor.sizeDelta = new Vector2(
            cardWidth + cursorThickness, cardHeight + cursorThickness);
        cardBeingPlayed.sizeDelta = new Vector2(
            cardWidth + cursorThickness, cardHeight + cursorThickness);

        Coord initCardPos = Coord._(
            (int) (camWidth + cardWidth + 1) / 2, 0);

        handCards = new HandCard[MAX_HAND_CARDS];
        handCardsParent.SetParent(GetComponent<RectTransform>());
        for (int i = 0; i < MAX_HAND_CARDS; i++)
        {
            handCards[i] = new HandCard(i, baseCard, handCardsParent,
                cardWidth, cardHeight, initCardPos);
        }

        cardCursor.anchoredPosition = GetCardPos(0).ToVec2();
        cardBeingPlayed.anchoredPosition
            = Coord._(0, GetCardPos(0).Z).ToVec2();

        HideHand();
        cardCursor.gameObject.SetActive(false);
        cardBeingPlayed.gameObject.SetActive(false);
        baseCard.gameObject.SetActive(false);
    }

    public void ShowHand()
    {
        handCardsParent.gameObject.SetActive(true);
        cardCursor.gameObject.SetActive(cardCount > 0);
    }
    public void HideHand()
    {
        handCardsParent.gameObject.SetActive(false);
        cardCursor.gameObject.SetActive(false);
    }

    public void SetHandPiece(Piece handPiece)
    {
        Card[] cards = handPiece.Hand;
        if (cards == null) return;

        cardCount = cards.Length;
        if (cardCursorIdx >= cardCount) cardCursorIdx = cardCount - 1;
        else if (cardCount > 0 && cardCursorIdx == -1) cardCursorIdx = 0;
        cardCursor.gameObject.SetActive(cardCount > 0
            && handCardsParent.gameObject.activeSelf);

        for (int i = 0; i < MAX_HAND_CARDS; i++)
        {
            if (i < cards.Length)
            {
                handCards[i].SetArt(cards[i]);
                handCards[i].Show();
                handCards[i].Next = GetCardPos(i);
            }
            else handCards[i].Hide();
        }

        this.handPiece = handPiece;
    }

    private Coord GetCardPos(int idx)
    {
        bool neg = idx < 0;
        int u = (idx + MAX_HAND_CARDS) % CARDS_PER_ROW;
        int x = (-camWidth / 2)
            + (int) (cardWidth * (1.5F + (u % CARDS_PER_ROW)));
        int y = (int) (-cardHeight * (idx / CARDS_PER_ROW));
        if (neg && idx % CARDS_PER_ROW != 0) y += (int) cardHeight;
        return Coord._(x, y);
    }

    public void MoveCursor(int x_move, int y_move)
    {
        if (cardCount == 0) return;

        if (x_move == Util.LEFT)
        {
            // Cursor is a progressive woke marxist punk radical feminist
            if (cardCursorIdx % CARDS_PER_ROW == 0)
            {
                // Turn the cursor into an evil capitalist
                cardCursorIdx += CARDS_PER_ROW - 1;
                if (cardCursorIdx >= cardCount) cardCursorIdx = cardCount - 1;
            }
            else cardCursorIdx--;
        }
        else if (x_move == Util.RIGHT)
        {
            // Cursor is a white cishet 22-inch cock slung over his shoulder
            if (cardCursorIdx % CARDS_PER_ROW == CARDS_PER_ROW - 1)
            {
                // Turn the cursor into an LGBTQA+ woke-folk ally
                cardCursorIdx -= CARDS_PER_ROW - 1;
            }
            // When the right ain't right
            else if (cardCursorIdx == cardCount - 1)
            {
                cardCursorIdx -= (cardCount % CARDS_PER_ROW) - 1;
            }
            else cardCursorIdx++;
        }
        if (y_move == Util.UP)
        {
            cardCursorIdx -= CARDS_PER_ROW;

            // Cursor is a Russian
            if (cardCursorIdx < 0) cardCursorIdx
                += cardCount + (cardCount % CARDS_PER_ROW) + 1;
            if (cardCursorIdx >= cardCount) cardCursorIdx -= CARDS_PER_ROW;
        }
        else if (y_move == Util.DOWN)
        {
            cardCursorIdx += CARDS_PER_ROW;

            // Cursor is an American
            if (cardCursorIdx >= cardCount) cardCursorIdx
                -= cardCount + (cardCount % CARDS_PER_ROW) + 1;
            if (cardCursorIdx < 0) cardCursorIdx += CARDS_PER_ROW;
        }

        cardCursor.anchoredPosition
            = GetCardPos(cardCursorIdx % CARDS_PER_ROW).ToVec2();
        
        if (y_move == Util.UP || y_move == Util.DOWN)
        {
            // Move the cards up and down instead of the cursor.
            int row = cardCursorIdx / CARDS_PER_ROW;
            for (int i = 0; i < cardCount; i++)
            {
                handCards[i].Next = GetCardPos(i - (row * CARDS_PER_ROW));
            }
        }
    }

    public int GetHoverIdx() { return cardCursorIdx; }
    public void DisplayPlayCard(int idx)
    {
        if (idx < cardCount && idx >= 0)
        {
            cardBeingPlayed.gameObject.SetActive(true);
            cardBeingPlayed.gameObject.GetComponent<RawImage>().texture
                = handCards[idx].Art;
        }
    }
    public void HidePlayCard() { cardBeingPlayed.gameObject.SetActive(false); }
    public Piece GetHandPiece() { return handPiece; }

    public void UpdateHandCards()
    {
        bool otherOutGridAlreadyUpdated = false;
        foreach (HandCard handCard in handCards)
        {
            bool inGridYet = handCard.Update(otherOutGridAlreadyUpdated);
            if (!inGridYet) otherOutGridAlreadyUpdated = true;
        }
    }
}
