using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandScript : MonoBehaviour
{
    [SerializeField]
    private Image[] cursors;
    [SerializeField]
    private RawImage tint;
    [SerializeField]
    private int drawMoveSpeed, toggleMoveSpeed;

    private int[][] alignments = new int[4][] {
        new int[] { 7, 7 },
        new int[] { 20, 10 },
        new int[] { 36, 12 },
        new int[] { 60, 15 } };
    [SerializeField]
    private GameObject BASE_CARD;
    private GameObject[] objs_card;
    private GameObject obj_select;
    private RectTransform trans_select;
    private Texture texture_select;
    private readonly Stack<Card> DECK = new Stack<Card>();
    private readonly Stack<Card> HAND = new Stack<Card>();
    private readonly int MAX_HAND_SIZE = 60;
    private int width = Screen.width, height = Screen.height;
    private bool halfSizeWidgets = false;

    // targetPos_disp will be set to equal (targetPos - dispDist)
    private Vector2[] targetPos, targetPos_disp;
    private float cardDispMult, cardDimRatio;

    private float dispDist;
    private readonly int DISP_DIST_PERC_MAX = 100;
    private int dispDistPerc;

    private bool showing = false;
    public bool Showing
    {
        set
        {
            showing = value;
            tint.gameObject.SetActive(showing);
        }
    }

    private enum CursorType { NORMAL, DOT }

    private void Start()
    {
        dispDist = height;

        // It's not quite big enough without * 1.1F because z-pos is different.
        tint.GetComponent<RectTransform>().sizeDelta
            = new Vector2(width * 1.1F, height * 1.1F);
        tint.gameObject.SetActive(false);

        dispDistPerc = DISP_DIST_PERC_MAX;

        Transform trans = GetComponent<Transform>();

        Card[][] deckList = Card.GetTarot(false);
        for (int i = 0; i < deckList.Length; i++)
        {
            for (int j = 0; j < deckList[i].Length; j++)
            {
                DECK.Push(deckList[i][j]);
            }
        }

        Card deckPeek = DECK.Peek();
        texture_select = deckPeek.GetSelectTexture();
        cardDispMult = ((float) deckPeek.Width)
            / ((float) texture_select.width);
        cardDimRatio = ((float) deckPeek.Height) / ((float) deckPeek.Width);

        obj_select = Instantiate(BASE_CARD, trans);
        trans_select = obj_select.GetComponent<RectTransform>();
        Material material_select = new Material((Material) Resources.Load(
            "Materials/Card Material", typeof(Material)));
        material_select.mainTexture = texture_select;
        obj_select.GetComponent<Image>().material = material_select;
        obj_select.name = "Card Select";

        objs_card = new GameObject[MAX_HAND_SIZE];
        for (int i = 0; i < MAX_HAND_SIZE; i++)
        {
            objs_card[i] = Instantiate(BASE_CARD, trans);
            objs_card[i].name = "Card Object";
            objs_card[i].SetActive(false);
        }

        targetPos = new Vector2[MAX_HAND_SIZE];
        targetPos_disp = new Vector2[MAX_HAND_SIZE];
        for (int i = 0; i < MAX_HAND_SIZE; i++)
        {
            targetPos[i] = new Vector2(0, 0);
            targetPos_disp[i] = new Vector2(0, 0);
        }

        BASE_CARD.SetActive(false);

        UpdateCardPositions();
    }

    // This should only be called if there is multiplayer split-screen.
    public void SetDims(int width, int height, int orientation = 0)
    {
        this.width = width;
        this.height = height;

        dispDist = height;
        tint.GetComponent<RectTransform>().sizeDelta
            = new Vector2(width * 1.1F, height * 1.1F);
        
        if (orientation == 1) // wide
        {
            alignments = new int[3][] {
                new int[] { 10, 10 },
                new int[] { 30, 15 },
                new int[] { 60, 20 } };
        }
        else if (orientation == 2) // tall
        {
            alignments = new int[5][] {
                new int[] { 5, 5 },
                new int[] { 14, 7 },
                new int[] { 27, 9 },
                new int[] { 33, 11 },
                new int[] { 60, 12 } };
        }

        halfSizeWidgets = true;
    }

    public void DrawCards(int num)
    {
        for (int i = 0; i < num; i++)
        {
            if (HAND.Count >= MAX_HAND_SIZE || DECK.Count == 0) break;
            HAND.Push(DECK.Pop());
        }
        AlignCards();
    }

    private void AlignCards()
    {
        if (HAND.Count == 0) { cardSelect = -1; return; }
        else if (cardSelect == -1) cardSelect = 0;

        Card[] handArray = HAND.ToArray();
        // Reverse the array.
        Card[] handArrayR = new Card[handArray.Length];
        for (int i = 0; i < handArray.Length; i++)
        {
            handArrayR[handArray.Length - 1 - i] = handArray[i];
        }

        float targetSpacing = 0;
        for (int i = 0; i < alignments.Length; i++)
        {
            // See which alignment is appropriate based on hand size.
            if (HAND.Count <= alignments[i][0])
            {
                targetSpacing = (width / (alignments[i][1] + 2));
                float targetSpacingVert = targetSpacing * cardDimRatio;
                // targetWidth will be slightly smaller than targetSpacing.
                float targetWidth = targetSpacing * cardDispMult;

                for (int j = 0; j < MAX_HAND_SIZE; j++)
                {
                    // Iterate over cards that are in the hand.
                    if (j < HAND.Count)
                    {
                        // If card was already in the hand (not being drawn).
                        bool alreadyInHand = objs_card[j].activeSelf;

                        objs_card[j].SetActive(true);
                        handArrayR[j].SetToObj(objs_card[j], targetWidth);

                        float xPlace = targetSpacing;
                        float yPlace = (height / 2) - (i * targetSpacingVert / 2);

                        for (int k = i; k >= 0; k--)
                        {
                            // See what row the card goes in.
                            int val = alignments[i][1] * k;
                            if (j >= val)
                            {
                                // Move card right a number of times, set back
                                // by row number.
                                xPlace *= ((float) j - val) + 1.5F;
                                // Move card up a number of times depending on
                                // row number.
                                yPlace += targetSpacingVert * (i - k);
                                // (i - k) instead of (k) so that the cards
                                // come in from the bottom instead of the top.

                                break;
                            }
                        }

                        targetPos[j] = new Vector2(xPlace, yPlace);
                    
                        Vector2 pos;
                        // Cards already in hand stay where they are.
                        if (alreadyInHand) pos = new Vector2(
                            xPlace, yPlace);
                        // Cards that were just added go off screen to the right.
                        else pos = new Vector2(
                            width + targetSpacing, yPlace);

                        objs_card[j].GetComponent<RectTransform>()
                            .anchoredPosition = pos;
                    }
                    else
                    {
                        // Cards not in hand
                        objs_card[j].SetActive(false);
                    }
                }

                break;
            }
        }

        // targetSpacing will be zero if there were no cards in hand.
        if (targetSpacing != 0)
        {
            // Resize texture_select based on the cards' dimensions.
            float mult = targetSpacing / texture_select.width;
            trans_select.sizeDelta = new Vector2(
                texture_select.width * mult,
                texture_select.height * mult);
        }
        
    }

    private int cardSelect = -1;
    private void MoveSelection(int dir)
    {
        for (int i = 0; i < alignments.Length; i++)
        {
            // See which alignment is appropriate based on hand size.
            if (HAND.Count <= alignments[i][0])
            {
                if (HAND.Count > 0)
                {
                    int rowSize = alignments[i][1];

                    if (i > 0)
                    {
                        if (dir == Coord.UP)
                        {
                            if (cardSelect >= rowSize)
                                cardSelect -= rowSize;
                            else
                            {
                                while (cardSelect + rowSize
                                    < HAND.Count)
                                {
                                    cardSelect += rowSize;
                                }
                            }
                        }
                        else if (dir == Coord.DOWN)
                        {
                            if (cardSelect + rowSize
                                < HAND.Count)
                            {
                                cardSelect += rowSize;
                            }
                            else
                            {
                                while (cardSelect - rowSize >= 0)
                                {
                                    cardSelect -= rowSize;
                                }
                            }
                        }
                    }

                    int row = 0, temp = cardSelect;
                    while (temp > rowSize)
                    {
                        temp -= rowSize;
                        row++;
                    }

                    int column = cardSelect % rowSize;

                    if (dir == Coord.LEFT)
                    {
                        if (column == 0)
                        {
                            cardSelect += rowSize - 1;
                            if (cardSelect >= HAND.Count)
                                cardSelect = HAND.Count - 1;
                        }
                        else cardSelect--;
                    }
                    else if (dir == Coord.RIGHT)
                    {
                        if (cardSelect == HAND.Count - 1)
                            cardSelect = rowSize * row;
                        else if (column == rowSize - 1)
                            cardSelect -= rowSize - 1;
                        else cardSelect++;
                    }
                }
                else
                {
                    cardSelect = -1;
                    return;
                }

                break;
            }
        }
    }

    private void UpdateCardPositions()
    {
        for (int i = 0; i < MAX_HAND_SIZE; i++)
        {
            if (objs_card[i].activeSelf)
            {
                // Set targetPos_disp to equal (targetPos - dispDist)
                targetPos_disp[i] = new Vector2(
                    targetPos[i].x, targetPos[i].y - dispDist);

                RectTransform rt = objs_card[i].GetComponent<RectTransform>();
                Vector2 pos = rt.anchoredPosition;

                if (pos.x > targetPos[i].x)
                {
                    float val = drawMoveSpeed * width;
                    rt.anchoredPosition = new Vector2(
                        pos.x - (val / 1000F), targetPos_disp[i].y);
                    
                    // Don't let card position go past the mark.
                    if (rt.anchoredPosition.x <= targetPos[i].x)
                        rt.anchoredPosition = targetPos_disp[i];

                    break;
                    // Breaking here means that only the first "out of place"
                    // card travels. This way, only 1 card travels at a time.
                }
                else rt.anchoredPosition = targetPos_disp[i];
            }
        }

        if (cardSelect == -1) obj_select.SetActive(false);
        else
        {
            obj_select.SetActive(true);
            trans_select.anchoredPosition = objs_card[cardSelect]
                .GetComponent<RectTransform>().anchoredPosition;
        }
    }

    public void PressDirection(int dir, bool press)
    {
        if (press) MoveSelection(dir);
    }

    // Called from Player once in the constructor before Update is called.
    public void SetupCursors(int cursorRadius)
    {
        if (!halfSizeWidgets) cursorRadius *= 2;
        cursors[(int) CursorType.NORMAL].GetComponent<RectTransform>().sizeDelta
            = new Vector2(cursorRadius, cursorRadius);
        cursors[(int) CursorType.DOT].GetComponent<RectTransform>().sizeDelta
            = new Vector2(cursorRadius / 20F, cursorRadius / 20F);
    }
    
    // Update which cursor(s) is being displayed.
    // Called from Player everytime state is changed.
    public void SetCursor(StateEnum state)
    {
        bool useNormalCursor = (state == StateEnum.PIECE
            || state == StateEnum.DESTINATION);
        bool useDotCursor = (state == StateEnum.TILE
            || state == StateEnum.DESTINATION);
        cursors[(int) CursorType.NORMAL].gameObject.SetActive(useNormalCursor);
        cursors[(int) CursorType.DOT].gameObject.SetActive(useDotCursor);
    }

    private void Update()
    {
        if (showing)
        {
            // Increment/decrement dispPerc
            if (dispDistPerc > 0) dispDistPerc -= toggleMoveSpeed;
            foreach (Image cursor in cursors)
                cursor.gameObject.SetActive(false);
            
            UpdateCardPositions();
        }
        else
        {
            if (dispDistPerc < DISP_DIST_PERC_MAX)
                dispDistPerc += toggleMoveSpeed;

            // Only update card display movement if cards are in sight.
            if (dispDistPerc != DISP_DIST_PERC_MAX) UpdateCardPositions();
        }
        dispDist = height * ((float) dispDistPerc) / DISP_DIST_PERC_MAX;
    }
}
