/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections;
using System.Collections.Generic;
using System;
public class Deck
{
    private Stack<Card> cards;
    private System.Random rand;

    public Deck(Card[] cards)
    {
        rand = new Random();
        this.cards = new Stack<Card>();
        foreach (Card card in cards) { this.cards.Push(card); }
    }

    public Card DrawCard()
    {
        return cards.Pop();
    }
    public void Shuffle()
    {
        Card[] arr = cards.ToArray();

        /* The following while-loop was written by "grenade"
         * (grenade.github.io) and edited by "Uwe Keim" (zeta-producer.com)
         * from Stack Overflow.
         */
        int n = arr.Length;
        while (n > 1)
        {
            n--;
            int k = rand.Next(n + 1);
            Card value = arr[k];
            arr[k] = arr[n];
            arr[n] = value;
        }

        cards = new Stack<Card>();
        foreach (Card card in arr) { cards.Push(card); }
    }
}
