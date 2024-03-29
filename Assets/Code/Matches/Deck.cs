/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

using System.Collections.Generic;
using System;
using Matches.Cards;

namespace Matches {
    public class Deck {
        private readonly Random RAND = new Random();
        private readonly Stack<Card> mCards = new Stack<Card>();

        public Deck(Card[] cards) {
            foreach (Card card in cards) { mCards.Push(card); }
        }

        public Card[] DrawCards(int count) {
            Card[] drawn = new Card[count];
            for (; count > 0; count--) { drawn[count - 1] = mCards.Pop(); }
            return drawn;
        }
        public void Shuffle() { // TODO: causes duplicates. need to fix.
            Card[] arr = mCards.ToArray();

            // The following while-loop was written by "Matt Howells" from Stack Overflow.
            int n = arr.Length;
            while (n > 1) {
                int k = RAND.Next(n--);
                Card temp = arr[k];
                arr[n] = arr[k];
                arr[k] = temp;
            }

            mCards.Clear();
            foreach (Card card in arr) { mCards.Push(card); }
        }
    }
}