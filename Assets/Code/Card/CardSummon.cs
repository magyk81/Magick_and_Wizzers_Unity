/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSummon : Card
{
    int attackPoints, defensePoints;

    public CardSummon(string name, Texture art, int level,
        GameTerrain terrain, int attackPoints, int defensePoints)
        : base(name, art, level, terrain)
    {
        this.attackPoints = attackPoints;
        this.defensePoints = defensePoints;
    }

    public override UX_Player.Mode GetPlayMode()
    {
        return UX_Player.Mode.TARGET_TILE;
    }
}
