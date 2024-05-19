using System;
using System.Collections.Generic;
using UnityEngine;

public class MessageChecker
{
    /// <summary>
    /// Checks the priority of a message from the given byte array.
    /// </summary>
    /// <param name="message">The byte array containing the message.</param>
    /// <returns>The message priority as a MessagePriority enum value.</returns>
    public static MessagePriority CheckMessagePriority(byte[] message)
    {
        int messagePriority = BitConverter.ToInt32(message, 4);

        return (MessagePriority)messagePriority;
    }

    /// <summary>
    /// Checks the type of a message from the given byte array.
    /// </summary>
    /// <param name="message">The byte array containing the message.</param>
    /// <returns>The message type as a MessageType enum value.</returns>
    public static MessageType CheckMessageType(byte[] message)
    {
        int messageType = BitConverter.ToInt32(message, 0);

        return (MessageType)messageType;
    }

    /// <summary>
    /// Serializes a character array into a byte array.
    /// </summary>
    /// <param name="charArray">The character array to serialize.</param>
    /// <returns>A byte array representing the serialized character array.</returns>
    public static byte[] SerializeString(char[] charArray)
    {
        List<byte> outData = new ();

        // Add the length of the character array as the first bytes
        outData.AddRange(BitConverter.GetBytes(charArray.Length));

        // Add each character as a set of bytes
        for (int i = 0; i < charArray.Length; i++)
        {
            outData.AddRange(BitConverter.GetBytes(charArray[i]));
        }

        return outData.ToArray();
    }

    /// <summary>
    /// Deserializes a string from a byte array starting at a specified index.
    /// </summary>
    /// <param name="message">The byte array containing the serialized string.</param>
    /// <param name="indexToInit">The starting index for deserialization.</param>
    /// <returns>The deserialized string.</returns>
    public static string DeserializeString(byte[] message, int indexToInit)
    {
        int stringSize = BitConverter.ToInt32(message, indexToInit);

        char[] charArray = new char[stringSize];

        indexToInit += sizeof(int);

        // Extract each character from the byte array
        for (int i = 0; i < stringSize; i++)
        {
            charArray[i] = BitConverter.ToChar(message, indexToInit + sizeof(char) * i);
        }

        return new string(charArray);
    }

    /// <summary>
    /// Checks if a message's checksum is valid.
    /// </summary>
    /// <param name="message">The byte array containing the message.</param>
    /// <returns>True if the checksum is valid, otherwise false.</returns>
    public static bool DeserializeCheckSum(byte[] message)
    {
        uint messageSum = (uint)BitConverter.ToInt32(message, message.Length - sizeof(int));

        messageSum >>= 5;
        //DeserializeSum(ref messageSum);

        if (messageSum != message.Length)
        {
            Debug.LogError("Message corrupted.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Serializes a checksum for a given list of bytes.
    /// </summary>
    /// <param name="data">The list of bytes to create a checksum for.</param>
    /// <returns>A byte array representing the serialized checksum.</returns>
    public static byte[] SerializeCheckSum(List<byte> data)
    {
        uint sum = (uint)(data.Count + sizeof(int));

        sum <<= 5;
        //SerializeSum(ref sum);

        return BitConverter.GetBytes(sum);
    }

    /// <summary>
    /// Deserializes the checksum value using a custom encryption algorithm.
    /// </summary>
    /// <param name="sum">The checksum value to be decrypted.</param>
    private static void DeserializeSum(ref uint sum)
    {
        sum <<= 2;
        sum >>= 3;
        sum <<= 2;
        sum >>= 1;
        sum += 556;
        sum -= 2560;
        sum += 256;
        sum -= 1234;
        sum >>= 2;
        sum <<= 1;
    }

    /// <summary>
    /// Serializes the checksum value using a custom encryption algorithm.
    /// </summary>
    /// <param name="sum">The checksum value to be encrypted.</param>
    private static void SerializeSum(ref uint sum)
    {
        sum >>= 1;
        sum <<= 2;
        sum += 1234;
        sum -= 256;
        sum += 2560;
        sum -= 556;
        sum <<= 1;
        sum >>= 2;
        sum <<= 3;
        sum >>= 2;
    }
}