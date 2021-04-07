using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card
{
    private static string[][] tarot_keywords = {
        new string[] { "Wands", "Pentacles", "Cups", "Swords" },
        new string[] { "Ace", "Two", "Three", "Four", "Five",
            "Six", "Seven", "Eight", "Nine", "Ten",
            "Page", "Knight", "Queen", "King" },
        new string[] { "The Fool", "The Magician", "The High Priestess",
            "The Empress", "The Emperor", "The Hierophant", "The Lovers",
            "The Chariot", "Strength", "The Hermit", "Wheel of Fortune",
            "Justice", "The Hanged Man", "Death", "Temperance", "The Devil",
            "The Tower", "The Star", "The Moon", "The Sun", "Judgement",
            "The World" }
    };

    public static Card[][] GetTarot(bool allArcana)
    {
        if (tarot != null)
        {
            if (tarot.Length == 5 && allArcana
                || tarot.Length == 4 && !allArcana) return tarot;
        }

        tarot = new Card[allArcana ? 5 : 4][];
        int majorCount = tarot_keywords[2].Length;
        int suitCount = tarot_keywords[0].Length;
        int rankCount;
        if (allArcana) rankCount = tarot_keywords[1].Length;
        else rankCount = tarot_keywords[1].Length - 1;

        if (allArcana)
        {
            tarot[4] = new Card[majorCount];
            for (int a = 0; a < majorCount; a++)
            {
                tarot[4][a] = new Card(tarot_keywords[2][a], "TarotCards");
            }
        }
        for (int i = 0; i < suitCount; i++)
        {
            tarot[i] = new Card[rankCount];
            string suit = tarot_keywords[0][i];
            for (int j = 0; j < rankCount; j++)
            {
                string rank;
                // Skip the knight if not using all arcana
                if (!allArcana && j >= 11) rank = tarot_keywords[1][j + 1];
                else rank = tarot_keywords[1][j];
                tarot[i][j] = new Card(rank + " of " + suit,
                    allArcana ? "TarotCards" : "PlayingCards");
            }
        }

        return tarot;
    }

    private static Card[][] tarot;

    private string name;
    private Texture texture;
    private int width, height;
    public int Width { get { return width; } }
    public int Height { get { return height; } }
    public Card(string name, string folder = "Cards")
    {
        this.name = name;

        texture = (Texture) Resources.Load(
            folder + "/" + name, typeof(Texture));
        
        if (texture == null) Debug.Log(name);
        
        width = texture.width;
        height = texture.height;
    }

    private Material material;
    private float targetWidth = 0;
    public void SetToObj(GameObject obj_rect, float targetWidth)
    {
        if (targetWidth != this.targetWidth)
        {
            float mult = targetWidth / texture.width;
            obj_rect.GetComponent<RectTransform>().sizeDelta
                = new Vector2(texture.width * mult, texture.height * mult);
        }
        
        if (material == null)
            material = new Material((Material) Resources.Load(
                "Materials/Card Material", typeof(Material)));

        material.mainTexture = texture;
        obj_rect.GetComponent<Image>().material = material;
        obj_rect.name = "Card Object: " + name;
    }

    public Texture GetSelectTexture()
    {
        return (Texture) Resources.Load(
            "Textures/Card Select " + width + " x " + height,
            typeof(Texture));
    }
}
