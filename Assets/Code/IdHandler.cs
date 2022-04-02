using System;
using System.Collections.Generic;

public static class IdHandler {
    private static readonly Dictionary<Type, ID> sIds = new Dictionary<Type, ID>();

    public static int Create(Type type) {
        // This condition ensures that all child classes of Piece use the same key.
        if (type.Equals(typeof(Piece)) || type.IsSubclassOf(typeof(Piece))) {
            if (!sIds.ContainsKey(typeof(Piece))) sIds.Add(typeof(Piece), new ID());
            return sIds[typeof(Piece)].Next;
        } else if (!sIds.ContainsKey(type)) sIds.Add(type, new ID());
        return sIds[type].Next;
    }

    public static void Reset() {
        foreach (KeyValuePair<Type, ID> item in sIds) { item.Value.Reset(); }
    }

    private class ID {
        private int next = 0;
        public int Next { get => next++; }
        public void Reset() { next = 0; }
    }
}
