using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match
{
    private Player[] players;
    private Board[] boards = new Board[1];

    private UX_Match uxMatch;
    public Match(UX_Match uxMatch, int playerCount)
    {
        players = new Player[playerCount];
        for (int i = 0; i < playerCount; i++) { players[i] = new Player(); }
        boards[0] = new Board(2);

        int[] boardSizes = new int[boards.Length];
        for (int i = 0; i < boards.Length; i++)
        {
            boardSizes[i] = boards[i].GetSize();
        }
        uxMatch.InitBoardObjs(boardSizes, playerCount);
    }

    public void PlayerInput(int playerIdx, int input)
    {
        players[playerIdx].Input(input);
    }
}
