/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using UnityEngine;
using UnityEngine.UI;

public class UX_Hand : MonoBehaviour
{
    private static readonly float CARD_DIM_RATIO = 1.4F, CURSOR_THICKNESS = 25;
    private static readonly int MAX_HAND_CARDS = 60, CARDS_PER_ROW = 5;

    [SerializeField]
    private RectTransform baseCard, cursor;
    private readonly UX_Card[] mCards = new UX_Card[MAX_HAND_CARDS];
    // IDs of the last card that was selected and its caster.
    private int mPlayCardID = -1, mCasterID = -1;
    private RectTransform mPlayCardTran;
    private int mCursorIdx = 0, mCount = 0;
    private UX_Piece mPiece;
    private float mCardWidth, mCardHeight;
    private RectTransform mTran, mCardParent;
    private Coord mCamDims;
    private bool mIsShown = false, mSetupDone = false;

    public int PlayCardID {
        get {
            int temp = mPlayCardID;
            mPlayCardID = -1;
            return temp;
        }
    }
    public int CasterID {
        get {
            int temp = mCasterID;
            mCasterID = -1;
            return temp;
        }
    }

    public void Show(UX_Piece piece) {
        if (mSetupDone && piece != null) {
            mPiece = piece;

            // Setup cards using the hovered piece's hand.
            Card[] cards = piece.Hand;
            if (cards == null) return;

            mCount = cards.Length;
            if (mCursorIdx >= mCount) mCursorIdx = mCount - 1;
            else if (mCount > 0 && mCursorIdx == -1) mCursorIdx = 0;
            cursor.gameObject.SetActive(mCount > 0 && mCardParent.gameObject.activeSelf);

            for (int i = 0; i < MAX_HAND_CARDS; i++) {
                if (i < cards.Length) {
                    mCards[i].SetArt(cards[i]);
                    mCards[i].Show();
                    mCards[i].Next = GetCardPos(i);
                } else mCards[i].Hide();
            }

            // Show parent and cursor.
            mCardParent.gameObject.SetActive(true);
            cursor.gameObject.SetActive(mCount > 0);

            // Update cursor position.
            MoveCursor(-1, Util.UP);
            MoveCursor(-1, Util.DOWN);

            mIsShown = true;
        }
    }

    public void Hide(bool hideAll = true) {
        if (mSetupDone) {
            //foreach (UX_Card card in cards) { card.Finish(); }

            mCardParent.gameObject.SetActive(false);
            cursor.gameObject.SetActive(false);
            if (hideAll) mPlayCardTran.gameObject.SetActive(false);
            mIsShown = false;
        }
    }

    public void MoveCursor(int x_move, int y_move) {
        if (mCount == 0) return;

        if (x_move == Util.LEFT) {
            // The cursor is on the left-most column.
            if (mCursorIdx % CARDS_PER_ROW == 0) {
                // Put the cursor on the right-most column.
                mCursorIdx += CARDS_PER_ROW - 1;
                if (mCursorIdx >= mCount) mCursorIdx = mCount - 1;
            }
            else mCursorIdx--;
        } else if (x_move == Util.RIGHT) {
            // The cursor is along the right-most column. Put the cursor on the left-most column.
            if (mCursorIdx % CARDS_PER_ROW == CARDS_PER_ROW - 1) mCursorIdx -= CARDS_PER_ROW - 1;
            // When the right ain't right.
            else if (mCursorIdx == mCount - 1) mCursorIdx -= (mCount % CARDS_PER_ROW) - 1;
            else mCursorIdx++;
        }
        if (y_move == Util.UP) {
            mCursorIdx -= CARDS_PER_ROW;

            // The cursur is on the top row.
            if (mCursorIdx < 0) mCursorIdx += mCount + (mCount % CARDS_PER_ROW);
            if (mCursorIdx >= mCount) mCursorIdx -= CARDS_PER_ROW;
        } else if (y_move == Util.DOWN) {
            mCursorIdx += CARDS_PER_ROW;

            // The cursor is on the bottom row.
            if (mCursorIdx >= mCount) mCursorIdx -= mCount + (mCount % CARDS_PER_ROW);
            if (mCursorIdx < 0) mCursorIdx += CARDS_PER_ROW;
        }

        cursor.anchoredPosition = GetCardPos(mCursorIdx % CARDS_PER_ROW).ToVec2();
        
        if (y_move == Util.UP || y_move == Util.DOWN) {
            // Move the cards up and down instead of the cursor.
            int row = mCursorIdx / CARDS_PER_ROW;
            for (int i = 0; i < mCount; i++) { mCards[i].Next = GetCardPos(i - (row * CARDS_PER_ROW)); }
        }
    }

    /// <returns>
    /// Whether a card was selected.
    /// </returns>
    public bool Select() {
        if (mCursorIdx < mCount && mCursorIdx >= 0) {
            if (mPiece.Hand.Length >= 0) {
                mPlayCardTran.gameObject.GetComponent<RawImage>().texture = mCards[mCursorIdx].Art;
                mPlayCardTran.gameObject.SetActive(true);
                Hide(false);
                mPlayCardID = mPiece.Hand[mCursorIdx].ID;
                mCasterID = mPiece.PieceID;
                // return new int[] { mPiece.Hand[mCursorIdx].ID, mPiece.PieceID };
                return true;
            }
            // Hand is empty.
            return false;
        }
        // Cursor is out of bounds (why?)
        Debug.LogError("CursorIdx: " + mCursorIdx + " is out of bounds (Count: " + mCount + ")");
        return false;
    }

    private Coord GetCardPos(int idx) {
        bool neg = idx < 0;
        int u = (idx + MAX_HAND_CARDS) % CARDS_PER_ROW;
        int x = (-mCamDims.X / 2) + (int) (mCardWidth * (1.5F + (u % CARDS_PER_ROW)));
        int y = (int) (-mCardHeight * (idx / CARDS_PER_ROW));
        if (neg && idx % CARDS_PER_ROW != 0) y += (int) mCardHeight;
        return Coord._(x, y);
    }

    // Start is called before the first frame update
    private void Start() {
        mTran = GetComponent<RectTransform>();

        Rect pixelRect = GetComponent<Canvas>().pixelRect;
        mCamDims = Coord._((int) pixelRect.width, (int) pixelRect.height);

        mCardWidth = ((float) mCamDims.X) / (CARDS_PER_ROW + 2);
        mCardHeight = mCardWidth * CARD_DIM_RATIO;

        // Setup play-card visualization.
        mPlayCardTran = Instantiate(baseCard, mTran).GetComponent<RectTransform>();
        mPlayCardTran.sizeDelta = new Vector2(mCardWidth + CURSOR_THICKNESS, mCardHeight + CURSOR_THICKNESS);
        mPlayCardTran.anchoredPosition = Coord._(0, (int) (mCamDims.Z - mCardHeight) / 2).ToVec2();
        mPlayCardTran.gameObject.SetActive(false);

        // Setup parent for cards.
        GameObject cardParentObj = new GameObject();
        cardParentObj.GetComponent<Transform>().parent = mTran;
        cardParentObj.AddComponent<RectTransform>();
        cardParentObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        cardParentObj.name = "Hand";
        mCardParent = cardParentObj.GetComponent<RectTransform>();

        // Setup cursor.
        cursor = Instantiate(cursor, mTran).GetComponent<RectTransform>();
        cursor.sizeDelta = new Vector2(mCardWidth + CURSOR_THICKNESS, mCardHeight + CURSOR_THICKNESS);
        cursor.sizeDelta = new Vector2(mCardWidth + CURSOR_THICKNESS, mCardHeight + CURSOR_THICKNESS);
        cursor.anchoredPosition = GetCardPos(0).ToVec2();
        cursor.gameObject.SetActive(false);        

        // Setup cards.
        Coord initCardPos = Coord._((int) (mCamDims.X + mCardWidth + 1) / 2, 0);
        for (int i = 0; i < MAX_HAND_CARDS; i++) {
            mCards[i] = new UX_Card(i, baseCard, mCardParent, mCardWidth, mCardHeight, initCardPos);
        }
        baseCard.gameObject.SetActive(false);

        mSetupDone = true;
        Hide();
    }

    // Update is called once per frame
    private void Update() {
        if (mIsShown) {
            bool otherOutGridAlreadyUpdated = false;
            foreach (UX_Card card in mCards) {
                bool inGridYet = card.Update(otherOutGridAlreadyUpdated);
                if (!inGridYet) otherOutGridAlreadyUpdated = true;
            }
        }
    }

    private class CardPos {
        private bool inGridYet;
        public bool InGridYet { get => inGridYet; }
        private Coord prev, next, curr;
        public CardPos(Coord coord) {
            prev = coord.Copy();
            curr = coord.Copy();
            next = coord.Copy();
            inGridYet = false;
        }
        public Coord Next { set { next = value.Copy(); prev = curr.Copy(); } }
        public Coord Curr { get => curr; }
        public bool Lerp(float dist) {
            curr = Coord.Lerp(prev, next, dist);
            if (curr == next) { prev = curr.Copy(); inGridYet = true; }
            return inGridYet;
        }
    }

    private class UX_Card {
        private static readonly int MAX_TICK_COUNT = 50;
        private int idx, row, column;
        private CardPos cardPos;
        private Coord initPos;
        private RectTransform rectTran;
        private RawImage art;
        public Texture Art { get => art.texture; }
        private int tickCountdown = 0;
        public UX_Card(
            int idx, RectTransform baseCard, RectTransform parent, float width, float height, Coord initPos) {

            this.idx = idx;

            rectTran = Instantiate(baseCard.gameObject, parent).GetComponent<RectTransform>();
            rectTran.gameObject.name = "Card " + ((idx < 9) ? "0" : "") + (idx + 1);
            rectTran.sizeDelta = new Vector2(width, height);
            rectTran.gameObject.SetActive(false);

            art = rectTran.gameObject.GetComponent<RawImage>();

            cardPos = new CardPos(initPos);
            this.initPos = initPos;
            rectTran.anchoredPosition = Curr.ToVec2();
        }
        public Coord Next { set { cardPos.Next = value; tickCountdown = MAX_TICK_COUNT; } }
        public Coord Curr { get => cardPos.Curr; }
        public void Show() { rectTran.gameObject.SetActive(true); }
        public void Hide() {
            if (rectTran.gameObject.activeSelf) {
                cardPos = new CardPos(initPos);
                rectTran.gameObject.SetActive(false);
            }
        }
        public void SetArt(Card card) { art.texture = card.Art; }
        public bool Update(bool otherOutGridAlreadyUpdated) {
            // Another hand card that's NOT in the grid has moved.
            if (otherOutGridAlreadyUpdated) return false;

            // It's already in the grid and/or has not been assigned to move.
            if (tickCountdown <= 0) return true;

            tickCountdown--;
            int invTickCountdown = MAX_TICK_COUNT - tickCountdown;
            bool inGridYet = cardPos.Lerp((float) invTickCountdown / (float) MAX_TICK_COUNT);
            rectTran.anchoredPosition = Curr.ToVec2();

            return inGridYet;
        }

        // Puts the card in its final place, skipping the animation.
        public void Finish() {
            tickCountdown = 0;
            cardPos.Lerp(1);
            rectTran.anchoredPosition = Curr.ToVec2();
        }
    }
}
