/* Copyright (C) All Rights Reserved
 * Unauthorized copying of this file, via any medium is prohibited.
 * Proprietary and confidential.
 * Written by Robin Campos <magyk81@gmail.com>, year 2021.
 */

namespace Matches {
    public class Player {
        public readonly int ID;

        public readonly string Name;
        public readonly int ClientID, LocalID;
        public readonly bool IsBot;
        public Player(string name, int clientID, bool isBot) {
            ID = IdHandler.Create(GetType());

            Name = name;
            ClientID = clientID;
            IsBot = isBot;
        }
    }
}