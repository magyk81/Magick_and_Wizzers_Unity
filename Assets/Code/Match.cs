using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match
{
    private Player[] players;
    private Board[] boards = new Board[1];
    private readonly UX_Match UX_MATCH;
    public Match(UX_Match uxMatch, Player[] players)
    {
        UX_MATCH = uxMatch;
        this.players = players;

        // Manually decide number of boards and their size.
        int boardSizeChunks = 2; // 2 here is magic for debugging.
        boards[0] = new Board(boardSizeChunks);
        int boardSize = boardSizeChunks * Board.CHUNK_SIZE;
        
        int[] boardSizes = new int[boards.Length];
        for (int i = 0; i < boards.Length; i++)
        {
            boardSizes[i] = boards[i].GetSize();
        }
        UX_MATCH.InitBoardObjs(boardSizes, players);

        // Start each player with her initial master
        Coord[] masterStartPos = new Coord[players.Length];
        
        if (players.Length == 2) masterStartPos = new Coord[] {
            Coord._(boardSize / 4    , boardSize / 4),
            Coord._(boardSize / 4 * 3, boardSize / 4 * 3) };
        else if (players.Length == 3) masterStartPos = new Coord[] {
            Coord._(boardSize / 2    , boardSize / 6),
            Coord._(boardSize / 2    , boardSize / 2),
            Coord._(boardSize / 2    , boardSize / 6 * 5) };
        else if (players.Length == 4) masterStartPos = new Coord[] {
            Coord._(boardSize / 4    , boardSize / 4),
            Coord._(boardSize / 4 * 3, boardSize / 4),
            Coord._(boardSize / 4    , boardSize / 4 * 3),
            Coord._(boardSize / 4 * 3, boardSize / 4 * 3) };
        for (int i = 0; i < players.Length; i++)
        {
            Master initialMaster = new Master(
                players[i], i, 0, masterStartPos[i]);
            AddPiece(initialMaster);
        }
    }

    public void AddPiece(Piece piece, int boardIdx = 0)
    {
        boards[piece.BoardIdx].AddPiece(piece);
        UX_MATCH.AddPiece(piece);
    }

    // Update is called once per frame
    public void Update()
    {
        UX_MATCH.Update();
    }
}