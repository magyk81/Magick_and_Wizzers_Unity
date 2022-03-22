/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using UnityEngine;

public class CardTrap : Card {
    
    public CardTrap(string name, Texture art, int level, GameTerrain terrain)
        : base(name, art, level, terrain) { }

    public override UX_Player.Mode GetPlayMode() { return UX_Player.Mode.TARGET_TILE; }
}
