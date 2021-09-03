using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTrap : Card
{
    private int attackPoints, defensePoints;
    
    public CardTrap(string name, Texture art, int level,
        GameTerrain terrain, int attackPoints, int defensePoints)
        : base(name, art, level, terrain)
    {
        this.attackPoints = attackPoints;
        this.defensePoints = defensePoints;
    }
}
