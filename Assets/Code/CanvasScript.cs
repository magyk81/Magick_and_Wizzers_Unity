using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScript : MonoBehaviour
{
    private int camWidth, camHeight;
    [SerializeField]
    private float cursorToCardRatio;
    private float cardCursorWidth, cardCursorHeight, cardWidth, cardHeight;
    private RectTransform reticle, darkScreen, baseCard;
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
        public Coord Next { set { prev = value; } }
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
        private readonly int MAX_TICK_COUNT = 100;
        public Coord Next { set
            {
                cardPos.Next = value;
                if (tickCountdown <= 0 && value != Curr)
                    tickCountdown = MAX_TICK_COUNT;
            } }
        public Coord Curr { get { return cardPos.Curr; } }
        private RectTransform rectTran;
        private RawImage art;
        private int tickCountdown = 0;
        public HandCard(int idx, RectTransform baseCard, RectTransform parent,
            float width, float height, Coord initPos)
        {
            this.idx = idx;

            rectTran = Instantiate(baseCard.gameObject, parent)
                .GetComponent<RectTransform>();
            rectTran.gameObject.name = "Card " + ((idx < 9) ? "0" : "") + (idx + 1);
            rectTran.sizeDelta = new Vector2(width, height);
            rectTran.gameObject.SetActive(false);

            art = rectTran.gameObject.GetComponent<RawImage>();

            cardPos = new CardPos(initPos);
            this.initPos = initPos;
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
            int invTickCountdown = MAX_HAND_CARDS - tickCountdown;
            bool inGridYet = cardPos.Lerp(
                (float) invTickCountdown / (float) MAX_TICK_COUNT);
            rectTran.anchoredPosition = Curr.ToVec2();

            return inGridYet;
        }
    }
    private int cardHoverIdx = 0;

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
        }

        darkScreen.gameObject.SetActive(false);

        // Set up hand cards
        cardCursorWidth = ((float) camWidth) / (CARDS_PER_ROW + 2);
        cardWidth = cardCursorWidth / cursorToCardRatio;
        cardCursorHeight = cardCursorWidth * CARD_DIM_RATIO;
        cardHeight = cardWidth * CARD_DIM_RATIO;

        Coord initCardPos = Coord._(
            (int) (camWidth + cardWidth + 1) / 2,
            camHeight / 2);

        handCards = new HandCard[MAX_HAND_CARDS];
        handCardsParent.SetParent(GetComponent<RectTransform>());
        for (int i = 0; i < MAX_HAND_CARDS; i++)
        {
            handCards[i] = new HandCard(i, baseCard, handCardsParent,
                cardWidth, cardHeight, initCardPos);
        }

        HideHand();
        baseCard.gameObject.SetActive(false);
    }

    public void ShowHand() { handCardsParent.gameObject.SetActive(true); }
    public void HideHand() { handCardsParent.gameObject.SetActive(false); }

    public void SetHandCards(Card[] cards)
    {
        for (int i = 0; i < MAX_HAND_CARDS; i++)
        {
            if (i < cards.Length)
            {
                handCards[i].SetArt(cards[i]);
                handCards[i].Show();
                handCards[i].Next = Coord._(camWidth, camHeight / 2);
            }
            else handCards[i].Hide();
        }
    }

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
