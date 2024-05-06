using System;
using System.Collections.Generic;
using UnityEngine;

public class MessageChecker
{
    public MessageType CheckMessageType(byte[] message)
    {
        int messageType = BitConverter.ToInt32(message, 0);
        return (MessageType)messageType;
    }

    public static byte[] SerializeString(char[] charArray)
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes(charArray.Length));

        for (int i = 0; i < charArray.Length; i++)
        {
            outData.AddRange(BitConverter.GetBytes(charArray[i]));
        }

        return outData.ToArray();
    }

    public static string DeserializeString(byte[] message, int indexToInit)
    {
        int stringSize = BitConverter.ToInt32(message, indexToInit);

        char[] charArray = new char[stringSize];

        indexToInit += sizeof(int);
        for (int i = 0; i < stringSize; i++)
        {
            charArray[i] = BitConverter.ToChar(message, indexToInit + sizeof(char) * i);
        }

        return new string(charArray);
    }

    public static bool DeserializeCheckSum(byte[] message)
    {
        int messageSum = BitConverter.ToInt32(message, message.Length - sizeof(int));
        messageSum >>= 5;

        if (messageSum != message.Length)
        {
            Debug.LogError("Message corrupted.");
            return false;
        }

        return true;
    }

    public static byte[] SerializeCheckSum(List<byte> data)
    {
        int sum = data.Count + sizeof(int);

        sum <<= 5;

        return BitConverter.GetBytes(sum);
    }
}