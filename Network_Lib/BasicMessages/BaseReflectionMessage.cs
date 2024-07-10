using System.Collections.Generic;
using System;

namespace Net
{
    [System.Serializable]
    public struct RouteInfo
    {
        [NetVariable(0)] public int route;
        [NetVariable(1)] public int collectionIndex;
        [NetVariable(2)] public int collectionSize;

        public RouteInfo(int route, int collectionIndex = -1, int collectionSize = -1)
        {
            this.route = route;
            this.collectionIndex = collectionIndex;
            this.collectionSize = collectionSize;
        }

        public void SetRoute(int route)
        {
            this.route = route;
        }

        public void SetCollectionIndex(int collectionIndex)
        {
            this.collectionIndex = collectionIndex;
        }

        public void SetCollectionSize(int collectionSize)
        {
            this.collectionSize = collectionSize;
        }

        public int GetRoute()
        {
            return route;
        }

        public int GetCollectionIndex()
        {
            return collectionIndex;
        }

        public int GetCollectionSize()
        {
            return collectionSize;
        }

        public override string ToString()
        {
            return $"Route: {route} - CollectionIndex: {collectionIndex} - CollectionSize: {collectionSize}";
        }
    }


    public abstract class BaseReflectionMessage<T> : BaseMessage<T>
    {
        protected List<RouteInfo> messageRoute = new List<RouteInfo>();

        public BaseReflectionMessage(MessagePriority messagePriority, List<RouteInfo> messageRoute) : base(messagePriority)
        {
            this.messageRoute = messageRoute;
        }

        public List<RouteInfo> GetMessageRoute()
        {
            return messageRoute;
        }

        public override void SerializeHeader(ref List<byte> outData)
        {
            base.SerializeHeader(ref outData);
            SerializeMessageRoute(ref outData);
        }

        public override void DeserializeHeader(byte[] message)
        {
            base.DeserializeHeader(message);
            DeserializeMessageRoute(message);
        }

        protected void SerializeMessageRoute(ref List<byte> outData)
        {
            outData.AddRange(BitConverter.GetBytes(messageRoute.Count));

            foreach (RouteInfo info in messageRoute)
            {
                outData.AddRange(BitConverter.GetBytes(info.route));
                outData.AddRange(BitConverter.GetBytes(info.collectionIndex));
                outData.AddRange(BitConverter.GetBytes(info.collectionSize));
            }
        }

        protected void DeserializeMessageRoute(byte[] message)
        {
            int messageRouteLength = BitConverter.ToInt32(message, messageHeaderSize);
            messageHeaderSize += sizeof(int);

            if (messageRouteLength > 0)
            {
                for (int i = 0; i < messageRouteLength; i++)
                {
                    int route = BitConverter.ToInt32(message, messageHeaderSize);
                    messageHeaderSize += sizeof(int);
                    int collectionIndex = BitConverter.ToInt32(message, messageHeaderSize);
                    messageHeaderSize += sizeof(int);
                    int collectionSize = BitConverter.ToInt32(message, messageHeaderSize);
                    messageHeaderSize += sizeof(int);

                    messageRoute.Add(new RouteInfo(route, collectionIndex, collectionSize));
                }
            }
        }
    }
}