using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalFromClient : Signal
{

    private SignalFromClient(params int[] intMessage) : base(intMessage) {}

    public int PlayerID
    {
        set
        {
            if (message == null)
            {
                int messageLength = 2 + (intMessage.Length * sizeof(int));
                message = new byte[messageLength];

                // First byte is message length, second byte is playerID.
                message[0] = (byte) (messageLength - 1);
                message[1] = (byte) value;

                for (int i = 2, j = 0; i < messageLength;
                    i += sizeof(int), j++)
                {
                    byte[] intAsBytes = BitConverter.GetBytes(intMessage[j]);
                    for (int k = 0; k < sizeof(int); k++)
                    {
                        message[i + k] = intAsBytes[k];
                    }
                }
            }
            else Debug.Log(
                "Warning: Should not assign PlayerID more than once.");
        }
    }

    public enum Request { CAST_SPELL }

    public static SignalFromClient CastSpell(Card card, Piece caster,
        Coord tile)
    {
        return new SignalFromClient(
            (int) Request.CAST_SPELL,   // Request enum
            card.ID,                    // Card ID
            caster.ID,                  // Piece ID that's casting the spell
            tile.X, tile.Z              // Tile where it's going onto
        );
    }
}
