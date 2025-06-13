using System;
using System.Collections.Generic;

// ctrl R G  -- ctrl shift V -- shitf enter
namespace Net
{
    public class NetWin : BaseMessage<WinWrapper>
    {
        WinWrapper winner;

        public NetWin(WinWrapper error) : base(MessagePriority.Default) //Simepre van a ser default, lo dejo implicito aca desde el principio
        {
            currentMessageType = MessageType.Winner;
            this.winner = error;
        }

        public NetWin(byte[] message) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.Winner;
            winner = Deserialize(message);
        }

        public override WinWrapper Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            if (MessageChecker.DeserializeCheckSum(message))
            {
                winner.winner = BitConverter.ToInt32(message, messageHeaderSize);
            }

            return winner;
        }

        public WinWrapper GetData()
        {
            return winner;
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();

            SerializeHeader(ref outData);

            outData.AddRange(BitConverter.GetBytes(winner.winner));

            SerializeQueue(ref outData);

            return outData.ToArray();
        }
    }
}


public class WinWrapper
{
    public int winner;
    public WinWrapper(int winner)
    {
        this.winner = winner;
    }
}