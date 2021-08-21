using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSummon : Card
{
    int attackPoints, defensPoints;

    public CardSummon(string name, UnityEngine.UI.RawImage art, int level,
        GameTerrain terrain, int attackPoints, int defensePoints)
        : base(name, art, level, terrain)
    {
        this.attackPoints = attackPoints;
        this.defensPoints = defensePoints;
    }
}
