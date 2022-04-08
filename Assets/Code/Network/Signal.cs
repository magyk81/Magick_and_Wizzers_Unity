using System;
using System.Text;

public abstract class Signal {
    protected byte[] mByteMessage;
    protected readonly int[] mIntMessage;

    public override string ToString() {
        StringBuilder strBuilder = new StringBuilder("[");
        for (int i = 0; i < mIntMessage.Length; i++) {
            strBuilder.Append(mIntMessage[i]);
            strBuilder.Append(i == mIntMessage.Length - 1 ? "]" : ", ");
        }
        return strBuilder.ToString();
    }

    protected Signal(params int[] intMessage) { mIntMessage = intMessage; }

    /// <summary>
    /// Generates the byte message if the integer message has been set but the byte message has not been set.
    /// This is used in <c>SocketHand#SendMessages</c> for a call to <c>Socket#Send</c>.
    /// </summary>
    /// <returns>
    /// The byte message.
    /// </returns>
    public static implicit operator byte[](Signal signal) {
        if (signal.mByteMessage == null) {
            if (signal.mIntMessage.Length == 0) return null;

            // Set the message (byte array).
            int messageLength = 1 + (signal.mIntMessage.Length * sizeof(int));
            signal.mByteMessage = new byte[messageLength];

            // First byte is message length.
            signal.mByteMessage[0] = (byte) (messageLength - 1);

            for (int i = 1, j = 0; i < messageLength; i += sizeof(int), j++) {
                byte[] intAsBytes = BitConverter.GetBytes(signal.mIntMessage[j]);
                for (int k = 0; k < sizeof(int); k++) { signal.mByteMessage[i + k] = intAsBytes[k]; }
            }
        }
        return signal.mByteMessage;
    }
}
