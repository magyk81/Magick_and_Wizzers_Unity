/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainBase : GameTerrain
{
    public enum Type { NORMAL, GROVE, LAKE, PEAK, MEADOW, FEN, WASTE, TOON }
    private readonly Type type;
    public TerrainBase(Type type) { this.type = type; }
}
