using System.Runtime.Serialization;
using System.Collections.Generic;
using Network_Lib.BasicMessages;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Net;
using System;
using System.Diagnostics;

namespace Net
{
    /// <summary>
    /// Represents the possible states for null, empty or remove values in network communication.
    /// </summary>
    public enum PossibleStates
    {
        Null,
        Empty,
        Remove
    }

    /// <summary>
    /// Provides reflection-based inspection and manipulation of network objects.
    /// Handles serialization, deserialization, and network communication of object states.
    /// </summary>
    public class Reflection
    {
        #region Fields and Properties
        private BindingFlags bindingFlags;
        private Assembly executeAssembly;
        private Assembly gameAssembly;
        private NetworkEntity networkEntity;

        public static Action<string> consoleDebugger;
        public static Action consoleDebuggerPause;

        public Dictionary<Type, MethodInfo> extensionMethods = new Dictionary<Type, MethodInfo>();
        private Dictionary<object, Dictionary<object, int>> previousDictionaryStates = new Dictionary<object, Dictionary<object, int>>();
        private readonly Dictionary<object, int> previousCollectionCounts = new Dictionary<object, int>();
        #endregion

        #region Initialization
        /// <summary>
        /// Provides reflection-based inspection and manipulation of network objects.
        /// Handles serialization, deserialization, and network communication of object states.
        /// </summary>
        public Reflection(NetworkEntity entity)
        {
            networkEntity = entity;
            networkEntity.OnReceivedMessage += OnReceivedReflectionMessage;

            executeAssembly = Assembly.GetExecutingAssembly();
            gameAssembly = Assembly.GetCallingAssembly();
            bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            foreach (Type type in gameAssembly.GetTypes())
            {
                NetExtensionClass netExtensionClass = type.GetCustomAttribute<NetExtensionClass>();
                if (netExtensionClass != null)
                {
                    foreach (MethodInfo methodInfo in type.GetMethods())
                    {
                        NetExtensionMethod netExtensionMethod = methodInfo.GetCustomAttribute<NetExtensionMethod>();
                        if (netExtensionMethod != null)
                        {
                            extensionMethods.TryAdd(netExtensionMethod.extensionMethod, methodInfo);
                            //consoleDebugger?.Invoke($"Registered extension for: {netExtensionMethod.extensionMethod.Name}");
                        }
                    }
                }
            }

        }
        #endregion

        #region Main Inspection Logic
        /// <summary>
        /// Updates the reflection state by inspecting all network objects owned by this client.
        /// </summary>
        public void UpdateReflection()
        {
            if (NetObjFactory.NetObjectsCount <= 0)
            {
                return;
            }

            foreach (INetObj netObj in NetObjFactory.NetObjects())
            {
                if (netObj.GetOwnerID() == networkEntity.clientID)
                {
                    List<RouteInfo> idRoute = new List<RouteInfo>
                    {
                        RouteInfo.CreateForProperty(netObj.GetID())
                    };
                    Inspect(netObj.GetType(), netObj, idRoute);

                    if (netObj.GetTRS() != null)
                    {
                        TRS trs = netObj.GetTRS();
                        NetTRSMessage netTRSMessage = new NetTRSMessage(MessagePriority.Default, trs, idRoute);
                        networkEntity.SendMessage(netTRSMessage.Serialize());
                    }
                }
            }
        }

        /// <summary>
        /// Inspects an object and its fields recursively, sending network messages for any changes.
        /// </summary>
        /// <param name="type">The type of the object to inspect.</param>
        /// <param name="obj">The object instance to inspect.</param>
        /// <param name="idRoute">The route information for network message routing.</param>
        public void Inspect(Type type, object obj, List<RouteInfo> idRoute)
        {
            string debug = "";
            if (obj != null)
            {
                foreach (FieldInfo info in GetAllFields(type))
                {
                    debug += "___info field: " + info.FieldType + "\n";
                    debug += "___info route: " + idRoute[0].route + "\n";
                    //consoleDebugger.Invoke(debug);
                    IEnumerable<Attribute> attributes = info.GetCustomAttributes();
                    foreach (Attribute attribute in attributes)
                    {
                        if (attribute is NetVariable netVariable)
                        {
                            //consoleDebugger.Invoke($"Inspect: {type} - {obj} - {info} - {info.GetType()} - {info.GetValue(obj)}");
                            ReadValue(info, obj, netVariable, new List<RouteInfo>(idRoute));
                        }
                    }
                    if (extensionMethods.TryGetValue(type, out MethodInfo methodInfo))
                    {
                        object unitializedObject = FormatterServices.GetUninitializedObject(type);
                        object fields = methodInfo.Invoke(null, new object[] { unitializedObject });

                        if (fields != null)
                        {
                            List<(FieldInfo, NetVariable)> values = (List<(FieldInfo, NetVariable)>)fields;
                            foreach ((FieldInfo, NetVariable) field in values)
                            {
                                //consoleDebugger.Invoke($"Inspect: {type} - {obj} - {info} - {info.GetType()} - {info.GetValue(obj)}, fields Item1: " + field.Item1);
                                ReadValue(field.Item1, obj, field.Item2, new List<RouteInfo>(idRoute));
                            }
                        }
                    }
                    if (type.BaseType != null)
                    {
                        Inspect(type.BaseType, obj, new List<RouteInfo>(idRoute));
                    }
                }
                debug += "Exit foreach: " + obj + "\n";
                //consoleDebugger.Invoke(debug);
            }
            else
            {
                debug += "Object is NULL";
                //consoleDebugger.Invoke(debug);
            }
        }

        #endregion

        #region Value Processing
        /// <summary>
        /// Reads and processes the value of a field, sending appropriate network messages.
        /// </summary>
        /// <param name="info">Field information.</param>
        /// <param name="obj">Parent object containing the field.</param>
        /// <param name="attribute">NetVariable attribute of the field.</param>
        /// <param name="idRoute">Route information for network message routing.</param>
        public void ReadValue(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute)
        {
            string debug = "ReadValue Start - ";
            debug += $"Field: {info.Name}, Type: {info.FieldType}, Current Route: {string.Join("->", idRoute.Select(r => r.route))}\n";
            //consoleDebugger?.Invoke(debug);

            object fieldValue = info.GetValue(obj);
            Type fieldType = info.FieldType;

            // Handle null case
            if (fieldValue == null)
            {
                //consoleDebugger?.Invoke("Field is NULL\n");
                idRoute.Add(RouteInfo.CreateForProperty(attribute.VariableId));
                SendPackage(PossibleStates.Null, attribute, idRoute);
                return;
            }

            // Handle simple types
            if (IsSimpleType(fieldType))
            {
                //consoleDebugger?.Invoke("Handling primitive/string/enum type\n");
                idRoute.Add(RouteInfo.CreateForProperty(attribute.VariableId));
                SendPackage(fieldValue, attribute, idRoute);
                return;
            }

            // Handle collections
            if (typeof(IEnumerable).IsAssignableFrom(fieldType))
            {
                if (fieldType.IsArray && fieldType.GetArrayRank() > 1)
                {
                    // Multi-dimensional array handling (unchanged)
                    Array mdArray = (Array)fieldValue;
                    int[] dimensions = new int[mdArray.Rank];
                    for (int i = 0; i < mdArray.Rank; i++)
                    {
                        dimensions[i] = mdArray.GetLength(i);
                    }

                    foreach (int[] indices in GetArrayIndices(mdArray))
                    {
                        object element = mdArray.GetValue(indices);
                        List<RouteInfo> currentRoute = new List<RouteInfo>(idRoute)
                        {
                            RouteInfo.CreateForMultiDimensionalArray(
                                attribute.VariableId,
                                indices,
                                dimensions,
                                element?.GetType() ?? fieldType.GetElementType())
                        };
                        ProcessValue(element, currentRoute, attribute);
                    }
                }
                else if (typeof(IDictionary).IsAssignableFrom(fieldType))
                {
                    IDictionary dictionary = (IDictionary)fieldValue;
                    Type valueType = fieldType.GetGenericArguments()[1];

                    // Track changes
                    List<object> currentKeys = dictionary.Keys.Cast<object>().ToList();
                    debug += ($"Current Keys: {string.Join(",", currentKeys)}\n");

                    // Check for removals FIRST
                    if (previousDictionaryStates.TryGetValue(dictionary, out Dictionary<object, int>? previousKeys))
                    {
                        List<object> removedKeys = previousKeys.Keys.Except(currentKeys).ToList();
                        debug += ($"Removed Keys: {(removedKeys.Any() ? string.Join(",", removedKeys) : "none")}\n");

                        // Process removals FIRST and RETURN
                        if (removedKeys.Any())
                        {
                            foreach (object? key in removedKeys)
                            {
                                int keyHash = GetStableKeyHash(key);
                                debug += ($"Sending Remove for Key: {key} (Hash: {keyHash})\n");

                                List<RouteInfo> removeRoute = new List<RouteInfo>(idRoute)
                                {
                                    RouteInfo.CreateForDictionary(attribute.VariableId, keyHash, valueType)
                                };

                                // Create and send dedicated Remove message
                                NetRemoveMessage removeMessage = new NetRemoveMessage(
                                    attribute.MessagePriority,
                                    keyHash,
                                    removeRoute);

                                byte[] serialized = removeMessage.Serialize();
                                //consoleDebugger?.Invoke($"Sending Remove - Full Data: {BitConverter.ToString(serialized)}");
                                networkEntity.SendMessage(serialized);
                            }

                            // Update state and RETURN after processing removals
                            previousDictionaryStates[dictionary] = currentKeys.ToDictionary(k => k, GetStableKeyHash);
                            debug += ("--- REMOVALS PROCESSED ---");
                            //consoleDebugger?.Invoke(debug.ToString());
                            return;
                        }
                    }

                    // Only process current values if no removals occurred
                    foreach (DictionaryEntry entry in dictionary)
                    {
                        ProcessValue(entry.Value, new List<RouteInfo>(idRoute)
                        {
                            RouteInfo.CreateForDictionary(attribute.VariableId, GetStableKeyHash(entry.Key), valueType)
                        }, attribute);
                    }

                    // Handle empty dictionary
                    if (dictionary.Count == 0)
                    {
                        SendPackage(PossibleStates.Empty, attribute, new List<RouteInfo>(idRoute)
                        {
                            RouteInfo.CreateForDictionary(attribute.VariableId, -1, valueType)
                        });
                    }

                    // Update state
                    previousDictionaryStates[dictionary] = currentKeys.ToDictionary(k => k, GetStableKeyHash);

                    debug += ("--- INSPECTION COMPLETE ---");
                    //consoleDebugger?.Invoke(debug);
                    return;
                }
                else
                {
                    //consoleDebugger?.Invoke($"Processing as generic collection: {fieldType.Name}");

                    IEnumerable collection = (IEnumerable)fieldValue;
                    int count = 0;
                    int index = 0;

                    // Get count via enumeration (works for any IEnumerable)
                    IEnumerator enumerator = collection.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        count++;
                    }

                    int previousCount = -1;
                    if (previousCollectionCounts.TryGetValue(fieldValue, out previousCount))
                    {
                        if (previousCount > count)
                        {
                            int removedCount = previousCount - count;

                            for (int i = 0; i < removedCount; i++)
                            {
                                int removedIndex = previousCount - i - 1;

                                List<RouteInfo> removeRoute = new List<RouteInfo>(idRoute)
                                {
                                    RouteInfo.CreateForCollection(
                                        routeId: attribute.VariableId,
                                        index: removedIndex,
                                        size: count,
                                        elementType: GetElementType(fieldType))
                                };

                                NetRemoveMessage removeMessage = new NetRemoveMessage(
                                    attribute.MessagePriority,
                                    -1,
                                    removeRoute);

                                byte[] serialized = removeMessage.Serialize();
                                //consoleDebugger?.Invoke($"[ReadValue] Sending Remove for index {removedIndex} (count reduced) - Data: {BitConverter.ToString(serialized)}");
                                networkEntity.SendMessage(serialized);
                            }
                        }
                    }

                    // Update count tracking
                    previousCollectionCounts[fieldValue] = count;

                    // Process items
                    enumerator = collection.GetEnumerator(); // Reset enumerator
                    while (enumerator.MoveNext())
                    {
                        object? item = enumerator.Current;
                        List<RouteInfo> currentRoute = new List<RouteInfo>(idRoute)
                        {
                            RouteInfo.CreateForCollection(
                                routeId: attribute.VariableId,
                                index: index++,
                                size: count,
                                elementType: item?.GetType() ?? GetElementType(fieldType))
                        };

                        //consoleDebugger?.Invoke($"Processing collection item [{index - 1}]: " +
                        //                      $"Type: {item?.GetType()?.Name ?? "null"}, " +
                        //                      $"Value: {item ?? "null"}");

                        ProcessValue(item, currentRoute, attribute);
                    }

                    if (count == 0)
                    {
                        //consoleDebugger?.Invoke("Collection is empty");
                        idRoute.Add(new RouteInfo(
                            attribute.VariableId,
                            collectionKey: -1,
                            collectionSize: 0,
                            elementType: GetElementType(fieldType)));
                        SendPackage(PossibleStates.Empty, attribute, idRoute);
                    }
                }
            }

            // Handle complex objects
            //consoleDebugger?.Invoke("Handling complex object type\n");
            idRoute.Add(RouteInfo.CreateForProperty(attribute.VariableId));
            Inspect(fieldType, fieldValue, idRoute);
        }
        #endregion

        #region Send Message
        /// <summary>
        /// Sends a network package based on the value type and route information.
        /// </summary>
        /// <param name="value">The value to send.</param>
        /// <param name="attribute">NetVariable attribute containing metadata.</param>
        /// <param name="idRoute">Route information for message routing.</param>
        public void SendPackage(object value, NetVariable attribute, List<RouteInfo> idRoute)
        {
            string debug = "SendPackage - ";
            debug += $"Value Type: {value?.GetType().Name ?? "null"}";
            debug += $"Route: {string.Join("->", idRoute.Select(r => $"{r.route}[{r.collectionKey}]"))}";
            //consoleDebugger?.Invoke(debug);

            if (value is PossibleStates.Null)
            {
                //consoleDebugger?.Invoke("Sending NullMessage\n");
                NetNullMessage netNullMessage = new NetNullMessage(attribute.MessagePriority, null, idRoute);
                networkEntity.SendMessage(netNullMessage.Serialize());
                return;
            }

            if (value is PossibleStates.Empty)
            {
                //consoleDebugger?.Invoke("Sending EmptyMessage\n");
                NetEmptyMessage netEmptyMessage = new NetEmptyMessage(attribute.MessagePriority, new Empty(), idRoute);
                networkEntity.SendMessage(netEmptyMessage.Serialize());
                return;
            }

            if (value is PossibleStates.Remove)
            {
                //consoleDebugger?.Invoke("Sending RemoveMessage\n");
                int keyHash = idRoute.Last().collectionKey;
                NetRemoveMessage netRemoveMessage = new NetRemoveMessage(attribute.MessagePriority, keyHash, idRoute);
                byte[] serialized = netRemoveMessage.Serialize();
                //consoleDebugger?.Invoke($"Sending Remove - KeyHash: {keyHash}, Data: {BitConverter.ToString(serialized)}");
                networkEntity.SendMessage(serialized);
                return;
            }

            if (value is Enum enumValue)
            {
                //consoleDebugger?.Invoke("Sending Enum package\n");
                NetEnumMessage enumMessage = new NetEnumMessage(attribute.MessagePriority, enumValue, idRoute);
                networkEntity.SendMessage(enumMessage.Serialize());
                return;
            }

            Type packageType = value.GetType();
            debug += $"Looking for message type for {packageType.Name}\n";

            foreach (Type type in executeAssembly.GetTypes())
            {
                if (type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(BaseReflectionMessage<>))
                {
                    Type[] genericTypes = type.BaseType.GetGenericArguments();
                    foreach (Type arg in genericTypes)
                    {
                        if (packageType == arg)
                        {
                            debug += $"Found matching message type: {type.Name}\n";
                            try
                            {
                                object[] parameters = new[] { attribute.MessagePriority, value, idRoute };
                                ConstructorInfo ctor = type.GetConstructor(new[] { typeof(MessagePriority), packageType, typeof(List<RouteInfo>) });
                                if (ctor != null)
                                {
                                    ParentBaseMessage message = (ParentBaseMessage)ctor.Invoke(parameters);
                                    debug += $"Message created successfully. Serializing...\n";
                                    //consoleDebugger?.Invoke(debug);
                                    networkEntity.SendMessage(message.Serialize());
                                    return;
                                }
                                else
                                {
                                    debug += $"Constructor not found for {type.Name}\n";
                                }
                            }
                            catch (Exception ex)
                            {
                                debug += $"Error creating message: {ex.Message}\n";
                            }
                        }
                    }
                }
            }

            debug += $"No suitable message type found for {packageType.Name}\n";
            //consoleDebugger?.Invoke(debug);
        }
        #endregion

        #region Message Handling
        /// <summary>
        /// Handles received reflection messages and routes them to appropriate processing methods.
        /// </summary>
        /// <param name="data">The raw message data.</param>
        /// <param name="ip">The endpoint from which the message was received.</param>
        public void OnReceivedReflectionMessage(byte[] data, IPEndPoint ip)
        {
            try
            {
                //consoleDebugger?.Invoke($"Received {data.Length} bytes: {BitConverter.ToString(data)}");
                //int rawType = BitConverter.ToInt32(data, 0);
                //consoleDebugger?.Invoke($"First 4 bytes: {rawType} ({(MessageType)rawType})");

                string debug = "OnReceivedReflectionMessage - ";
                //// Verify minimum length
                //if (data == null || data.Length < 4)
                //{
                //    consoleDebugger?.Invoke("ERROR: Message too short");
                //    return;
                //}

                // Directly read message type from first 4 bytes
                //MessageType messageType = (MessageType)BitConverter.ToInt32(data, 0);
                MessageType messageType = MessageChecker.CheckMessageType(data);
                //consoleDebugger?.Invoke($"Received MessageType: {messageType}");
                //consoleDebugger?.Invoke($"\nRAW DATA RECEIVED ({data?.Length ?? 0} bytes): {BitConverter.ToString(data ?? new byte[0])}");
                debug += $"Message Type: {messageType}\n";

                switch (messageType)
                {
                    case MessageType.Ulong:
                        debug += "Processing Ulong message\n";
                        NetULongMessage netULongMessage = new NetULongMessage(data);
                        debug += $"Data: {netULongMessage.GetData()}, Route: {string.Join("->", netULongMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //consoleDebugger?.Invoke(debug);
                        VariableMapping(netULongMessage.GetMessageRoute(), netULongMessage.GetData());
                        break;

                    case MessageType.Uint:
                        debug += "Processing Uint message\n";
                        NetUIntMessage netUIntMessage = new NetUIntMessage(data);
                        debug += $"Data: {netUIntMessage.GetData()}, Route: {string.Join("->", netUIntMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //consoleDebugger?.Invoke(debug);
                        VariableMapping(netUIntMessage.GetMessageRoute(), netUIntMessage.GetData());
                        break;

                    case MessageType.Ushort:
                        debug += "Processing Ushort message\n";
                        NetUShortMessage netUShortMessage = new NetUShortMessage(data);
                        debug += $"Data: {netUShortMessage.GetData()}, Route: {string.Join("->", netUShortMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //consoleDebugger?.Invoke(debug);
                        VariableMapping(netUShortMessage.GetMessageRoute(), netUShortMessage.GetData());
                        break;

                    case MessageType.String:
                        debug += "Processing String message\n";
                        NetStringMessage netStringMessage = new NetStringMessage(data);
                        debug += $"Data: {netStringMessage.GetData()}, Route: {string.Join("->", netStringMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //consoleDebugger?.Invoke(debug);
                        VariableMapping(netStringMessage.GetMessageRoute(), netStringMessage.GetData());
                        break;

                    case MessageType.Short:
                        debug += "Processing Short message\n";
                        NetShortMessage netShortMessage = new NetShortMessage(data);
                        debug += $"Data: {netShortMessage.GetData()}, Route: {string.Join("->", netShortMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //consoleDebugger?.Invoke(debug);
                        VariableMapping(netShortMessage.GetMessageRoute(), netShortMessage.GetData());
                        break;

                    case MessageType.Sbyte:
                        debug += "Processing Sbyte message\n";
                        NetSByteMessage netSByteMessage = new NetSByteMessage(data);
                        debug += $"Data: {netSByteMessage.GetData()}, Route: {string.Join("->", netSByteMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //consoleDebugger?.Invoke(debug);
                        VariableMapping(netSByteMessage.GetMessageRoute(), netSByteMessage.GetData());
                        break;

                    case MessageType.Long:
                        debug += "Processing Long message\n";
                        NetLongMessage netLongMessage = new NetLongMessage(data);
                        debug += $"Data: {netLongMessage.GetData()}, Route: {string.Join("->", netLongMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //consoleDebugger?.Invoke(debug);
                        VariableMapping(netLongMessage.GetMessageRoute(), netLongMessage.GetData());
                        break;

                    case MessageType.Int:
                        debug += "Processing Int message\n";
                        NetIntMessage netIntMessage = new NetIntMessage(data);
                        debug += $"Data: {netIntMessage.GetData()}, Route: {string.Join("->", netIntMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //consoleDebugger?.Invoke(debug);
                        VariableMapping(netIntMessage.GetMessageRoute(), netIntMessage.GetData());
                        break;

                    case MessageType.Float:
                        debug += "Processing Float message\n";
                        NetFloatMessage netFloatMessage = new NetFloatMessage(data);
                        debug += $"Data: {netFloatMessage.GetData()}, Route: {string.Join("->", netFloatMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //consoleDebugger?.Invoke(debug);
                        VariableMapping(netFloatMessage.GetMessageRoute(), netFloatMessage.GetData());
                        break;

                    case MessageType.Double:
                        debug += "Processing Double message\n";
                        NetDoubleMessage netDoubleMessage = new NetDoubleMessage(data);
                        debug += $"Data: {netDoubleMessage.GetData()}, Route: {string.Join("->", netDoubleMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //consoleDebugger?.Invoke(debug);
                        VariableMapping(netDoubleMessage.GetMessageRoute(), netDoubleMessage.GetData());
                        break;

                    case MessageType.Decimal:
                        debug += "Processing Decimal message\n";
                        NetDecimalMessage netDecimalMessage = new NetDecimalMessage(data);
                        debug += $"Data: {netDecimalMessage.GetData()}, Route: {string.Join("->", netDecimalMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //consoleDebugger?.Invoke(debug);
                        VariableMapping(netDecimalMessage.GetMessageRoute(), netDecimalMessage.GetData());
                        break;

                    case MessageType.Char:
                        debug += "Processing Char message\n";
                        NetCharMessage netCharMessage = new NetCharMessage(data);
                        debug += $"Data: {netCharMessage.GetData()}, Route: {string.Join("->", netCharMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //consoleDebugger?.Invoke(debug);
                        VariableMapping(netCharMessage.GetMessageRoute(), netCharMessage.GetData());
                        break;

                    case MessageType.Byte:
                        debug += "Processing Byte message\n";
                        NetByteMessage netByteMessage = new NetByteMessage(data);
                        debug += $"Data: {netByteMessage.GetData()}, Route: {string.Join("->", netByteMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //consoleDebugger?.Invoke(debug);
                        VariableMapping(netByteMessage.GetMessageRoute(), netByteMessage.GetData());
                        break;

                    case MessageType.Bool:
                        debug += "Processing Bool message\n";
                        NetBoolMessage netBoolMessage = new NetBoolMessage(data);
                        debug += $"Data: {netBoolMessage.GetData()}, Route: {string.Join("->", netBoolMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //consoleDebugger?.Invoke(debug);
                        VariableMapping(netBoolMessage.GetMessageRoute(), netBoolMessage.GetData());
                        break;

                    case MessageType.Null:
                        debug += "Processing Null message\n";
                        NetNullMessage netNullMessage = new NetNullMessage(data);
                        debug += $"Data: {netNullMessage.GetData()}, Route: {string.Join("->", netNullMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //consoleDebugger?.Invoke(debug);
                        VariableMappingNullException(netNullMessage.GetMessageRoute(), netNullMessage.GetData());
                        break;

                    case MessageType.Empty:
                        debug += "Processing Empty message\n";
                        NetEmptyMessage netEmptyMessage = new NetEmptyMessage(data);
                        debug += $"Data: {netEmptyMessage.GetData()}, Route: {string.Join("->", netEmptyMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //consoleDebugger?.Invoke(debug);
                        VariableMappingEmpty(netEmptyMessage.GetMessageRoute(), netEmptyMessage.GetData());
                        break;

                    case MessageType.Method:
                        debug += "Processing Method message\n";
                        NetMethodMessage netMethodMessage = new NetMethodMessage(data);
                        (int, List<(string, string)>) methodData = netMethodMessage.GetData();
                        debug += $"Method: {methodData.Item1}, Args: {string.Join(", ", methodData.Item2)}, Route: {netMethodMessage.GetMessageRoute()[0].route}\n";
                        //consoleDebugger?.Invoke(debug);
                        InvokeReflectionMethod(methodData.Item1, methodData.Item2, netMethodMessage.GetMessageRoute()[0].route);
                        break;

                    case MessageType.Remove:
                        debug += "Processing Remove message\n";
                        NetRemoveMessage removeMessage = new NetRemoveMessage(data);
                        debug += $"Data: {removeMessage.GetData()}, Route: {string.Join("->", removeMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //consoleDebugger?.Invoke(debug);
                        VariableMapping(removeMessage.GetMessageRoute(), removeMessage.GetData());
                        break;

                    case MessageType.Enum:
                        debug += "Processing Enum message\n";
                        NetEnumMessage netEnumMessage = new NetEnumMessage(data);
                        debug += $"Enum: {netEnumMessage.GetData()}, Route: {string.Join("->", netEnumMessage.GetMessageRoute().Select(r => r.route))}\n";
                        //consoleDebugger?.Invoke(debug);
                        VariableMapping(netEnumMessage.GetMessageRoute(), netEnumMessage.GetData());
                        break;

                    case MessageType.Event:
                        debug += "Processing Event message\n";
                        NetEventMessage netEventMessage = new NetEventMessage(data);
                        (int, List<(string, string)>) eventData = netEventMessage.GetData();
                        debug += $"Method: {eventData.Item1}, Args: {string.Join(", ", eventData.Item2)}, Route: {netEventMessage.GetMessageRoute()[0].route}\n";
                        List<RouteInfo> route = netEventMessage.GetMessageRoute();
                        //consoleDebugger?.Invoke(debug);
                        InvokeCSharpEvent(eventData.Item1, eventData.Item2, route[0].route);
                        break;

                    case MessageType.TRS:
                        debug += "Processing TRS message\n";
                        NetTRSMessage netTRSMessage = new NetTRSMessage(data);
                        debug += $"Enum: {netTRSMessage.GetData()}, Route: {string.Join("->", netTRSMessage.GetMessageRoute().Select(r => r.route))}\n";
                        consoleDebugger?.Invoke(debug);
                        TRSMapping(netTRSMessage.GetMessageRoute(), netTRSMessage.GetData());
                        break;

                    default:
                        debug += $"Unhandled message type: {messageType}\n";
                        consoleDebugger?.Invoke(debug);
                        break;
                }
            }
            catch (Exception ex)
            {
                consoleDebugger?.Invoke($"ERROR Processing Message: {ex.Message}");
            }
        }

        #endregion

        #region Variable Mapping
        /// <summary>
        /// Maps received network values to their corresponding object fields.
        /// </summary>
        /// <param name="route">Route information for the value.</param>
        /// <param name="variableValue">The value to map.</param>
        void VariableMapping(List<RouteInfo> route, object variableValue)
        {
            string debug = $"VariableMapping - Start\n";
            debug += $"Type: {variableValue?.GetType()?.Name ?? "null"}, Value: {variableValue}\n";
            debug += $"Route: {string.Join("->", route.Select(r => r.route))}\n";

            try
            {
                if (route == null || route.Count == 0)
                {
                    //consoleDebugger?.Invoke("Empty route, aborting\n");
                    return;
                }

                INetObj objectRoot = NetObjFactory.GetINetObject(route[0].route);
                if (objectRoot == null)
                {
                    //consoleDebugger?.Invoke($"No INetObj found for ID: {route[0].route}\n");
                    return;
                }

                debug += $"Found root object: {objectRoot.GetType().Name} (OwnerID: {objectRoot.GetOwnerID()})\n";
                debug += $"NetworkEntity ClientID: {networkEntity.clientID}\n";

                if (objectRoot.GetOwnerID() != networkEntity.clientID)
                {
                    debug += "Proceeding with write operation\n";
                    //consoleDebugger?.Invoke(debug);
                    object result = InspectWrite(objectRoot.GetType(), objectRoot, route, 1, variableValue);
                    debug += $"InspectWrite completed. Result: {result}\n";
                }
                else
                {
                    debug += "Skipping write (owner matches)\n";
                }
            }
            catch (Exception ex)
            {
                debug += $"VariableMapping error: {ex.Message}\n{ex.StackTrace}";
            }

            //consoleDebugger?.Invoke(debug);
        }

        /// <summary>
        /// Handles mapping of null values received from the network.
        /// </summary>
        /// <param name="route">Route information for the value.</param>
        /// <param name="variableValue">The null value to map.</param>
        void VariableMappingNullException(List<RouteInfo> route, object variableValue)
        {
            string debug = "VariableMappingNullException - ";
            debug += $"Value: {variableValue}, Type: {variableValue?.GetType()?.Name ?? "null"}, ";
            debug += $"Route: {string.Join("->", route.Select(r => r.route))}\n";

            if (route == null || route.Count == 0)
            {
                //consoleDebugger?.Invoke("Empty route\n");
                return;
            }

            INetObj objectRoot = NetObjFactory.GetINetObject(route[0].route);
            if (objectRoot == null)
            {
                //consoleDebugger?.Invoke($"No INetObj found for ID: {route[0].route}\n");
                return;
            }

            debug += $"Root Object: {objectRoot.GetType().Name}, OwnerID: {objectRoot.GetOwnerID()}, NetworkEntity ClientID: {networkEntity.clientID}\n";

            if (objectRoot.GetOwnerID() != networkEntity.clientID)
            {
                debug += "Processing write operation for null exception\n";
                //consoleDebugger?.Invoke(debug);
                _ = InspectWriteNullException(objectRoot.GetType(), objectRoot, route, 1, variableValue);
            }

            if (objectRoot.GetOwnerID() != networkEntity.clientID)
            {
                debug += "Processing write operation for null exception\n";
                //consoleDebugger?.Invoke(debug);
                _ = InspectWriteNullException(objectRoot.GetType(), objectRoot, route, 1, variableValue);
            }
        }

        /// <summary>
        /// Handles mapping of empty values (collections) received from the network.
        /// </summary>
        /// <param name="route">Route information for the value.</param>
        /// <param name="variableValue">The empty value to map.</param>
        void VariableMappingEmpty(List<RouteInfo> route, object variableValue)
        {
            string debug = "VariableMappingEmpty - ";
            debug += $"Value: {variableValue}, Type: {variableValue?.GetType()?.Name ?? "null"}, ";
            debug += $"Route: {string.Join("->", route.Select(r => r.route))}\n";

            if (route == null || route.Count == 0)
            {
                //consoleDebugger?.Invoke("Empty route\n");
                return;
            }

            INetObj objectRoot = NetObjFactory.GetINetObject(route[0].route);
            if (objectRoot == null)
            {
                //consoleDebugger?.Invoke($"No INetObj found for ID: {route[0].route}\n");
                return;
            }

            debug += $"Root Object: {objectRoot.GetType().Name}, OwnerID: {objectRoot.GetOwnerID()}, NetworkEntity ClientID: {networkEntity.clientID}\n";

            if (objectRoot.GetOwnerID() != networkEntity.clientID)
            {
                debug += "Processing empty collection\n";
                _ = InspectWriteEmpty(objectRoot.GetType(), objectRoot, route, 1, variableValue);
            }
            //consoleDebugger?.Invoke(debug);
        }

        private void TRSMapping(List<RouteInfo> route, TRS data)
        {
            if (route == null || route.Count == 0)
            {
                //consoleDebugger?.Invoke("Empty route\n");
                return;
            }

            INetObj objectRoot = NetObjFactory.GetINetObject(route[0].route);
            if (objectRoot == null)
            {
                //consoleDebugger?.Invoke($"No INetObj found for ID: {route[0].route}\n");
                return;
            }

            if (objectRoot.GetOwnerID() != networkEntity.clientID)
            {
                NetTRS auxSync = objectRoot.GetType().GetCustomAttribute<NetTRS>();
                objectRoot.SetTRS(data, auxSync != null ? auxSync.syncData : NetTRS.SYNC.DEFAULT);
            }
        }
        #endregion

        #region Inspect Write
        /// <summary>
        /// Recursively writes a value to an object's field based on route information.
        /// </summary>
        /// <param name="type">The type of the current object.</param>
        /// <param name="obj">The current object instance.</param>
        /// <param name="idRoute">Route information for the value.</param>
        /// <param name="idToRead">Current position in the route.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>The modified object.</returns>
        public object InspectWrite(Type type, object obj, List<RouteInfo> idRoute, int idToRead, object value)
        {
            string debug = $"InspectWrite - Start\n";
            debug += $"Target Type: {type.Name}, Current Value: {obj}\n";
            debug += $"New Value: {value.ToString()} (Type: {value?.GetType()?.Name ?? "null"})\n";
            debug += $"Route Position: {idToRead}/{idRoute.Count}\n";
            debug += $"Full Route: {string.Join("->", idRoute.Select(r => r.route))}\n";

            try
            {
                if (obj == null)
                {
                    //consoleDebugger?.Invoke("Target object is null\n");
                    return null;
                }

                if (idRoute.Count <= idToRead)
                {
                    //consoleDebugger?.Invoke("Route exhausted without finding target\n");
                    return obj;
                }

                RouteInfo currentRoute = idRoute[idToRead];
                debug += $"Current Route Info: {currentRoute}\n";

                // Regular fields check
                foreach (FieldInfo info in GetAllFields(type))
                {
                    NetVariable attributes = info.GetCustomAttribute<NetVariable>();
                    if (attributes != null && attributes.VariableId == currentRoute.route)
                    {
                        //debug += $"Found matching field: {info.Name} (Type: {info.FieldType.Name})\n";
                        //debug += $"Current field value: {info.GetValue(obj)}\n";
                        //consoleDebugger?.Invoke(debug);

                        object result = WriteValue(info, obj, attributes, idRoute, idToRead, value);
                        //debug += $"WriteValue completed. New field value: {info.GetValue(result)}\n";
                        debug += $"Final field value after WriteValue: {info.GetValue(obj)}\n";
                        //consoleDebugger?.Invoke(debug);
                        return result;
                    }
                }

                // Extension fields check
                if (extensionMethods.TryGetValue(type, out MethodInfo methodInfo))
                {
                    debug += "Checking extension methods\n";
                    object unitializedObject = FormatterServices.GetUninitializedObject(type);
                    object fields = methodInfo.Invoke(null, new object[] { unitializedObject });

                    if (fields is List<(FieldInfo, NetVariable)> values)
                    {
                        foreach ((FieldInfo, NetVariable) field in values)
                        {
                            if (field.Item2.VariableId == currentRoute.route)
                            {
                                debug += $"Found extension field: {field.Item1.Name}\n";
                                //consoleDebugger?.Invoke(debug);
                                return WriteValue(field.Item1, obj, field.Item2, idRoute, idToRead, value);
                            }
                        }
                    }
                }

                debug += "No matching field found in this type\n";
            }
            catch (Exception ex)
            {
                debug += $"InspectWrite error: {ex.Message}\n{ex.StackTrace}";
            }

            //consoleDebugger?.Invoke(debug);
            return obj;
        }

        /// <summary>
        /// Handles writing null values to object fields based on route information.
        /// </summary>
        /// <param name="type">The type of the current object.</param>
        /// <param name="obj">The current object instance.</param>
        /// <param name="idRoute">Route information for the value.</param>
        /// <param name="idToRead">Current position in the route.</param>
        /// <param name="value">The null value to write.</param>
        /// <returns>The modified object.</returns>
        public object InspectWriteNullException(Type type, object obj, List<RouteInfo> idRoute, int idToRead, object value)
        {
            string debug = "InspectWriteNullException - ";
            debug += $"Type: {type.Name}, Current Route Index: {idToRead}, Value: {value}\n";
            debug += $"Full Route: {string.Join("->", idRoute.Select(r => r.route))}\n";

            if (obj == null || idRoute.Count <= idToRead)
            {
                //consoleDebugger?.Invoke($"Exit condition - obj null: {obj == null}, route count: {idRoute.Count}, idToRead: {idToRead}\n");
                return obj;
            }

            RouteInfo currentRoute = idRoute[idToRead];
            debug += $"Current Route Info: {currentRoute}\n";

            foreach (FieldInfo info in GetAllFields(type))
            {
                NetVariable attributes = info.GetCustomAttribute<NetVariable>();
                if (attributes != null && attributes.VariableId == currentRoute.route)
                {
                    debug += $"Found matching field: {info.Name}, VariableId: {attributes.VariableId}\n";
                    //consoleDebugger?.Invoke(debug);
                    return WriteValueNullException(info, obj, attributes, idRoute, idToRead, value);
                }
            }

            debug += "No matching field found\n";
            //consoleDebugger?.Invoke(debug);
            return obj;
        }

        /// <summary>
        /// Handles writing empty values (collections) to object fields based on route information.
        /// </summary>
        /// <param name="type">The type of the current object.</param>
        /// <param name="obj">The current object instance.</param>
        /// <param name="idRoute">Route information for the value.</param>
        /// <param name="idToRead">Current position in the route.</param>
        /// <param name="value">The empty value to write.</param>
        /// <returns>The modified object.</returns>
        public object InspectWriteEmpty(Type type, object obj, List<RouteInfo> idRoute, int idToRead, object value)
        {
            string debug = "InspectWriteEmpty - ";
            debug += $"Type: {type.Name}, Current Route Index: {idToRead}, Value: {value}\n";
            debug += $"Full Route: {string.Join("->", idRoute.Select(r => r.route))}\n";

            if (obj == null || idRoute.Count <= idToRead)
            {
                //consoleDebugger?.Invoke($"Exit condition - obj null: {obj == null}, route count: {idRoute.Count}, idToRead: {idToRead}\n");
                return obj;
            }

            RouteInfo currentRoute = idRoute[idToRead];
            debug += $"Current Route Info: {currentRoute}\n";

            foreach (FieldInfo info in GetAllFields(type))
            {
                NetVariable attributes = info.GetCustomAttribute<NetVariable>();
                if (attributes != null && attributes.VariableId == currentRoute.route)
                {
                    debug += $"Found matching field: {info.Name}, VariableId: {attributes.VariableId}\n";
                    //consoleDebugger?.Invoke(debug);
                    return WriteValueNullException(info, obj, attributes, idRoute, idToRead, value);
                }
            }

            debug += "No matching field found\n";
            //consoleDebugger?.Invoke(debug);
            return obj;
        }
        #endregion

        #region Write Value
        /// <summary>
        /// Writes a value to a specific field of an object.
        /// </summary>
        /// <param name="info">Field information.</param>
        /// <param name="obj">Parent object containing the field.</param>
        /// <param name="attribute">NetVariable attribute of the field.</param>
        /// <param name="idRoute">Route information for the value.</param>
        /// <param name="idToRead">Current position in the route.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>The modified object.</returns>
        object WriteValue(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int idToRead, object value)
        {
            //consoleDebugger?.Invoke($"WriteValue - Field: {info.Name}, ValueType: {value?.GetType().Name}");

            RouteInfo currentRoute = idRoute[idToRead];
            Type fieldType = info.FieldType;
            object currentValue = info.GetValue(obj);

            // Handle null assignments
            if (value == null || value is Null)
            {
                info.SetValue(obj, null);
                return obj;
            }

            // Handle simple types
            if (IsSimpleType(fieldType))
            {
                //consoleDebugger?.Invoke($"WriteValue: Setting field '{info.Name}' on '{obj.GetType().Name}' to '{value}' (Type: {value?.GetType()?.Name})");
                info.SetValue(obj, value);
                return obj;
            }

            if (value is Remove removeData)
            {
                object fieldValue = info.GetValue(obj);
                if (fieldValue == null)
                    return obj;

                if (typeof(IDictionary).IsAssignableFrom(info.FieldType))
                {
                    return HandleDictionaryRemove(info, obj, removeData.KeyHash);
                }
                else if (typeof(IList).IsAssignableFrom(info.FieldType))
                {
                    IList? list = fieldValue as IList;
                    if (list != null && currentRoute.collectionKey >= 0 && currentRoute.collectionKey < list.Count)
                    {
                        //consoleDebugger?.Invoke($"[WriteValue] Removing item at index {currentRoute.collectionKey} from '{info.Name}'");
                        list.RemoveAt(currentRoute.collectionKey);
                    }
                    else
                    {
                        //consoleDebugger?.Invoke($"[WriteValue] Cannot remove at index {currentRoute.collectionKey} — out of bounds or invalid list");
                    }
                    return obj;
                }
                else
                {
                    List<RouteInfo> newRoute = new List<RouteInfo>(idRoute);
                    newRoute.Add(RouteInfo.CreateForProperty(attribute.VariableId));
                    return InspectWrite(fieldValue.GetType(), fieldValue, newRoute, idToRead + 1, value);
                }
            }

            if (typeof(IEnumerable).IsAssignableFrom(fieldType))
            {
                //consoleDebugger?.Invoke($"[WriteValue] Attempting to assign collection element at index {currentRoute.collectionKey} with value: {value}");
                // Get current collection size
                int currentSize = (currentValue as ICollection)?.Count ?? 0;
                int newSize = currentRoute.collectionSize;

                object newCollection;
                if (fieldType.IsArray)
                {
                    if (fieldType.IsArray && fieldType.GetArrayRank() > 1)
                    {
                        Array newArray;
                        if (currentValue == null || ((Array)currentValue).Rank != fieldType.GetArrayRank())
                        {
                            newArray = Array.CreateInstance(
                                fieldType.GetElementType(),
                                currentRoute.Dimensions);
                        }
                        else
                        {
                            newArray = (Array)currentValue;
                        }

                        if (idRoute.Count <= idToRead + 1)
                        {
                            int[] indices = currentRoute.GetMultiDimensionalIndices();
                            newArray.SetValue(value, indices);
                        }
                        else
                        {
                            int[] indices = currentRoute.GetMultiDimensionalIndices();
                            object element = newArray.GetValue(indices);
                            if (element == null)
                            {
                                element = ConstructObject(fieldType.GetElementType());
                                newArray.SetValue(element, indices);
                            }
                            InspectWrite(element.GetType(), element, idRoute, idToRead + 1, value);
                        }

                        info.SetValue(obj, newArray);
                        return obj;
                    }
                    else
                    {
                        newCollection = Array.CreateInstance(fieldType.GetElementType(), newSize);

                        // Copy existing elements if available
                        if (currentValue != null)
                        {
                            Array.Copy((Array)currentValue, (Array)newCollection, Math.Min(currentSize, newSize));
                        }
                    }
                }
                else if (currentRoute.IsDictionary)
                {
                    return HandleDictionaryWrite(info, obj, attribute, idRoute, idToRead, value);
                }
                else
                {
                    if (currentValue == null)
                    {
                        //consoleDebugger?.Invoke($"[WriteValue] Field '{info.Name}' is null on receiver. Attempting to construct new instance of {fieldType.Name}.");
                        currentValue = ConstructObject(fieldType);
                        info.SetValue(obj, currentValue);
                    }

                    // Always check if we need to prefill it
                    if (currentRoute.IsCollection && (currentValue as ICollection)?.Count < currentRoute.collectionSize)
                    {
                        Type elementTypeToFill = GetElementType(fieldType) ?? typeof(object);
                        MethodInfo addMethod = fieldType.GetMethod("Add");

                        int currentCount = (currentValue as ICollection)?.Count ?? 0;
                        int fillCount = currentRoute.collectionSize - currentCount;

                        for (int i = 0; i < fillCount; i++)
                        {
                            object defaultValue = elementTypeToFill.IsValueType ? Activator.CreateInstance(elementTypeToFill) : null;

                            addMethod?.Invoke(currentValue, new object[] { defaultValue });
                        }

                        //consoleDebugger?.Invoke($"[WriteValue] Pre-filled {fieldType.Name} with {fillCount} additional default elements (now has {currentRoute.collectionSize})");
                    }

                    if (idRoute.Count <= idToRead + 1)
                    {
                        if (TrySetCollectionIndexValue(currentValue, currentRoute.collectionKey, value))
                        {
                            return obj;
                        }
                    }
                    else
                    {
                        object? nestedElement = TryGetCollectionIndexValue(currentValue, currentRoute.collectionKey);

                        if (nestedElement == null)
                        {
                            Type nestedElementType = GetElementType(fieldType) ?? typeof(object);
                            nestedElement = ConstructObject(nestedElementType);
                            TrySetCollectionIndexValue(currentValue, currentRoute.collectionKey, nestedElement);
                        }

                        InspectWrite(nestedElement.GetType(), nestedElement, idRoute, idToRead + 1, value);
                        return obj;
                    }

                    Type elementType = GetElementType(fieldType);
                    object[] arrayCopy = new object[newSize];

                    if (currentValue != null)
                    {
                        int i = 0;
                        foreach (object? item in (IEnumerable)currentValue)
                        {
                            if (i >= newSize) break;
                            arrayCopy[i++] = item;
                        }

                        for (; i < newSize; i++)
                        {
                            arrayCopy[i] = elementType.IsValueType ? Activator.CreateInstance(elementType) : null;
                        }
                    }

                    //consoleDebugger?.Invoke($"[WriteValue] Successfully constructed and assigned new {fieldType.Name} to field '{info.Name}'");

                    if (fieldType.IsGenericType)
                    {
                        Type genericType = fieldType.GetGenericTypeDefinition();
                        Type constructedType = genericType.MakeGenericType(elementType);
                        newCollection = Activator.CreateInstance(constructedType);

                        MethodInfo addMethod = constructedType.GetMethod("Add");
                        foreach (object? item in arrayCopy)
                        {
                            addMethod.Invoke(newCollection, new[] { item });
                        }
                    }
                    else
                    {
                        newCollection = arrayCopy;
                    }
                }

                if (currentRoute.collectionKey < 0)
                {
                    //consoleDebugger?.Invoke($"[WriteValue] Skipping write to invalid index {currentRoute.collectionKey} in collection '{info.Name}'");
                    return obj;
                }

                if (currentRoute.collectionKey >= 0 && currentRoute.collectionKey < newSize)
                {
                    if (idRoute.Count <= idToRead + 1)
                    {
                        if (fieldType.IsArray)
                        {
                            ((Array)newCollection).SetValue(value, currentRoute.collectionKey);
                        }
                        else if (newCollection is IList list)
                        {
                            list[currentRoute.collectionKey] = value;
                        }
                    }
                    else
                    {
                        if (currentRoute.collectionKey < 0)
                        {
                            //consoleDebugger?.Invoke($"[WriteValue] Skipping InspectWrite for invalid collectionKey {currentRoute.collectionKey} in '{info.Name}'");
                            return obj;
                        }

                        object element = fieldType.IsArray ? ((Array)newCollection).GetValue(currentRoute.collectionKey) : ((IList)newCollection)[currentRoute.collectionKey];

                        if (element == null)
                        {
                            element = ConstructObject(GetElementType(fieldType));
                            if (fieldType.IsArray)
                            {
                                ((Array)newCollection).SetValue(element, currentRoute.collectionKey);
                            }
                            else if (newCollection is IList list)
                            {
                                list[currentRoute.collectionKey] = element;
                            }
                        }

                        InspectWrite(element.GetType(), element, idRoute, idToRead + 1, value);
                    }
                }

                info.SetValue(obj, newCollection);
                return obj;
            }

            object objReference = info.GetValue(obj);
            if (objReference == null)
            {
                objReference = ConstructObject(info.FieldType);
            }
            else if (idRoute.Count > idToRead + 1)
            {
                InspectWrite(info.FieldType, objReference, idRoute, idToRead + 1, value);
            }

            info.SetValue(obj, objReference);
            return obj;
        }

        /// <summary>
        /// Attempts to get a value from a collection at a specific index.
        /// </summary>
        /// <param name="collection">The collection to read from.</param>
        /// <param name="index">The index to read.</param>
        /// <returns>The value at the specified index, or null if not found.</returns>
        private object? TryGetCollectionIndexValue(object collection, int index)
        {
            if (collection == null || index < 0)
                return null;

            Type type = collection.GetType();
            PropertyInfo indexer = type.GetProperty("Item", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (indexer != null && indexer.CanRead)
            {
                try
                {
                    return indexer.GetValue(collection, new object[] { index });
                }
                catch
                {
                    return null;
                }
            }

            if (collection is IList list && index < list.Count)
            {
                return list[index];
            }

            return null;
        }

        /// <summary>
        /// Attempts to set a value in a collection at a specific index.
        /// </summary>
        /// <param name="collection">The collection to modify.</param>
        /// <param name="index">The index to write to.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>True if the operation succeeded, false otherwise.</returns>
        private bool TrySetCollectionIndexValue(object collection, int index, object value)
        {
            if (collection == null || index < 0) return false;

            Type type = collection.GetType();

            // 1. Public or non-public indexer
            PropertyInfo indexer = type.GetProperty("Item", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (indexer != null && indexer.CanWrite)
            {
                try
                {
                    indexer.SetValue(collection, value, new object[] { index });
                    //consoleDebugger?.Invoke($"[TrySetCollectionIndexValue] Set via indexer [{index}] = {value} on {type.Name}");
                    return true;
                }
                catch (Exception ex)
                {
                    //consoleDebugger?.Invoke($"[TrySetCollectionIndexValue] Indexer set failed: {ex.Message}");
                }
            }

            // 2. IList
            if (collection is IList list && index < list.Count)
            {
                try
                {
                    list[index] = value;
                    //consoleDebugger?.Invoke($"[TrySetCollectionIndexValue] Set via IList at index {index} to {value}");
                    return true;
                }
                catch (Exception ex)
                {
                    //consoleDebugger?.Invoke($"[TrySetCollectionIndexValue] IList set failed: {ex.Message}");
                }
            }

            // 3. Insert(int, T)
            MethodInfo? insertMethod = type.GetMethod("Insert", new[] { typeof(int), typeof(object) });
            if (insertMethod != null)
            {
                try
                {
                    insertMethod.Invoke(collection, new object[] { index, value });
                    //consoleDebugger?.Invoke($"[TrySetCollectionIndexValue] Inserted value at index {index}");
                    return true;
                }
                catch (Exception ex)
                {
                    //consoleDebugger?.Invoke($"[TrySetCollectionIndexValue] Insert failed: {ex.Message}");
                }
            }

            //consoleDebugger?.Invoke($"[TrySetCollectionIndexValue] Failed to set index {index} on {type.Name}");
            return false;
        }

        /// <summary>
        /// Handles removal of dictionary entries based on key hash.
        /// </summary>
        /// <param name="info">Field information for the dictionary.</param>
        /// <param name="obj">Parent object containing the dictionary.</param>
        /// <param name="keyHash">Hash of the key to remove.</param>
        /// <returns>The modified object.</returns>
        private object HandleDictionaryRemove(FieldInfo info, object obj, int keyHash)
        {
            IDictionary dictionary = (IDictionary)info.GetValue(obj);
            if (dictionary == null)
            {
                //consoleDebugger?.Invoke("ERROR: Dictionary is null");
                return obj;
            }

            //consoleDebugger?.Invoke($"Target Dictionary: {info.Name} | Current Keys: {string.Join(",", dictionary.Keys.Cast<object>())}");
            //consoleDebugger?.Invoke($"Searching for key with hash: {keyHash}");

            bool found = false;
            foreach (object key in dictionary.Keys)
            {
                int currentHash = GetStableKeyHash(key);
                if (currentHash == keyHash)
                {
                    //consoleDebugger?.Invoke($"FOUND KEY: {key} (Hash: {currentHash}) - REMOVING");
                    dictionary.Remove(key);
                    found = true;

                    // Update previous state if tracking
                    if (previousDictionaryStates.TryGetValue(dictionary, out Dictionary<object, int>? state))
                    {
                        state.Remove(key);
                    }
                    break;
                }
            }

            //if (!found) consoleDebugger?.Invoke("WARNING: No matching key found");
            //consoleDebugger?.Invoke($"Final Keys: {string.Join(",", dictionary.Keys.Cast<object>())}");
            return obj;
        }

        /// <summary>
        /// Handles writing values to dictionary entries.
        /// </summary>
        /// <param name="info">Field information for the dictionary.</param>
        /// <param name="obj">Parent object containing the dictionary.</param>
        /// <param name="attribute">NetVariable attribute of the field.</param>
        /// <param name="idRoute">Route information for the value.</param>
        /// <param name="idToRead">Current position in the route.</param>
        /// <param name="value">The value to write.</param>
        /// <returns>The modified object.</returns>
        private object HandleDictionaryWrite(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int idToRead, object value)
        {
            string debug = "HandleDictionaryWrite - ";
            debug += $"Field: {info.Name}, Current Route Index: {idToRead}\n";

            RouteInfo currentRoute = idRoute[idToRead];
            debug += $"RouteInfo: {currentRoute}\n";
            Type fieldType = info.FieldType;
            Type[] genericArgs = fieldType.GetGenericArguments();
            Type valueType = genericArgs[1];
            IDictionary dictionary = (IDictionary)info.GetValue(obj);

            if (dictionary == null)
            {
                debug += "Creating new dictionary instance\n";
                dictionary = (IDictionary)Activator.CreateInstance(info.FieldType);
            }

            debug += $"Looking for key with hash: {currentRoute.collectionKey}\n";
            debug += $"Current dictionary keys: {string.Join(", ", dictionary.Keys.Cast<object>().Select(k => $"{k}(hash:{GetStableKeyHash(k)})"))}\n";

            object matchingKey = FindMatchingKey(dictionary, currentRoute.collectionKey);
            debug += matchingKey != null ? $"Found matching key: {matchingKey}\n" : "No matching key found!\n";

            if (idRoute.Count <= idToRead + 1)
            {
                debug += $"Directly setting value: {value}\n";
                if (matchingKey != null)
                {
                    dictionary[matchingKey] = value;
                }
                else
                {
                    dictionary[currentRoute.collectionKey] = value;
                }
            }
            else
            {
                debug += $"Nested inspection for value\n";
                object element = dictionary[matchingKey] ?? ConstructObject(valueType);
                dictionary[matchingKey] = element;
                InspectWrite(element.GetType(), element, idRoute, idToRead + 1, value);
            }

            info.SetValue(obj, dictionary);
            debug += $"Final dictionary state: {string.Join(", ", dictionary.Keys.Cast<object>().Select(k => $"{k}={dictionary[k]}"))}\n";

            //consoleDebugger?.Invoke(debug);
            return obj;
        }

        /// <summary>
        /// Handles writing null or empty values to fields.
        /// </summary>
        /// <param name="info">Field information.</param>
        /// <param name="obj">Parent object containing the field.</param>
        /// <param name="attribute">NetVariable attribute of the field.</param>
        /// <param name="idRoute">Route information for the value.</param>
        /// <param name="idToRead">Current position in the route.</param>
        /// <param name="value">The null/empty value to write.</param>
        /// <returns>The modified object.</returns>
        object WriteValueNullException(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int idToRead, object value)
        {
            //consoleDebugger?.Invoke($"WriteValueNullException - Field: {info.Name}, Type: {info.FieldType}, Value: {value}");

            RouteInfo currentRoute = idRoute[idToRead];
            Type fieldType = info.FieldType;

            // Handle simple types
            if (IsSimpleType(fieldType))
            {
                info.SetValue(obj, null);
                return obj;
            }

            bool isEmpty = value is Empty || (value?.ToString() == "Empty");

            if (isEmpty)
            {
                //consoleDebugger?.Invoke("Processing EMPTY state");

                object currentValue = info.GetValue(obj);
                if (currentValue is IEnumerable enumerable && currentValue != null)
                {
                    //consoleDebugger?.Invoke("Processing collection");
                    MethodInfo clearMethod = currentValue.GetType().GetMethod("Clear");
                    if (clearMethod != null)
                    {
                        //consoleDebugger?.Invoke("Invoking Clear()");
                        clearMethod.Invoke(currentValue, null);
                    }
                    else
                    {
                        //consoleDebugger?.Invoke("No Clear() found - creating new instance");
                        currentValue = FormatterServices.GetUninitializedObject(info.FieldType);
                        info.SetValue(obj, currentValue);
                    }
                }
                else if (currentValue == null)
                {
                    //consoleDebugger?.Invoke("Creating new empty instance");
                    currentValue = FormatterServices.GetUninitializedObject(info.FieldType);
                    info.SetValue(obj, currentValue);
                }

                foreach (FieldInfo field in info.FieldType.GetFields(bindingFlags))
                {
                    NetVariable fieldAttr = field.GetCustomAttribute<NetVariable>();
                    if (fieldAttr != null)
                    {
                        WriteValueNullException(field, currentValue, fieldAttr,
                            new List<RouteInfo>(idRoute) { RouteInfo.CreateForProperty(fieldAttr.VariableId) },
                            idToRead + 1, value);
                    }
                }
                return obj;
            }

            // Default null handling
            info.SetValue(obj, null);
            return obj;
        }
        #endregion

        #region Reflection Utilities
        /// <summary>
        /// Finds a dictionary key that matches the specified hash value.
        /// </summary>
        /// <param name="dictionary">The dictionary to search.</param>
        /// <param name="keyHash">The hash value to match.</param>
        /// <returns>The matching key, or null if not found.</returns>
        private object FindMatchingKey(IDictionary dictionary, int keyHash)
        {
            foreach (object key in dictionary.Keys)
            {
                if (GetStableKeyHash(key) == keyHash) return key;
            }
            return null;
        }

        /// <summary>
        /// Gets a stable hash code for a dictionary key.
        /// </summary>
        /// <param name="key">The key to hash.</param>
        /// <returns>A stable hash code for the key.</returns>
        private int GetStableKeyHash(object key)
        {
            // For types that can be reliably hashed
            if (key is string strKey) return strKey.GetHashCode();
            if (key is int intKey) return intKey;
            if (key is float floatKey) return floatKey.GetHashCode();
            if (key.GetType().IsEnum) return (int)key;

            // Fallback to standard hash code
            return key.GetHashCode();
        }

        /// <summary>
        /// Determines if a type is a simple type (value type, string, or enum).
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if the type is simple, false otherwise.</returns>
        private bool IsSimpleType(Type type)
        {
            return (type.IsValueType && type.IsPrimitive) || type == typeof(string) || type.IsEnum;
        }

        /// <summary>
        /// Processes a value and sends appropriate network messages.
        /// </summary>
        /// <param name="value">The value to process.</param>
        /// <param name="route">Route information for the value.</param>
        /// <param name="attribute">NetVariable attribute containing metadata.</param>
        private void ProcessValue(object value, List<RouteInfo> route, NetVariable attribute)
        {
            string debug = "ProcessValue - ";
            debug += $"Value: {value ?? "null"}, Type: {value?.GetType()?.Name ?? "null"}, ";
            debug += $"Route: {string.Join("->", route.Select(r => r.route))}\n";

            if (value == null)
            {
                debug += "Sending NULL package\n";
                //consoleDebugger?.Invoke(debug);
                SendPackage(PossibleStates.Null, attribute, route);
                return;
            }

            if (value is IDictionary dict && dict.Count == 0)
            {
                // Ensure we preserve dictionary type information
                RouteInfo lastRoute = route.Last();
                if (!lastRoute.IsDictionary && route.Count > 1)
                {
                    lastRoute = route[route.Count - 2];
                }

                if (lastRoute.IsDictionary)
                {
                    // Create new route info with proper dictionary flags
                    RouteInfo dictRoute = RouteInfo.CreateForDictionary(
                        lastRoute.route,
                        -1, // No specific key
                        lastRoute.ElementType ?? typeof(object));

                    route[route.Count - 1] = dictRoute;
                }

                SendPackage(PossibleStates.Empty, attribute, route);
                return;
            }

            Type valueType = value.GetType();

            if ((valueType.IsValueType && valueType.IsPrimitive) || valueType == typeof(string) || valueType.IsEnum)
            {
                debug += "Sending primitive/string/enum package\n";
                //consoleDebugger?.Invoke(debug);
                SendPackage(value, attribute, route);
            }
            else
            {
                debug += "Inspecting complex object\n";
                //consoleDebugger?.Invoke(debug);
                Inspect(valueType, value, route);
            }
        }

        /// <summary>
        /// Gets all indices for a multi-dimensional array.
        /// </summary>
        /// <param name="array">The array to process.</param>
        /// <returns>An enumerable of index arrays.</returns>
        private IEnumerable<int[]> GetArrayIndices(Array array)
        {
            int[] indices = new int[array.Rank];
            yield return indices; // Return first index (0,0,...)

            while (IncrementIndices(array, indices))
            {
                yield return (int[])indices.Clone();
            }
        }

        /// <summary>
        /// Increments multi-dimensional array indices.
        /// </summary>
        /// <param name="array">The array being processed.</param>
        /// <param name="indices">The current indices to increment.</param>
        /// <returns>True if indices were successfully incremented, false if at end of array.</returns>
        private bool IncrementIndices(Array array, int[] indices)
        {
            for (int dim = array.Rank - 1; dim >= 0; dim--)
            {
                indices[dim]++;
                if (indices[dim] < array.GetLength(dim))
                {
                    return true;
                }
                indices[dim] = 0;
            }
            return false;
        }

        /// <summary>
        /// Gets the element type of a collection or array.
        /// </summary>
        /// <param name="type">The collection/array type.</param>
        /// <returns>The element type, or typeof(object) if not determinable.</returns>
        private Type GetElementType(Type type)
        {
            if (type.IsArray)
                return type.GetElementType();
            else if (type.IsGenericType)
                return type.GetGenericArguments()[0];
            return typeof(object);
        }

        /// <summary>
        /// Constructs an instance of the specified type.
        /// </summary>
        /// <param name="type">The type to instantiate.</param>
        /// <returns>A new instance of the type.</returns>
        private object ConstructObject(Type type)
        {
            ConstructorInfo[] constructors = type.GetConstructors(bindingFlags);
            ConstructorInfo constructorInfo = null;

            foreach (ConstructorInfo constructor in constructors)
            {
                if (constructor.GetParameters().Length == 0)
                {
                    return constructor.Invoke(new object[0]);
                }
                else
                {
                    foreach (ParameterInfo parametersInfo in constructor.GetParameters())
                    {
                        if (parametersInfo.ParameterType == type)
                        {
                            continue;
                        }
                    }
                    constructorInfo = constructor;
                }
            }

            ParameterInfo[] parameterInfos = constructorInfo.GetParameters();
            object[] parameters = new object[parameterInfos.Length];

            for (int i = 0; i < parameterInfos.Length; i++)
            {
                if (parameterInfos[i].ParameterType.IsValueType || parameterInfos[i].ParameterType == typeof(string) || parameterInfos[i].ParameterType.IsEnum)
                {
                    parameters[i] = default;
                }
                else
                {
                    parameters[i] = ConstructObject(parameterInfos[i].ParameterType);
                }
            }
            return constructorInfo.Invoke(parameters);
        }

        /// <summary>
        /// Gets all fields of a type, including inherited fields.
        /// </summary>
        /// <param name="type">The type to inspect.</param>
        /// <returns>An enumerable of FieldInfo objects.</returns>
        private IEnumerable<FieldInfo> GetAllFields(Type type)
        {
            while (type != null)
            {
                foreach (FieldInfo? field in type.GetFields(bindingFlags))
                    yield return field;

                type = type.BaseType;
            }
        }
        #endregion

        #region Method Invocation
        // When I get the message invoke this
        /// <summary>
        /// Invokes a method on a network object when a method message is received.
        /// </summary>
        /// <param name="id">The method ID from the NetMethod attribute.</param>
        /// <param name="parameters">List of parameter type-value pairs.</param>
        /// <param name="objectId">The ID of the target network object.</param>
        private void InvokeReflectionMethod(int id, List<(string, string)> parameters, int objectId)
        {
            foreach (INetObj netObj in NetObjFactory.NetObjects())
            {
                if (netObj.GetOwnerID() != networkEntity.clientID && netObj.GetID() == objectId)
                {
                    List<object> parametersToApply = new List<object>();

                    foreach ((string, string) param in parameters)
                    {
                        TypeConverter converter = TypeDescriptor.GetConverter(Type.GetType(param.Item1));
                        object parameterValue = converter.ConvertFromInvariantString(param.Item2);
                        parametersToApply.Add(parameterValue);
                    }
                    int length = netObj.GetType().GetMethods(bindingFlags).Length;

                    for (int i = 0; i < length; i++)
                    {
                        MethodInfo method = netObj.GetType().GetMethods(bindingFlags)[i];

                        NetMethod netMethod = method.GetCustomAttribute<NetMethod>();

                        if (netMethod != null && netMethod.MethodId == id)
                        {
                            object[] objectParameters = parametersToApply.ToArray();
                            object invokeMethod = method.Invoke(netObj, objectParameters);
                        }
                    }
                }
            }
        }

        // Invoke this in the game
        /// <summary>
        /// Sends a method invocation message over the network.
        /// </summary>
        /// <param name="iNetObj">The network object containing the method.</param>
        /// <param name="methodName">The name of the method to invoke.</param>
        /// <param name="parameters">The parameters to pass to the method.</param>
        /// <returns>The method's return value, or null for void methods.</returns>
        public object SendMethodMessage(INetObj iNetObj, string methodName, params object[] parameters)
        {
            string debug = "";
            debug += "Started SendMethodMessage";
            //consoleDebugger.Invoke(debug);
            object objectToReturn = null;

            if (iNetObj.GetOwnerID() != networkEntity.clientID)
                return objectToReturn;

            MethodInfo method = iNetObj.GetType().GetMethod(methodName, bindingFlags);

            NetMethod netmethod = method.GetCustomAttribute<NetMethod>();
            debug += "NetMethod is " + netmethod;
            //consoleDebugger.Invoke(debug);
            if (netmethod != null)
            {
                object invokeMethod = method.Invoke(iNetObj, parameters);

                if (method.ReturnParameter.GetType() != typeof(void))
                {
                    objectToReturn = invokeMethod;
                }

                List<(string, string)> parametersList = new List<(string, string)>();

                foreach (object parameter in parameters)
                {
                    (string, string) param;
                    param.Item1 = parameter.GetType().ToString();
                    param.Item2 = parameter.ToString();

                    parametersList.Add(param);
                }

                (int, List<(string, string)>) messageData;

                messageData.Item1 = netmethod.MethodId;
                messageData.Item2 = parametersList;
                List<RouteInfo> idRoute = new List<RouteInfo>
                {
                    new RouteInfo(iNetObj.GetID())
                };

                foreach ((string, string) item in messageData.Item2)
                {
                    debug += "Parameter List: " + item;
                }
                //consoleDebugger.Invoke(debug);

                NetMethodMessage messageToSend = new NetMethodMessage(MessagePriority.Default, messageData, idRoute);
                networkEntity.SendMessage(messageToSend.Serialize());
            }
            return objectToReturn;
        }
        #endregion

        #region Event Invocation
        /// <summary>
        /// Sends a C# event invocation message from the owner of a network object to all other clients.
        /// </summary>
        /// <param name="iNetObj">The network object that owns the event.</param>
        /// <param name="eventName">The name of the event to invoke (e.g. "OnEventX").</param>
        /// <param name="parameters">Optional parameters to serialize and send to listeners.</param>
        public void SendCSharpEventMessage(INetObj iNetObj, string eventName, params object[] parameters)
        {
            if (iNetObj.GetOwnerID() != networkEntity.clientID)
            {
                consoleDebugger?.Invoke("[SendCSharpEventMessage] Skipped: not owner.");
                return;
            }

            EventInfo eventInfo = iNetObj.GetType().GetEvent(eventName, bindingFlags);
            if (eventInfo == null)
            {
                consoleDebugger?.Invoke($"[SendCSharpEventMessage] Could not find event '{eventName}' on {iNetObj.GetType().Name}");
                return;
            }

            NetEvent netEvent = eventInfo.GetCustomAttribute<NetEvent>();
            if (netEvent == null)
            {
                consoleDebugger?.Invoke($"[SendCSharpEventMessage] Missing [NetEvent] on event '{eventName}'");
                return;
            }

            string backingFieldName = netEvent.BackingFieldName ?? $"on{eventInfo.Name.Substring(2)}";

            FieldInfo field = iNetObj.GetType().GetField(backingFieldName, bindingFlags);

            if (field == null)
            {
                consoleDebugger?.Invoke($"[SendCSharpEventMessage] Could not find backing field '{backingFieldName}'");
                return;
            }

            consoleDebugger?.Invoke($"[SendCSharpEventMessage] Found backing field '{backingFieldName}', sending event...");

            // Serialize parameters
            List<(string, string)> parametersList = new List<(string, string)>();
            foreach (object param in parameters)
            {
                parametersList.Add((param.GetType().ToString(), param.ToString()));
            }

            (int EventId, List<(string, string)> parametersList) messageData = (netEvent.EventId, parametersList);
            List<RouteInfo> idRoute = new List<RouteInfo>() { new RouteInfo(iNetObj.GetID()) };

            NetEventMessage messageToSend = new NetEventMessage(netEvent.MessagePriority, messageData, idRoute);
            networkEntity.SendMessage(messageToSend.Serialize());
        }

        /// <summary>
        /// Invokes a previously declared C# event on a target network object by matching its event ID.
        /// </summary>
        /// <param name="eventId">The unique event ID specified in the NetEvent attribute.</param>
        /// <param name="parameters">Serialized parameter values sent from the sender.</param>
        /// <param name="objectId">The target network object ID whose event should be invoked.</param>
        public void InvokeCSharpEvent(int eventId, List<(string, string)> parameters, int objectId)
        {
            foreach (INetObj netObj in NetObjFactory.NetObjects())
            {
                if (netObj.GetID() != objectId || netObj.GetOwnerID() == networkEntity.clientID)
                    continue;

                consoleDebugger?.Invoke($"[InvokeCSharpEvent] Matching object ID {objectId}");

                Type targetType = netObj.GetType();
                EventInfo[] events = targetType.GetEvents(bindingFlags);

                foreach (EventInfo evt in events)
                {
                    NetEvent netEventAttr = evt.GetCustomAttribute<NetEvent>();
                    consoleDebugger?.Invoke($"[InvokeCSharpEvent] Checking event {evt.Name}...");

                    if (netEventAttr != null)
                    {
                        consoleDebugger?.Invoke($"[InvokeCSharpEvent] Found NetEvent with ID {netEventAttr.EventId}");
                    }

                    if (netEventAttr != null && netEventAttr.EventId == eventId)
                    {
                        string backingFieldName = netEventAttr.BackingFieldName ?? $"on{evt.Name.Substring(2)}";

                        FieldInfo field = targetType.GetField(backingFieldName, bindingFlags);

                        if (field == null)
                        {
                            consoleDebugger?.Invoke($"[InvokeCSharpEvent] Could not find backing field '{backingFieldName}'");
                            return;
                        }

                        Delegate eventDelegate = field.GetValue(netObj) as Delegate;
                        if (eventDelegate == null)
                        {
                            consoleDebugger?.Invoke($"[InvokeCSharpEvent] No subscribers for event '{evt.Name}'");
                            return;
                        }

                        List<object> parsedParameters = new List<object>();
                        ParameterInfo[] paramInfos = evt.EventHandlerType?.GetMethod("Invoke")?.GetParameters() ?? Array.Empty<ParameterInfo>();

                        for (int i = 0; i < parameters.Count; i++)
                        {
                            string typeStr = parameters[i].Item1;
                            string valueStr = parameters[i].Item2;

                            Type type = Type.GetType(typeStr);
                            TypeConverter converter = TypeDescriptor.GetConverter(type);
                            object param = converter.ConvertFromInvariantString(valueStr);
                            parsedParameters.Add(param);
                        }

                        foreach (Delegate handler in eventDelegate.GetInvocationList())
                        {
                            consoleDebugger?.Invoke($"[InvokeCSharpEvent] Invoking {evt.Name}...");
                            handler.DynamicInvoke(parsedParameters.ToArray());
                        }

                        return;
                    }
                }

                consoleDebugger?.Invoke($"[InvokeCSharpEvent] No matching event with ID {eventId} found.");
            }
        }
        #endregion
    }

    #region Attributes
    /// <summary>
    /// Attribute for marking classes that handle specific message types.
    /// </summary>
    public class NetMessageClass : Attribute
    {
        Type type;
        MessageType messageType;

        /// <summary>
        /// Initializes a new instance of the NetMessageClass attribute.
        /// </summary>
        /// <param name="type">The message type.</param>
        /// <param name="messageType">The network message type enum value.</param>
        public NetMessageClass(Type type, MessageType messageType)
        {
            this.type = type;
            this.messageType = messageType;
        }

        /// <summary>
        /// Gets the network message type.
        /// </summary>
        public MessageType MessageType
        {
            get { return messageType; }
        }

        /// <summary>
        /// Gets the message class type.
        /// </summary>
        public Type Type
        {
            get { return type; }
        }
    }

    /// <summary>
    /// Attribute for marking fields that should be synchronized over the network.
    /// </summary>
    public class NetVariable : Attribute
    {
        int variableId;
        MessagePriority messagePriority;

        /// <summary>
        /// Initializes a new instance of the NetVariable attribute.
        /// </summary>
        /// <param name="id">The unique identifier for this variable.</param>
        /// <param name="messagePriority">The priority for network messages.</param>
        public NetVariable(int id, MessagePriority messagePriority = MessagePriority.Default)
        {
            variableId = id;
            this.messagePriority = messagePriority;
        }

        /// <summary>
        /// Gets the message priority for this variable.
        /// </summary>
        public MessagePriority MessagePriority
        {
            get { return messagePriority; }
        }

        /// <summary>
        /// Gets the unique identifier for this variable.
        /// </summary>
        public int VariableId
        {
            get { return variableId; }
        }
    }

    /// <summary>
    /// Attribute for marking methods that should be invokable over the network.
    /// </summary>
    public class NetMethod : Attribute
    {
        int methodId;
        MessagePriority messagePriority;

        /// <summary>
        /// Initializes a new instance of the NetMethod attribute.
        /// </summary>
        /// <param name="id">The unique identifier for this method.</param>
        /// <param name="messagePriority">The priority for network messages.</param>
        public NetMethod(int id, MessagePriority messagePriority = MessagePriority.Default)
        {
            methodId = id;
            this.messagePriority = messagePriority;
        }

        /// <summary>
        /// Gets the message priority for this method.
        /// </summary>
        public MessagePriority MessagePriority
        {
            get { return messagePriority; }
        }

        /// <summary>
        /// Gets the unique identifier for this method.
        /// </summary>
        public int MethodId
        {
            get { return methodId; }
        }
    }

    /// <summary>
    /// Attribute for marking classes that contain extension methods for network reflection.
    /// </summary>
    public class NetExtensionClass : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the NetExtensionClass attribute.
        /// </summary>
        public NetExtensionClass()
        {

        }
    }

    /// <summary>
    /// Attribute for marking extension methods used in network reflection.
    /// </summary>
    public class NetExtensionMethod : Attribute
    {
        /// <summary>
        /// Gets the type that this extension method extends.
        /// </summary>
        public Type extensionMethod;

        /// <summary>
        /// Initializes a new instance of the NetExtensionMethod attribute.
        /// </summary>
        /// <param name="type">The type that this method extends.</param>
        public NetExtensionMethod(Type type)
        {
            extensionMethod = type;
        }
    }

    /// <summary>
    /// Marks a C# event as network-synchronized. When invoked by the owner of the network object,
    /// it will be triggered on all other clients that have subscribed to it.
    /// </summary>
    public class NetEvent : Attribute
    {
        public int EventId { get; }
        public MessagePriority MessagePriority { get; }
        public string? BackingFieldName { get; }

        /// <summary>
        /// Creates a new NetEvent attribute instance.
        /// </summary>
        /// <param name="eventId">Unique identifier for the event.</param>
        /// <param name="priority">The priority of the network message.</param>
        /// <param name="backingFieldName">
        /// Optional backing field name (e.g. "onEventX"). If omitted, the system falls back to a convention.
        /// </param>
        public NetEvent(int eventId, MessagePriority priority = MessagePriority.Default, string? backingFieldName = null)
        {
            EventId = eventId;
            MessagePriority = priority;
            BackingFieldName = backingFieldName;
        }
    }

    public class NetTRS : Attribute
    {
        [Flags]
        public enum SYNC
        {
            DEFAULT = 0,
            NOTPOSITION = 1,
            NOTROTATION = 2,
            NOTSCALE = 4,
            NOTTRS = 7,
            NOTISACTIVE = 8,
            NOTALL = 15
        }

        public SYNC syncData = SYNC.DEFAULT;

        public NetTRS(SYNC value)
        {
            syncData = value;
        }
    }

    #endregion
}