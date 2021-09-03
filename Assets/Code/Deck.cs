using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private Stack<Card> cards;

    public Deck(Card[] cards)
    {
        foreach (Card card in cards) { this.cards.Push(card); }
    }

    public Card DrawCard()
    {
        return cards.Pop();
    }
}
