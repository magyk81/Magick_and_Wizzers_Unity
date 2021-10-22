/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinTicket
{
    public enum Type { ADD_PIECE, REMOVE_PIECE, MOVE_PIECE,
        ADD_WAYPOINT, REMOVE_WAYPOINT }
    private Type type;
    public Type TicketType { get { return type; } }
    private Piece piece;
    public Piece Piece { get { return piece; } }
    private Card card;
    public Card Card { get { return card; } }
    private Coord coord;
    public Coord Coord { get { return coord; } }

    public SkinTicket(Piece piece, Type type)
    {
        this.piece = piece;
        this.type = type;
    }
    public SkinTicket(Piece piece, Coord coord, Type type)
    {
        this.piece = piece;
        this.coord = coord;
        this.type = type;
    }
    public SkinTicket(Piece piece, Card card, Coord coord, Type type)
    {
        this.piece = piece;
        this.card = card;
        this.coord = coord;
        this.type = type;
    }
}