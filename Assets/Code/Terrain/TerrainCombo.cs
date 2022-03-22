/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

public class TerrainCombo : GameTerrain {
    public enum Adjacency { TWIN, FLEXIBLE, RIGID }

    private readonly TerrainBase.Type[] mBaseTypes;
    private readonly Adjacency mAdjacency;

    public TerrainCombo(string name, TerrainBase.Type[] baseTypes, Adjacency adjacency) {
        mBaseTypes = new TerrainBase.Type[baseTypes.Length];
        System.Array.Copy(baseTypes, mBaseTypes, baseTypes.Length);
        mAdjacency = adjacency;
    }
}
