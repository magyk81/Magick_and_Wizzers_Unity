using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card
{
    private Texture art;
    public Texture Art { get { return art; } }

    protected Card(string name, Texture art, int level,
        GameTerrain terrain)
    {
        this.art = art;
    }

    public static Card[] cards = {
        new CardSummon("Incubus Knight", null, 5, GameTerrain.FEN, 1650, 1300),
        new CardSummon("Armored Sea Star", null, 4,
            GameTerrain.LAKE, 850, 1400),
        new CardSummon("Lifeblood Drinker", null, 3,
            GameTerrain.FEN, 900, 800),
        new CardSummon("Darkland Thorns", null, 3, GameTerrain.FEN, 1200, 900),
        new CardSummon("Weak Dragon", null, 4, GameTerrain.PEAK, 1200, 1000),
        new CardSummon("Huge Totem of Iron", null, 5, null, 1400, 1800),
        new CardSummon("Jordan", null, 5, GameTerrain.LAKE, 1400, 1600),
        new CardSummon("Wing Scythe", null, 2, GameTerrain.LAKE, 450, 500),
        new CardSummon("Gravel Rock", null, 5, GameTerrain.PEAK, 1300, 1600),
        new CardSummon("Man-Eating Insect", null, 2,
            GameTerrain.FEN, 450, 600),
        new CardSummon("Bloom Coyote", null, 5, GameTerrain.GROVE, 1800, 1400),
        new CardTrap("Sad Rogue", null, 1, GameTerrain.LAKE, 300, 300),
        new CardSummon("Sky Predator", null, 4, GameTerrain.LAKE, 1550, 1200),
        new CardTrap("Mine Beast", null, 4, GameTerrain.PEAK, 1200, 1300),
        new CardSummon("Prickly Eel", null, 5, GameTerrain.LAKE, 1600, 1300),
        new CardSummon("Steel Drake", null, 6, null, 1850, 1700),
        new CardSummon("Cyclopse Buffer Wyvern", null, 3,
            GameTerrain.LAKE, 700, 1300),
        new CardSummon("Devil-Wielding Wyvern Maiden", null, 3,
            GameTerrain.ÆTHER, 1200, 900),
        new CardSummon("Fire Demon", null, 4, GameTerrain.PEAK, 1300, 1000),
        new CardSummon("Mermaid Chant", null, 3, GameTerrain.LAKE, 1200, 900),
        new CardSummon("Evil Napoleon", null, 2, GameTerrain.FEN, 800, 400),
        new CardSummon("Human-like Spider", null, 3,
            GameTerrain.GROVE, 700, 1400),
        new CardSummon("Lethal Panda", null, 4, GameTerrain.GROVE, 1200, 1000),
        new CardSummon("Tommy the Djinn", null, 4,
            GameTerrain.FEN, 1200, 1300),
        new CardSummon("Weaplate", null, 3, GameTerrain.MEADOW, 700, 1300),
        new CardSummon("Spirit of the Harpsichord", null, 4,
            GameTerrain.MEADOW, 800, 2000),
        new CardSummon("Magnet Knight", null, 3,
            GameTerrain.MEADOW, 500, 1000),
        new CardSummon("Electrum Knight", null, 3,
            GameTerrain.MEADOW, 1000, 500),
        new CardSummon("Man Consumer", null, 2, GameTerrain.GROVE, 800, 600),
        new CardSummon("Tough Armor", null, 3, null, 300, 1200),
        new CardSummon("Lingus", null, 3, GameTerrain.JUNGLE, 800, 1000),
        new CardSummon("Doom Djinn", null, 4, GameTerrain.FEN, 1400, 1300),
        new CardTrap("Card Reaper", null, 5,
            GameTerrain.GRAVEYARD, 1380, 1930),
        new CardSummon("Baleful Night Dragon", null, 7,
            GameTerrain.VOLCANO, 2400, 2000),
        new CardSummon("Feral Predator", null, 4, GameTerrain.PEAK, 1500, 800),
        new CardSummon("Huge Rock Soldier", null, 3, null, 1300, 2000),
        new CardSummon("Axygenala Knight", null, 4,
            GameTerrain.MEADOW, 1500, 1200),
        new CardSummon("Hex of Wyvern", null, 5, GameTerrain.PEAK, 2000, 1500),
        new CardSummon("Alabam", null, 4, GameTerrain.GROVE, 1200, 1500),
        new CardSummon("Cannon Horn", null, 4, GameTerrain.PEAK, 1200, 1400)
    };
}
