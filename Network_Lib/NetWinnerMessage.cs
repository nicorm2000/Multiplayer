using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Net
{
    public class NetWinnerMessage : BaseMessage<WinnerInfo>
    {
        WinnerInfo winnerInfo;

        public NetWinnerMessage(MessagePriority messagePriority, WinnerInfo winnerId) : base(messagePriority)
        {
            currentMessageType = MessageType.Winner;
            this.winnerInfo = winnerId;
        }

        public NetWinnerMessage(byte[] data) : base(MessagePriority.Default)
        {
            currentMessageType = MessageType.Winner;
            this.winnerInfo = Deserialize(data);
        }

        public override WinnerInfo Deserialize(byte[] message)
        {
            DeserializeHeader(message);

            int winnerId = BitConverter.ToInt32(message, messageHeaderSize);

            float extra = BitConverter.ToSingle(message, messageHeaderSize + sizeof(int));

            if (!MessageChecker.DeserializeCheckSum(message))
            {
                return new WinnerInfo(-1);
            }

            return new WinnerInfo(winnerId);
        }

        public WinnerInfo GetData()
        {
            return winnerInfo ?? new WinnerInfo(-1);
        }

        public override byte[] Serialize()
        {
            List<byte> outData = new List<byte>();
            SerializeHeader(ref outData);
            outData.AddRange(BitConverter.GetBytes(winnerInfo.winner));
            outData.AddRange(BitConverter.GetBytes(winnerInfo.extra));
            outData.AddRange(MessageChecker.SerializeCheckSum(outData));
            return outData.ToArray();
        }
    }

    public class WinnerInfo
    {
        public readonly int winner;
        public readonly float extra;
        public WinnerInfo(int winner)
        { 
            this.winner = winner;
            this.extra = 1.0f;
        }
    }
}
