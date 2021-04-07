using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandScript : MonoBehaviour
{
    [SerializeField]
    private RawImage tint;
    [SerializeField]
    private int drawMoveSpeed;
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

    // targetPos_disp will be set to equal (targetPos - dispDist)
    private Vector2[] targetPos, targetPos_disp;
    private float cardDispMult, cardDimRatio;

    private float dispDist = Screen.height;
    private readonly int DISP_DIST_PERC_MAX = 100;
    private int dispDistPerc;

    private void Start()
    {
        // It's not quite big enough without * 1.1F because z-pos is different.
        tint.GetComponent<RectTransform>().sizeDelta
            = new Vector2(Screen.width * 1.1F, Screen.height * 1.1F);
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
            Debug.Log("i: " + i);
        }

        targetPos = new Vector2[MAX_HAND_SIZE];
        targetPos_disp = new Vector2[MAX_HAND_SIZE];
        for (int i = 0; i < MAX_HAND_SIZE; i++)
        {
            targetPos[i] = new Vector2(0, 0);
            targetPos_disp[i] = new Vector2(0, 0);
        }

        BASE_CARD.SetActive(false);

        DrawCards(5);

        // Testing
        
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
                targetSpacing = (Screen.width / (alignments[i][1] + 2));
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
                        float yPlace = Screen.height / (i + 2);

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
                            Screen.width + targetSpacing, yPlace);

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
    
    private void Update()
    {
        // Increment/decrement dispPerc
        if (CameraScript.state == StateEnum.HAND)
        {
            if (dispDistPerc > 0) dispDistPerc--;
        }
        else
        {
            if (dispDistPerc < DISP_DIST_PERC_MAX) dispDistPerc++;
        }
        dispDist = Screen.height * ((float) dispDistPerc) / DISP_DIST_PERC_MAX;

        if (Input.GetKeyDown(KeyCode.H))
        {
            if (CameraScript.state == StateEnum.HAND)
                CameraScript.state = StateEnum.NORMAL;
            else CameraScript.state = StateEnum.HAND;
            tint.gameObject.SetActive(CameraScript.state == StateEnum.HAND);
        }

        // No need to update card display movement if no cards are in sight.
        if (CameraScript.state != StateEnum.HAND
            && dispDist == DISP_DIST_PERC_MAX) return;

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
                    // Multiplying Screen.width twice makes it seem like
                    // fullscreen is a similar speed as windowed.
                    float val = drawMoveSpeed * Screen.width * Screen.width;
                    if (Debug.isDebugBuild) val /= 3;
                    rt.anchoredPosition = new Vector2(
                        pos.x - (val / 100000F), targetPos_disp[i].y);
                    
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

        // Do not listen for key input if not in hand-view mode.
        if (CameraScript.state != StateEnum.HAND) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            DrawCards(3);
        }
        
        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveSelection(Coord.LEFT);
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveSelection(Coord.RIGHT);
        if (Input.GetKeyDown(KeyCode.UpArrow)) MoveSelection(Coord.UP);
        if (Input.GetKeyDown(KeyCode.DownArrow)) MoveSelection(Coord.DOWN);

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            cardSelect = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            cardSelect = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            cardSelect = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            cardSelect = 7;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            cardSelect = 8;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            cardSelect = 9;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            cardSelect = 10;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            cardSelect = 11;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            cardSelect = 8;
        }
    }
}
