using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainBase : GameTerrain
{
    public enum Type { NORMAL, GROVE, LAKE, PEAK, MEADOW, FEN, WASTE, TOON }
    private readonly Type type;
    public TerrainBase(Type type) { this.type = type; }
}
