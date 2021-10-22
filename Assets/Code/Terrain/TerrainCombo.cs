/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCombo : GameTerrain
{
    private TerrainBase.Type[] baseTypes;
    public enum Adjacency { TWIN, FLEXIBLE, RIGID }
    private Adjacency adjacency;
    public TerrainCombo(string name, TerrainBase.Type[] baseTypes,
        Adjacency adjacency)
    {
        this.baseTypes = new TerrainBase.Type[baseTypes.Length];
        System.Array.Copy(baseTypes, this.baseTypes, baseTypes.Length);
        this.adjacency = adjacency;
    }
}
