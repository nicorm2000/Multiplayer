using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Collections;
using System.Data;

namespace Net
{
    // New Route Info ACORDATE DE MODIFICAR EL HEADER DE LOS MENSAJES SI USAS EL NUEVO
    //public int Route { get; set; }
    //public IEnumerable Collection { get; set; }
    //public object Index { get; set; }
    //
    //public RouteInfo(int route, IEnumerable collection = null, object index = null)
    //{
    //    Route = route;
    //    Collection = collection;
    //    Index = index;
    //}
    //
    //// This will handle access for any ICollection or IEnumerable
    //public object GetItem()
    //{
    //    if (Collection == null)
    //        return null;
    //
    //    if (Collection is IDictionary dictionary && Index != null)
    //    {
    //        // Dictionary: Handle key-based lookup
    //        return dictionary[Index];
    //    }
    //    else if (Collection is IEnumerable enumerable)
    //    {
    //        if (Index is int index) // Single index for any IEnumerable (e.g., List<T>, Array)
    //        {
    //            int count = 0;
    //            foreach (var item in enumerable)
    //            {
    //                if (count == index)
    //                    return item;
    //                count++;
    //            }
    //            return null; // Index out of range
    //        }
    //        else if (Index is IEnumerable<int> indexList) // Handling multi-indexing for multidimensional arrays
    //        {
    //            var enumerator = enumerable.GetEnumerator();
    //            foreach (var idx in indexList)
    //            {
    //                if (!enumerator.MoveNext())
    //                    return null;
    //            }
    //            return enumerator.Current;
    //        }
    //        return "Unsupported index type.";
    //    }
    //    return "Unsupported collection type.";
    //}
    //
    //public override string ToString()
    //{
    //    return $"Route: {Route}, Collection Type: {Collection?.GetType().Name ?? "null"}, Index: {Index}";
    //}
    [System.Serializable]
    public struct RouteInfo
    {
        public int route;
        public int collectionIndex;
        public int collectionSize;

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