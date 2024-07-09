using System;
using System.Collections.Generic;
using System.Text;

namespace Net
{
    public static class MessageChecker
    {
        public static MessagePriority CheckMessagePriority(byte[] message)
        {
            return (MessagePriority)BitConverter.ToInt32(message, 4);
        }

        public static MessageType CheckMessageType(byte[] message)
        {
            return (MessageType)BitConverter.ToInt32(message, 0);
        }

        public static byte[] SerializeString(string input)
        {
            List<byte> outData = new List<byte>();

            byte[] stringBytes = ASCIIEncoding.UTF32.GetBytes(input);

            outData.AddRange(BitConverter.GetBytes(stringBytes.Length));

            outData.AddRange(stringBytes);

            return outData.ToArray();
        }

        public static string DeserializeString(byte[] message, ref int indexToInit)
        {
            int stringSize = BitConverter.ToInt32(message, indexToInit);
            
            indexToInit += 4;

            char[] charArray = new char[stringSize];

            for (int i = 0; i < stringSize; i++)
            {
                charArray[i] = BitConverter.ToChar(message, indexToInit + 4 * i);//CHECK VALUE DE LOS CHARS PORQUE VARIAN
            }

            indexToInit += stringSize * 4;

            return new string(charArray);
        }

        public static string DeserializeString(byte[] message, int indexToInit)
        {
            int stringSize = BitConverter.ToInt32(message, indexToInit);
            
            indexToInit += 4;

            char[] charArray = new char[stringSize];

            for (int i = 0; i < stringSize; i++)
            {
                charArray[i] = BitConverter.ToChar(message, indexToInit + 4 * i);//CHECK VALUE DE LOS CHARS PORQUE VARIAN
            }

            return new string(charArray);
        }

        public static bool DeserializeCheckSum(byte[] message)
        {
            uint messageSum = (uint)BitConverter.ToInt32(message, message.Length - sizeof(int));

            DeserializeSum(ref messageSum);

            if (messageSum != message.Length)
            {
                Console.Error.WriteLine("ERROR: Message Type " + CheckMessageType(message) + " (" + CheckMessagePriority(message) + ") got corrupted.");
                return false;
            }

            return true;
        }

        public static byte[] SerializeCheckSum(List<byte> data)
        {
            uint sum = (uint)(data.Count + sizeof(int));

            SerializeSum(ref sum);
            
            return BitConverter.GetBytes(sum);
        }

        static void DeserializeSum(ref uint sum)
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

        static void SerializeSum(ref uint sum)
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

        public static bool IsSorteableMessage(byte[] data)
        {
            return (CheckMessagePriority(data) & MessagePriority.Sorteable) != 0;
        }

        public static bool IsNondisponsableMessage(byte[] data)
        {
            return (CheckMessagePriority(data) & MessagePriority.NonDisposable) != 0;
        }
    }
}