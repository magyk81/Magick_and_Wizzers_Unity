using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal
{    
    protected byte[] message;
    protected readonly int[] intMessage;
    protected Signal(params int[] intMessage) { this.intMessage = intMessage; }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder("[");
        for (int i = 0; i < intMessage.Length; i++)
        {
            sb.Append(intMessage[i]);
            sb.Append(i == intMessage.Length - 1 ? "]" : ", ");
        }
        return sb.ToString();
    }

    public static implicit operator byte[](Signal s) => s.message;
}
