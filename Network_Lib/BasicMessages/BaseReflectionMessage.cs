using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Collections;
using System.Data;
using System.Text;
using System.IO;
using System.Linq;

namespace Net
{
    [System.Serializable]
    public struct RouteInfo
    {
        public int route;
        public int collectionKey;
        public int collectionSize;
        public byte flags;

        private const byte IS_COLLECTION = 0x01;
        private const byte IS_DICTIONARY = 0x02;
        private const byte IS_MULTIDIM_ARRAY = 0x04;
        private const byte IS_JAGGED_ARRAY = 0x08;

        private Type _elementType;
        private int[] _dimensions;

        public RouteInfo(int route, int collectionKey = -1, int collectionSize = -1, byte flags = 0, Type? elementType = null, int[]? dimensions = null)
        {
            this.route = route;
            this.collectionKey = collectionKey;
            this.collectionSize = collectionSize;
            this.flags = flags;
            this._elementType = elementType;
            this._dimensions = dimensions ?? Array.Empty<int>();
        }

        public bool IsCollection => (flags & IS_COLLECTION) != 0;
        public bool IsDictionary => (flags & IS_DICTIONARY) != 0;
        public bool IsMultiDimensionalArray => (flags & IS_MULTIDIM_ARRAY) != 0;
        public bool IsJaggedArray => (flags & IS_JAGGED_ARRAY) != 0;
        public int[] Dimensions
        {
            get => _dimensions;
            set => _dimensions = value ?? Array.Empty<int>();
        }
        public Type ElementType => _elementType;

        public static RouteInfo CreateForProperty(int routeId) => new RouteInfo(routeId, -1, -1, 0, null);

        public static RouteInfo CreateForCollection(int routeId, int index, int size, Type elementType) => new RouteInfo(routeId, index, size, IS_COLLECTION, elementType);

        public static RouteInfo CreateForDictionary(int routeId, int keyHash, Type valueType) => new RouteInfo(routeId, keyHash, -1, (byte)(IS_COLLECTION | IS_DICTIONARY), valueType);

        public static RouteInfo CreateForMultiDimensionalArray(int routeId, int[] indices, int[] dimensions, Type elementType)
        {
            if (dimensions == null || dimensions.Length < 2) throw new ArgumentException("There needs to be at least 2 dimensions");

            return new RouteInfo(
                routeId,
                LinearizeIndices(indices, dimensions),
                CalculateTotalSize(dimensions),
                (byte)(IS_COLLECTION | IS_MULTIDIM_ARRAY),
                elementType,
                dimensions);
        }

        public static RouteInfo CreateForJaggedArray(int routeId, int[] pathIndices, Type elementType) =>
            new RouteInfo(routeId, pathIndices[0], -1, (byte)(IS_COLLECTION | IS_JAGGED_ARRAY), elementType, pathIndices);

        private static int LinearizeIndices(int[] indices, int[] dimensions)
        {
            int index = 0;
            for (int i = 0; i < indices.Length; i++)
            {
                if (indices[i] >= dimensions[i])
                    throw new IndexOutOfRangeException();
                index = index * dimensions[i] + indices[i];
            }
            return index;
        }

        public int[] GetMultiDimensionalIndices()
        {
            if (!IsMultiDimensionalArray || _dimensions.Length == 0)
                return Array.Empty<int>();

            int[] indices = new int[_dimensions.Length];
            int remainder = collectionKey;

            for (int i = _dimensions.Length - 1; i >= 0; i--)
            {
                indices[i] = remainder % _dimensions[i];
                remainder /= _dimensions[i];
            }

            return indices;
        }

        private static int CalculateTotalSize(int[] dimensions) => dimensions.Aggregate(1, (acc, dim) => acc * dim);

        public byte[] Serialize()
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(route);
                writer.Write(collectionKey);
                writer.Write(collectionSize);
                writer.Write(flags);

                writer.Write(_dimensions?.Length ?? 0);
                foreach (var dim in _dimensions ?? Array.Empty<int>())
                    writer.Write(dim);

                string typeSig = GetTypeSignature();
                writer.Write(typeSig.Length);
                writer.Write(Encoding.UTF8.GetBytes(typeSig));

                return stream.ToArray();
            }
        }

        public static RouteInfo Deserialize(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int route = reader.ReadInt32();
                int key = reader.ReadInt32();
                int size = reader.ReadInt32();
                byte flags = reader.ReadByte();

                int dimCount = reader.ReadInt32();
                int[] dimensions = new int[dimCount];
                for (int i = 0; i < dimCount; i++)
                    dimensions[i] = reader.ReadInt32();

                int typeLen = reader.ReadInt32();
                string typeSig = Encoding.UTF8.GetString(reader.ReadBytes(typeLen));
                Type elementType = ParseTypeSignature(typeSig);

                return new RouteInfo(route, key, size, flags, elementType, dimensions);
            }
        }

        private string GetTypeSignature()
        {
            if (_elementType == null) return string.Empty;

            if (_elementType.IsArray)
            {
                int rank = _elementType.GetArrayRank();
                return rank > 1 ? $"{_elementType.GetElementType()?.AssemblyQualifiedName}[{new string(',', rank - 1)}]" : _elementType.AssemblyQualifiedName;
            }

            return _elementType.AssemblyQualifiedName;
        }

        private static Type ParseTypeSignature(string signature)
        {
            if (string.IsNullOrEmpty(signature)) return null;

            try
            {      
                if (signature.Contains("[")) // Special multi dimensional array handling
                {
                    int bracketIndex = signature.IndexOf('[');
                    string baseType = signature.Substring(0, bracketIndex);
                    string dimensions = signature.Substring(bracketIndex);

                    Type elementType = Type.GetType(baseType);
                    if (elementType == null) return null;

                    int rank = dimensions.Count(c => c == ',') + 1;
                    return rank > 1 ? elementType.MakeArrayType(rank) : elementType.MakeArrayType();
                }

                return Type.GetType(signature);
            }
            catch
            {
                return null;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Route: {route} | Key: {collectionKey} | Size: {collectionSize} | ");
            sb.Append($"Flags: {Convert.ToString(flags, 2).PadLeft(8, '0')}");

            if (_dimensions?.Length > 0)
                sb.Append($" | Dims: {string.Join("x", _dimensions)}");

            if (_elementType != null)
                sb.Append($" | Type: {_elementType.Name}");

            return sb.ToString();
        }
    }

    public abstract class BaseReflectionMessage<T> : BaseMessage<T>
    {
        protected List<RouteInfo> messageRoute = new List<RouteInfo>();

        public BaseReflectionMessage(MessagePriority messagePriority, List<RouteInfo> messageRoute) : base(messagePriority)
        {
            this.messageRoute = messageRoute ?? new List<RouteInfo>();
        }

        public List<RouteInfo> GetMessageRoute() => new List<RouteInfo>(messageRoute);

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
                outData.AddRange(info.Serialize());
            }
        }

        protected void DeserializeMessageRoute(byte[] message)
        {
            int offset = messageHeaderSize;
            int count = BitConverter.ToInt32(message, offset);
            offset += sizeof(int);

            messageRoute.Clear();
            for (int i = 0; i < count; i++)
            {
                int routeSize = CalculateRouteSize(message, offset);
                byte[] routeData = new byte[routeSize];
                Buffer.BlockCopy(message, offset, routeData, 0, routeSize);

                messageRoute.Add(RouteInfo.Deserialize(routeData));
                offset += routeSize;
            }
            messageHeaderSize = offset;
        }

        private int CalculateRouteSize(byte[] message, int offset)
        {
            const int baseSize = 13; // route + key + size + flags

            int dimCount = BitConverter.ToInt32(message, offset + baseSize); // count + values(n*4)
            int dimsSize = sizeof(int) + (dimCount * sizeof(int));

            int typeLenOffset = offset + baseSize + dimsSize; // length(4) + bytes
            int typeLen = BitConverter.ToInt32(message, typeLenOffset);
            int typeSize = sizeof(int) + typeLen;

            return baseSize + dimsSize + typeSize;
        }

        public string GetRouteDescription()
        {
            StringBuilder sb = new StringBuilder("Message Route:\n");
            for (int i = 0; i < messageRoute.Count; i++)
            {
                RouteInfo info = messageRoute[i];
                sb.Append($"[{i}] {info}\n");

                if (info.IsDictionary)
                {
                    sb.AppendLine($"  • Dictionary Key Hash: {info.collectionKey}");
                }
                else if (info.IsCollection)
                {
                    sb.AppendLine($"  • Collection Index: {info.collectionKey}");
                    if (info.IsMultiDimensionalArray)
                    {
                        int[] indices = info.GetMultiDimensionalIndices();
                        sb.AppendLine($"  • MD Indices: [{string.Join(",", indices)}]");
                    }
                    else if (info.IsJaggedArray)
                    {
                        sb.AppendLine($"  • Jagged Path: [{string.Join("][", info.Dimensions)}]");
                    }
                }

                if (info.ElementType != null)
                {
                    sb.AppendLine($"  • Element Type: {info.ElementType.FullName}");
                }

                if (info.Dimensions.Length > 0)
                {
                    sb.AppendLine($"  • Dimensions: {string.Join("x", info.Dimensions)}");
                }
            }
            return sb.ToString();
        }
    }
}