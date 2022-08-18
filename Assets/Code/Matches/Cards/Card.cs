/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using UnityEngine;
using Matches;
using Matches.Terrains;

namespace Matches.Cards {
    public abstract class Card {
        public readonly int ID;

        private readonly string mName;
        private readonly Texture mArt;
        private readonly int mLevel;

        protected Card(string name, Texture art, int level, GameTerrain terrain) {
            ID = IdHandler.Create(GetType());

            mName = name;
            mArt = art;
            mLevel = level;

            // Load debugging art
            mArt = Resources.Load<Texture>("Textures/Debug_Card_Art/" + Util.ReplaceSpaces(name));
        }

        public string Name { get => mName; }
        public Texture Art { get => mArt; }
        public int Level { get => mLevel; }
        public int PlayMode { get => 1; }

        public static Card[] friend_cards = {
            new CardSummon("Aidan Hendrickson", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Alan Gatica", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Allen Wing", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Amanda McIntire", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Anna Maurice", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Armin Boroujerdi", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Atle Olson", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Ben Matthews", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Brandon Garrett", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Bryan Tenorio", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Caitlynn Pokagon", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Charles West", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Commander Cody", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Connor McHarney", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Dakota Coleman", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Dara Castellanos", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Divya Komaravolu", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Drew Brost", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Dustin Loughrin", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Erick Medina", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Germaine Sy", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Jared Tarbell", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Joel Castellanos", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Joel Rojo", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("John-Mark Collins", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Jozeph Gulley", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Kevin Gao", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Kyle Leisker", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Lielen Castellanos", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Michael Burbank", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Mike Rivas", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Miles Benavides", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Nathan Gonzales", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Qi Lu", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Rachel Golden", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Robin Campos", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Rowan Castellanos", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Sam Roberts", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Steve Plass", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM),
            new CardSummon("Theresa Kitt", null, 1, null, 100, 100, Piece.Speed.NORMAL, Piece.Size.MEDIUM)
        };

        // public static Card[] lobewd_cards = {
        //     new CardSummon("Incubus Knight", null, 5, GameTerrain.FEN, 1650, 1300),
        //     new CardSummon("Armored Sea Star", null, 4,
        //         GameTerrain.LAKE, 850, 1400),
        //     new CardSummon("Lifeblood Drinker", null, 3,
        //         GameTerrain.FEN, 900, 800),
        //     new CardSummon("Darkland Thorns", null, 3, GameTerrain.FEN, 1200, 900),
        //     new CardSummon("Weak Dragon", null, 4, GameTerrain.PEAK, 1200, 1000),
        //     new CardSummon("Huge Totem of Iron", null, 5, null, 1400, 1800),
        //     new CardSummon("Jordan", null, 5, GameTerrain.LAKE, 1400, 1600),
        //     new CardSummon("Wing Scythe", null, 2, GameTerrain.LAKE, 450, 500),
        //     new CardSummon("Gravel Rock", null, 5, GameTerrain.PEAK, 1300, 1600),
        //     new CardSummon("Man-Eating Insect", null, 2,
        //         GameTerrain.FEN, 450, 600),
        //     new CardSummon("Bloom Coyote", null, 5, GameTerrain.GROVE, 1800, 1400),
        //     new CardTrap("Sad Rogue", null, 1, GameTerrain.LAKE, 300, 300),
        //     new CardSummon("Sky Predator", null, 4, GameTerrain.LAKE, 1550, 1200),
        //     new CardTrap("Mine Beast", null, 4, GameTerrain.PEAK, 1200, 1300),
        //     new CardSummon("Prickly Eel", null, 5, GameTerrain.LAKE, 1600, 1300),
        //     new CardSummon("Steel Drake", null, 6, null, 1850, 1700),
        //     new CardSummon("Cyclopse Buffer Wyvern", null, 3,
        //         GameTerrain.LAKE, 700, 1300),
        //     new CardSummon("Devil-Wielding Wyvern Maiden", null, 3,
        //         GameTerrain.ÆTHER, 1200, 900),
        //     new CardSummon("Fire Demon", null, 4, GameTerrain.PEAK, 1300, 1000),
        //     new CardSummon("Mermaid Chant", null, 3, GameTerrain.LAKE, 1200, 900),
        //     new CardSummon("Evil Napoleon", null, 2, GameTerrain.FEN, 800, 400),
        //     new CardSummon("Human-like Spider", null, 3,
        //         GameTerrain.GROVE, 700, 1400),
        //     new CardSummon("Lethal Panda", null, 4, GameTerrain.GROVE, 1200, 1000),
        //     new CardSummon("Tommy the Djinn", null, 4,
        //         GameTerrain.FEN, 1200, 1300),
        //     new CardSummon("Weaplate", null, 3, GameTerrain.MEADOW, 700, 1300),
        //     new CardSummon("Spirit of the Harpsichord", null, 4,
        //         GameTerrain.MEADOW, 800, 2000),
        //     new CardSummon("Magnet Knight", null, 3,
        //         GameTerrain.MEADOW, 500, 1000),
        //     new CardSummon("Electrum Knight", null, 3,
        //         GameTerrain.MEADOW, 1000, 500),
        //     new CardSummon("Man Consumer", null, 2, GameTerrain.GROVE, 800, 600),
        //     new CardSummon("Tough Armor", null, 3, null, 300, 1200),
        //     new CardSummon("Lingus", null, 3, GameTerrain.JUNGLE, 800, 1000),
        //     new CardSummon("Doom Djinn", null, 4, GameTerrain.FEN, 1400, 1300),
        //     new CardTrap("Card Reaper", null, 5,
        //         GameTerrain.GRAVEYARD, 1380, 1930),
        //     new CardSummon("Baleful Night Dragon", null, 7,
        //         GameTerrain.VOLCANO, 2400, 2000),
        //     new CardSummon("Feral Predator", null, 4, GameTerrain.PEAK, 1500, 800),
        //     new CardSummon("Huge Rock Soldier", null, 3, null, 1300, 2000),
        //     new CardSummon("Axygenala Knight", null, 4,
        //         GameTerrain.MEADOW, 1500, 1200),
        //     new CardSummon("Hex of Wyvern", null, 5, GameTerrain.PEAK, 2000, 1500),
        //     new CardSummon("Alabam", null, 4, GameTerrain.GROVE, 1200, 1500),
        //     new CardSummon("Cannon Horn", null, 4, GameTerrain.PEAK, 1200, 1400)
        // };
    }
}