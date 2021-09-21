using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match
{
    private static class InitInfo
    {
        public static Player[] players;
    }
    public static Player[] Players {
        set { InitInfo.players = value; } }

    private Board[] boards = new Board[2];
    public Match()
    {
        boards[0] = new Board("Main", InitInfo.players);
        boards[1] = new Board("Sheol", 1);
    }
    public void InitUX(UX_Match uxMatch) { uxMatch.Init(InitInfo.players, boards); }
    public void MainLoop() {}
}