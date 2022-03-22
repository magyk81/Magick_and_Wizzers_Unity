using System.Text;

public class Signal {
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

    public static implicit operator byte[](Signal s) => s.mByteMessage;
}
