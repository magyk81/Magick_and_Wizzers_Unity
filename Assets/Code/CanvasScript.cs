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
    private RectTransform reticle, darkScreen, card;
    public RectTransform Reticle { get { return reticle; } }
    public RectTransform DarkScreen { get { return darkScreen; } }

    private readonly float CARD_DIM_RATIO = 1.4F;
    private readonly int MAX_HAND_CARDS = 60, CARDS_PER_ROW = 5;
    private RectTransform[] handCards;
    private class CardPos
    {
        private Coord prev, next, curr;
        public Coord Next { set { prev = value; } }
        public Coord Curr { get { return curr; } }
        public void Lerp(float dist)
        {
            curr = Coord.Lerp(prev, next, dist);
            if (curr == next) prev = curr.Copy();
        }
        public CardPos(Coord coord)
        {
            prev = coord.Copy();
            curr = coord.Copy();
            next = coord.Copy();
        }
    }
    private int cardHoverIdx = 0;

    private readonly int MAX_TICK_COUNT = 100;
    private int tickCountdown = 0;

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
            else if (child.gameObject.name == "Card")
                card = child.gameObject.GetComponent<RectTransform>();
        }

        darkScreen.gameObject.SetActive(false);

        // Set up hand cards
        cardCursorWidth = ((float) camWidth) / (CARDS_PER_ROW + 2);
        cardWidth = cardCursorWidth / cursorToCardRatio;
        cardCursorHeight = cardCursorWidth * CARD_DIM_RATIO;
        cardHeight = cardWidth * CARD_DIM_RATIO;

        handCards = new RectTransform[MAX_HAND_CARDS];
        Transform handCardsGroup = new GameObject().GetComponent<Transform>();
        handCardsGroup.gameObject.name = "Hand Cards";
        handCardsGroup.SetParent(GetComponent<Transform>());
        for (int i = 0; i < MAX_HAND_CARDS; i++)
        {
            handCards[i] = Instantiate(card, handCardsGroup);
            handCards[i].name = "Card " + ((i < 9) ? "0" : "") + (i + 1);
            handCards[i].sizeDelta = new Vector2(cardWidth, cardHeight);
            handCards[i].gameObject.SetActive(false);
        }

        card.gameObject.SetActive(false);
    }

    public void SetCardArt(Card[] cards)
    {
        for (int i = 0; i < MAX_HAND_CARDS; i++)
        {
            if (i < cards.Length)
            {
                handCards[i].gameObject.GetComponent<RawImage>().texture
                    = cards[i].Art;
            }
            else handCards[i].gameObject.SetActive(false);
        }
    }

    private void SetCardPositions()
    {
        int cardHoverRow = cardHoverIdx / CARDS_PER_ROW,
            cardHoverColumn = cardHoverIdx - (cardHoverIdx * CARDS_PER_ROW);
        
        //for (int i = 0; i < )
    }
}
