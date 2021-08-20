using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainCombo : GameTerrain
{
    private TerrainBase.Type[] baseTypes;
    private bool isTwo;
    public TerrainCombo(string name, TerrainBase.Type[] baseTypes, bool isTwo)
    {
        System.Array.Copy(baseTypes, this.baseTypes, baseTypes.Length);
        this.isTwo = isTwo;
    }
}
