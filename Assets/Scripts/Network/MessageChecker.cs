using System;

public class MessageChecker
{
    public MessageType CheckMessageType(byte[] message)
    {
        int messageType = 0;

        messageType = BitConverter.ToInt32(message, 0);

        return (MessageType)messageType;
    }

    public int CheckClientID(byte[] message)
    {
        int clientID = 0;

        clientID = BitConverter.ToInt32(message, 0);

        return clientID;
    }
}