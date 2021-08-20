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
        players[0] = new Player(Player.Type.LOCAL_PLAYER);
        players[1] = new Player(Player.Type.BOT);

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

    // Terrains
    public static readonly TerrainBase NORMAL = new TerrainBase(TerrainBase.Type.NORMAL);
    public static readonly TerrainBase GROVE = new TerrainBase(TerrainBase.Type.GROVE);
    public static readonly TerrainBase LAKE = new TerrainBase(TerrainBase.Type.LAKE);
    public static readonly TerrainBase PEAK = new TerrainBase(TerrainBase.Type.PEAK);
    public static readonly TerrainBase MEADOW = new TerrainBase(TerrainBase.Type.MEADOW);
    public static readonly TerrainBase FEN = new TerrainBase(TerrainBase.Type.FEN);
    public static readonly TerrainBase WASTE = new TerrainBase(TerrainBase.Type.WASTE);
    public static readonly TerrainBase TOON = new TerrainBase(TerrainBase.Type.TOON);
    public static readonly TerrainCombo WOOD = new TerrainCombo("Wood", new TerrainBase.Type[]{TerrainBase.Type.GROVE}, true);
    public static readonly TerrainCombo FOREST = new TerrainCombo("Forest", new TerrainBase.Type[]{TerrainBase.Type.GROVE}, false);
}
