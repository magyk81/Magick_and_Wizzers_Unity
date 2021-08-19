using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match
{
    private Player[] players;
    private Board[] boards = new Board[1];

    private readonly UX_Match UX_MATCH;
    public Match(UX_Match uxMatch, int playerCount)
    {
        UX_MATCH = uxMatch;
        players = new Player[playerCount];
        for (int i = 0; i < playerCount; i++) { players[i] = new Player(); }

        // Manually decide number of boards and their size.
        int boardSizeChunks = 2; // 2 here is magic for debugging.
        boards[0] = new Board(boardSizeChunks);
        int boardSize = boardSizeChunks * Board.CHUNK_SIZE;
        
        int[] boardSizes = new int[boards.Length];
        for (int i = 0; i < boards.Length; i++)
        {
            boardSizes[i] = boards[i].GetSize();
        }
        UX_MATCH.InitBoardObjs(boardSizes, playerCount);

        // Start each player with her initial master
        Coord[] masterStartPos = new Coord[playerCount];
        if (playerCount == 2) masterStartPos = new Coord[] {
            Coord._(boardSize / 4    , boardSize / 4),
            Coord._(boardSize / 4 * 3, boardSize / 4 * 3) };
        else if (playerCount == 3) masterStartPos = new Coord[] {
            Coord._(boardSize / 2    , boardSize / 6),
            Coord._(boardSize / 2    , boardSize / 2),
            Coord._(boardSize / 2    , boardSize / 6 * 5) };
        else if (playerCount == 4) masterStartPos = new Coord[] {
            Coord._(boardSize / 4    , boardSize / 4),
            Coord._(boardSize / 4 * 3, boardSize / 4),
            Coord._(boardSize / 4    , boardSize / 4 * 3),
            Coord._(boardSize / 4 * 3, boardSize / 4 * 3) };
        for (int i = 0; i < playerCount; i++)
        {
            Master initialMaster = new Master(i, 0, masterStartPos[i]);
            Debug.Log(masterStartPos[i].X);
            AddPiece(initialMaster);
        }
    }

    public void AddPiece(Piece piece, int boardIdx = 0)
    {
        boards[piece.BoardIdx].AddPiece(piece);
        UX_MATCH.AddPiece(piece);
    }

    public void PlayerInput(int playerIdx, int input)
    {
        players[playerIdx].Input(input);
    }
}
