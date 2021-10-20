using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UX_Hand : MonoBehaviour
{
    [SerializeField]
    private RectTransform baseCard, cursor;

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

    private class UX_Card
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
        public UX_Card(int idx, RectTransform baseCard, RectTransform parent,
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

    private static readonly float CARD_DIM_RATIO = 1.4F;
    private static readonly int MAX_HAND_CARDS = 60, CARDS_PER_ROW = 5;

    private UX_Card[] cards = new UX_Card[MAX_HAND_CARDS];
    private RectTransform playCard;
    private int cursorIdx = 0, count = 0;
    private Piece piece;

    [SerializeField]
    private float cursorThickness;
    private float cardWidth, cardHeight;

    private RectTransform tra, cardParent;
    private Coord camDims;
    private bool isShown = false, setupDone = false;

    public void Show(Piece piece)
    {
        if (setupDone && piece != null)
        {
            this.piece = piece;

            // Setup cards using the hovered piece's hand.
            Card[] _cards = piece.Hand;
            if (_cards == null) return;

            count = _cards.Length;
            if (cursorIdx >= count) cursorIdx = count - 1;
            else if (count > 0 && cursorIdx == -1) cursorIdx = 0;
            cursor.gameObject.SetActive(count > 0
                && cardParent.gameObject.activeSelf);

            for (int i = 0; i < MAX_HAND_CARDS; i++)
            {
                if (i < _cards.Length)
                {
                    cards[i].SetArt(_cards[i]);
                    cards[i].Show();
                    cards[i].Next = GetCardPos(i);
                }
                else cards[i].Hide();
            }

            // Show parent and cursor.
            cardParent.gameObject.SetActive(true);
            cursor.gameObject.SetActive(count > 0);
            isShown = true;
        }
    }
    public void Hide(bool hideAll = true)
    {
        if (setupDone)
        {
            cardParent.gameObject.SetActive(false);
            cursor.gameObject.SetActive(false);
            if (hideAll) playCard.gameObject.SetActive(false);
            isShown = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        tra = GetComponent<RectTransform>();

        Rect pixelRect = GetComponent<Canvas>().pixelRect;
        camDims = Coord._((int) pixelRect.width, (int) pixelRect.height);

        cardWidth = ((float) camDims.X) / (CARDS_PER_ROW + 2);
        cardHeight = cardWidth * CARD_DIM_RATIO;

        // Setup play-card visualization.
        playCard = Instantiate(baseCard, tra).GetComponent<RectTransform>();
        playCard.sizeDelta = new Vector2(
            cardWidth + cursorThickness, cardHeight + cursorThickness);
        playCard.anchoredPosition
            = Coord._(0, (int) (camDims.Z - cardHeight) / 2).ToVec2();
        playCard.gameObject.SetActive(false);

        // Setup parent for cards.
        GameObject cardParentObj = new GameObject();
        cardParentObj.GetComponent<Transform>().parent = tra;
        cardParentObj.AddComponent<RectTransform>();
        cardParentObj.GetComponent<RectTransform>().anchoredPosition
            = new Vector2(0, 0);
        cardParentObj.name = "Hand";
        cardParent = cardParentObj.GetComponent<RectTransform>();

        // Setup cursor.
        cursor = Instantiate(cursor, tra).GetComponent<RectTransform>();
        cursor.sizeDelta = new Vector2(
            cardWidth + cursorThickness, cardHeight + cursorThickness);
        cursor.sizeDelta = new Vector2(
            cardWidth + cursorThickness, cardHeight + cursorThickness);
        cursor.anchoredPosition = GetCardPos(0).ToVec2();
        cursor.gameObject.SetActive(false);        

        // Setup cards.
        Coord initCardPos = Coord._(
            (int) (camDims.X + cardWidth + 1) / 2, 0);
        for (int i = 0; i < MAX_HAND_CARDS; i++)
        {
            cards[i] = new UX_Card(i, baseCard, cardParent,
                cardWidth, cardHeight, initCardPos);
        }
        baseCard.gameObject.SetActive(false);

        setupDone = true;
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        if (isShown)
        {
            bool otherOutGridAlreadyUpdated = false;
            foreach (UX_Card card in cards)
            {
                bool inGridYet = card.Update(otherOutGridAlreadyUpdated);
                if (!inGridYet) otherOutGridAlreadyUpdated = true;
            }
        }
    }

    public void MoveCursor(int x_move, int y_move)
    {
        if (count == 0) return;

        if (x_move == Util.LEFT)
        {
            // Cursor is a progressive woke marxist punk radical feminist
            if (cursorIdx % CARDS_PER_ROW == 0)
            {
                // Turn the cursor into an evil capitalist
                cursorIdx += CARDS_PER_ROW - 1;
                if (cursorIdx >= count) cursorIdx = count - 1;
            }
            else cursorIdx--;
        }
        else if (x_move == Util.RIGHT)
        {
            // Cursor is a white cishet 22-inch cock slung over his shoulder
            if (cursorIdx % CARDS_PER_ROW == CARDS_PER_ROW - 1)
            {
                // Turn the cursor into an LGBTQA+ woke-folk ally
                cursorIdx -= CARDS_PER_ROW - 1;
            }
            // When the right ain't right
            else if (cursorIdx == count - 1)
            {
                cursorIdx -= (count % CARDS_PER_ROW) - 1;
            }
            else cursorIdx++;
        }
        if (y_move == Util.UP)
        {
            cursorIdx -= CARDS_PER_ROW;

            // Cursor is a Russian
            if (cursorIdx < 0) cursorIdx
                += count + (count % CARDS_PER_ROW);
            if (cursorIdx >= count) cursorIdx -= CARDS_PER_ROW;
        }
        else if (y_move == Util.DOWN)
        {
            cursorIdx += CARDS_PER_ROW;

            // Cursor is an American
            if (cursorIdx >= count) cursorIdx
                -= count + (count % CARDS_PER_ROW);
            if (cursorIdx < 0) cursorIdx += CARDS_PER_ROW;
        }

        cursor.anchoredPosition
            = GetCardPos(cursorIdx % CARDS_PER_ROW).ToVec2();
        
        if (y_move == Util.UP || y_move == Util.DOWN)
        {
            // Move the cards up and down instead of the cursor.
            int row = cursorIdx / CARDS_PER_ROW;
            for (int i = 0; i < count; i++)
            {
                cards[i].Next = GetCardPos(i - (row * CARDS_PER_ROW));
            }
        }
    }

    public Card Select()
    {
        if (cursorIdx < count && cursorIdx >= 0)
        {
            playCard.gameObject.GetComponent<RawImage>().texture
                = cards[cursorIdx].Art;
            playCard.gameObject.SetActive(true);
            return piece.Hand[cursorIdx];
        }
        return null;
    }

    private Coord GetCardPos(int idx)
    {
        bool neg = idx < 0;
        int u = (idx + MAX_HAND_CARDS) % CARDS_PER_ROW;
        int x = (-camDims.X / 2)
            + (int) (cardWidth * (1.5F + (u % CARDS_PER_ROW)));
        int y = (int) (-cardHeight * (idx / CARDS_PER_ROW));
        if (neg && idx % CARDS_PER_ROW != 0) y += (int) cardHeight;
        return Coord._(x, y);
    }
}
