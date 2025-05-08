using Network_Lib.BasicMessages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Net
{
    public enum NullOrEmpty
    {
        Null,
        Empty,
        Remove
    }

    public class Reflection
    {
        BindingFlags bindingFlags;
        Assembly executeAssembly;
        Assembly gameAssembly;

        public static Action<string> consoleDebugger;
        public static Action consoleDebuggerPause;

        public Dictionary<Type, MethodInfo> extensionMethods = new Dictionary<Type, MethodInfo>();
        private Dictionary<object, Dictionary<object, int>> _previousDictionaryStates = new Dictionary<object, Dictionary<object, int>>();
        NetworkEntity networkEntity;

        public Reflection(NetworkEntity entity)
        {
            networkEntity = entity;
            networkEntity.OnReceivedMessage += OnReceivedReflectionMessage;

            executeAssembly = Assembly.GetExecutingAssembly();

            gameAssembly = Assembly.GetCallingAssembly();

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
                            consoleDebugger?.Invoke($"Registered extension for: {netExtensionMethod.extensionMethod.Name}");
                        }
                    }
                }
            }

            bindingFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        }

        public void UpdateReflection()
        {
            if (NetObjFactory.NetObjectsCount <= 0)
            {
                return;
            }

            foreach (INetObj netObj in NetObjFactory.NetObjects)
            {
                if (netObj.GetOwnerID() == networkEntity.clientID)
                {
                    List<RouteInfo> idRoute = new List<RouteInfo>
                    {
                        RouteInfo.CreateForProperty(netObj.GetID())
                    };
                    Inspect(netObj.GetType(), netObj, idRoute);
                }
            }
        }

        public void Inspect(Type type, object obj, List<RouteInfo> idRoute)
        {
            string debug = "";
            if (obj != null)
            {
                foreach (FieldInfo info in type.GetFields(bindingFlags))
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

        public void ReadValue(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute)
        {
            string debug = "ReadValue Start - ";
            debug += $"Field: {info.Name}, Type: {info.FieldType}, Current Route: {string.Join("->", idRoute.Select(r => r.route))}\n";

            object fieldValue = info.GetValue(obj);
            Type fieldType = info.FieldType;

            // Handle null case
            if (fieldValue == null)
            {
                debug += "Field is NULL\n";
                idRoute.Add(RouteInfo.CreateForProperty(attribute.VariableId));
                SendPackage(NullOrEmpty.Null, attribute, idRoute);
                return;
            }

            debug += $"Field Value: {fieldValue}\n";
            consoleDebugger?.Invoke(debug);

            // Handle simple types
            if ((fieldType.IsValueType && fieldType.IsPrimitive) || fieldType == typeof(string) || fieldType.IsEnum)
            {
                debug += "Handling primitive/string/enum type\n";
                idRoute.Add(RouteInfo.CreateForProperty(attribute.VariableId));
                SendPackage(fieldValue, attribute, idRoute);
                return;
            }

            consoleDebugger?.Invoke("typeof(IEnumerable).IsAssignableFrom(fieldType): " + typeof(IEnumerable).IsAssignableFrom(fieldType));

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
                    if (_previousDictionaryStates.TryGetValue(dictionary, out Dictionary<object, int>? previousKeys))
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
                                consoleDebugger?.Invoke($"Sending Remove - Full Data: {BitConverter.ToString(serialized)}");
                                networkEntity.SendMessage(serialized);
                            }

                            // Update state and RETURN after processing removals
                            _previousDictionaryStates[dictionary] = currentKeys.ToDictionary(k => k, GetStableKeyHash);
                            debug += ("--- REMOVALS PROCESSED ---");
                            consoleDebugger?.Invoke(debug.ToString());
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
                        SendPackage(NullOrEmpty.Empty, attribute, new List<RouteInfo>(idRoute)
                        {
                            RouteInfo.CreateForDictionary(attribute.VariableId, -1, valueType)
                        });
                    }

                    // Update state
                    _previousDictionaryStates[dictionary] = currentKeys.ToDictionary(k => k, GetStableKeyHash);

                    debug += ("--- INSPECTION COMPLETE ---");
                    consoleDebugger?.Invoke(debug);
                    return;
                }
                else
                {
                    consoleDebugger?.Invoke($"Processing as generic collection: {fieldType.Name}");

                    IEnumerable collection = (IEnumerable)fieldValue;
                    int count = 0;
                    int index = 0;

                    // Get count via enumeration (works for any IEnumerable)
                    IEnumerator enumerator = collection.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        count++;
                    }

                    // Process items
                    enumerator = collection.GetEnumerator(); // Reset enumerator
                    while (enumerator.MoveNext())
                    {
                        object? item = enumerator.Current;
                        List<RouteInfo> currentRoute = new List<RouteInfo>(idRoute)
                        {
                            new RouteInfo(
                                route: attribute.VariableId,
                                collectionKey: index++,
                                collectionSize: count,
                                elementType: item?.GetType() ?? GetElementType(fieldType))
                        };

                        consoleDebugger?.Invoke($"Processing collection item [{index - 1}]: " +
                                              $"Type: {item?.GetType()?.Name ?? "null"}, " +
                                              $"Value: {item ?? "null"}");

                        ProcessValue(item, currentRoute, attribute);
                    }

                    if (count == 0)
                    {
                        consoleDebugger?.Invoke("Collection is empty");
                        idRoute.Add(new RouteInfo(
                            attribute.VariableId,
                            collectionKey: -1,
                            collectionSize: 0,
                            elementType: GetElementType(fieldType)));
                        SendPackage(NullOrEmpty.Empty, attribute, idRoute);
                    }
                }
            }

            // Handle complex objects
            debug += "Handling complex object type\n";
            idRoute.Add(RouteInfo.CreateForProperty(attribute.VariableId));
            Inspect(fieldType, fieldValue, idRoute);
        }

        private bool IsSupportedCollection(Type type)
        {
            if (type == null || type.IsArray || typeof(string) == type)
                return false;

            // Check for any IEnumerable that's not a dictionary
            return typeof(IEnumerable).IsAssignableFrom(type) &&
                   !typeof(IDictionary).IsAssignableFrom(type);
        }

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

        public void SendPackage(object value, NetVariable attribute, List<RouteInfo> idRoute)
        {
            string debug = "SendPackage - ";
            debug += $"Value Type: {value?.GetType().Name ?? "null"}";
            debug += $"Value: {value}";
            debug += $"Route: {string.Join("->", idRoute.Select(r => $"{r.route}[{r.collectionKey}]"))}";

            if (value is NullOrEmpty.Null)
            {
                debug += "Sending NullMessage\n";
                //consoleDebugger?.Invoke(debug);
                NetNullMessage netNullMessage = new NetNullMessage(attribute.MessagePriority, null, idRoute);
                networkEntity.SendMessage(netNullMessage.Serialize());
                return;
            }

            if (value is NullOrEmpty.Empty)
            {
                debug += "Sending EmptyMessage\n";
                //consoleDebugger?.Invoke(debug);
                NetEmptyMessage netEmptyMessage = new NetEmptyMessage(attribute.MessagePriority, new Empty(), idRoute);
                networkEntity.SendMessage(netEmptyMessage.Serialize());
                return;
            }

            if (value is NullOrEmpty.Remove)
            {
                debug += "Sending RemoveMessage\n";
                consoleDebugger?.Invoke(debug);
                int keyHash = idRoute.Last().collectionKey;
                NetRemoveMessage netRemoveMessage = new NetRemoveMessage(attribute.MessagePriority, keyHash, idRoute);
                byte[] serialized = netRemoveMessage.Serialize();
                //consoleDebugger?.Invoke($"Sending Remove - KeyHash: {keyHash}, Data: {BitConverter.ToString(serialized)}");
                networkEntity.SendMessage(serialized);
                return;
            }

            if (value is Enum enumValue)
            {
                debug += "Sending Enum package\n";
                //consoleDebugger?.Invoke(debug);
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

                    default:
                        debug += $"Unhandled message type: {messageType}\n";
                        //consoleDebugger?.Invoke(debug);
                        break;
                }
            }
            catch (Exception ex)
            {
                consoleDebugger?.Invoke($"ERROR Processing Message: {ex.Message}");
            }
        }

        void VariableMapping(List<RouteInfo> route, object variableValue)
        {
            string debug = $"VariableMapping - Start\n";
            debug += $"Type: {variableValue?.GetType()?.Name ?? "null"}, Value: {variableValue}\n";
            debug += $"Route: {string.Join("->", route.Select(r => r.route))}\n";
            debug += $"Is Remove: {variableValue is NullOrEmpty.Remove}\n";

            try
            {
                if (route == null || route.Count == 0)
                {
                    debug += "Empty route, aborting\n";
                    //consoleDebugger?.Invoke(debug);
                    return;
                }

                INetObj objectRoot = NetObjFactory.GetINetObject(route[0].route);
                if (objectRoot == null)
                {
                    debug += $"No INetObj found for ID: {route[0].route}\n";
                    //consoleDebugger?.Invoke(debug);
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

        void VariableMappingNullException(List<RouteInfo> route, object variableValue)
        {
            string debug = "VariableMappingNullException - ";
            debug += $"Value: {variableValue}, Type: {variableValue?.GetType()?.Name ?? "null"}, ";
            debug += $"Route: {string.Join("->", route.Select(r => r.route))}\n";

            if (route == null || route.Count == 0)
            {
                debug += "Empty route\n";
                //consoleDebugger?.Invoke(debug);
                return;
            }

            INetObj objectRoot = NetObjFactory.GetINetObject(route[0].route);
            if (objectRoot == null)
            {
                debug += $"No INetObj found for ID: {route[0].route}\n";
                //consoleDebugger?.Invoke(debug);
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

        void VariableMappingEmpty(List<RouteInfo> route, object variableValue)
        {
            string debug = "VariableMappingEmpty - ";
            debug += $"Value: {variableValue}, Type: {variableValue?.GetType()?.Name ?? "null"}, ";
            debug += $"Route: {string.Join("->", route.Select(r => r.route))}\n";

            if (route == null || route.Count == 0)
            {
                debug += "Empty route\n";
                //consoleDebugger?.Invoke(debug);
                return;
            }

            INetObj objectRoot = NetObjFactory.GetINetObject(route[0].route);
            if (objectRoot == null)
            {
                debug += $"No INetObj found for ID: {route[0].route}\n";
                //consoleDebugger?.Invoke(debug);
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
                    debug += "Target object is null\n";
                    //consoleDebugger?.Invoke(debug);
                    return null;
                }

                if (idRoute.Count <= idToRead)
                {
                    debug += "Route exhausted without finding target\n";
                    //consoleDebugger?.Invoke(debug);
                    return obj;
                }

                RouteInfo currentRoute = idRoute[idToRead];
                debug += $"Current Route Info: {currentRoute}\n";

                // Regular fields check
                foreach (FieldInfo info in type.GetFields(bindingFlags))
                {
                    NetVariable attributes = info.GetCustomAttribute<NetVariable>();
                    if (attributes != null && attributes.VariableId == currentRoute.route)
                    {
                        debug += $"Found matching field: {info.Name} (Type: {info.FieldType.Name})\n";
                        debug += $"Current field value: {info.GetValue(obj)}\n";
                        //consoleDebugger?.Invoke(debug);

                        object result = WriteValue(info, obj, attributes, idRoute, idToRead, value);
                        debug += $"WriteValue completed. New field value: {info.GetValue(result)}\n";
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

        public object InspectWriteNullException(Type type, object obj, List<RouteInfo> idRoute, int idToRead, object value)
        {
            string debug = "InspectWriteNullException - ";
            debug += $"Type: {type.Name}, Current Route Index: {idToRead}, Value: {value}\n";
            debug += $"Full Route: {string.Join("->", idRoute.Select(r => r.route))}\n";

            if (obj == null || idRoute.Count <= idToRead)
            {
                debug += $"Exit condition - obj null: {obj == null}, route count: {idRoute.Count}, idToRead: {idToRead}\n";
                //consoleDebugger?.Invoke(debug);
                return obj;
            }

            RouteInfo currentRoute = idRoute[idToRead];
            debug += $"Current Route Info: {currentRoute}\n";

            foreach (FieldInfo info in type.GetFields(bindingFlags))
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

        public object InspectWriteEmpty(Type type, object obj, List<RouteInfo> idRoute, int idToRead, object value)
        {
            string debug = "InspectWriteEmpty - ";
            debug += $"Type: {type.Name}, Current Route Index: {idToRead}, Value: {value}\n";
            debug += $"Full Route: {string.Join("->", idRoute.Select(r => r.route))}\n";

            if (obj == null || idRoute.Count <= idToRead)
            {
                debug += $"Exit condition - obj null: {obj == null}, route count: {idRoute.Count}, idToRead: {idToRead}\n";
                //consoleDebugger?.Invoke(debug);
                return obj;
            }

            RouteInfo currentRoute = idRoute[idToRead];
            debug += $"Current Route Info: {currentRoute}\n";

            foreach (FieldInfo info in type.GetFields(bindingFlags))
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

        object WriteValue(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int idToRead, object value)
        {
            //consoleDebugger?.Invoke($"WriteValue - Field: {info.Name}, ValueType: {value?.GetType().Name}");
            //consoleDebugger?.Invoke($"Value is Remove: {value is Remove}");

            RouteInfo currentRoute = idRoute[idToRead];
            Type fieldType = info.FieldType;
            object currentValue = info.GetValue(obj);

            // Handle null assignments
            if (value == null || value is NullOrEmpty.Null)
            {
                info.SetValue(obj, null);
                return obj;
            }

            // Handle simple types
            if (IsSimpleType(fieldType))
            {
                info.SetValue(obj, value);
                return obj;
            }

            if (value is Remove removeData)
            {
                //consoleDebugger?.Invoke("\n--- DICTIONARY REMOVE OPERATION ---\n");

                // Get the field value
                object fieldValue = info.GetValue(obj);
                if (fieldValue == null)
                {
                    //consoleDebugger?.Invoke("ERROR: Field value is null");
                    return obj;
                }

                // Check if this is the dictionary field or a container
                if (typeof(IDictionary).IsAssignableFrom(info.FieldType))
                {
                    // Direct dictionary case
                    return HandleDictionaryRemove(info, obj, removeData.KeyHash);
                }
                else
                {
                    // Nested case - need to continue inspection
                    //consoleDebugger?.Invoke($"Field {info.Name} is a container, inspecting deeper...");

                    // Create new route for the nested path
                    List<RouteInfo> newRoute = new List<RouteInfo>(idRoute);
                    newRoute.Add(RouteInfo.CreateForProperty(attribute.VariableId));

                    // Continue inspection with the nested object
                    return InspectWrite(fieldValue.GetType(), fieldValue, newRoute, idToRead + 1, value);
                }
            }

            // Handle collections - Using OLD code approach
            if (typeof(IEnumerable).IsAssignableFrom(fieldType))
            {
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
                    // For non-array collections, use OLD code's approach
                    Type elementType = GetElementType(fieldType);
                    object[] arrayCopy = new object[newSize];

                    if (currentValue != null)
                    {
                        // Copy existing elements
                        int i = 0;
                        foreach (object? item in (IEnumerable)currentValue)
                        {
                            if (i >= newSize) break;
                            arrayCopy[i++] = item;
                        }

                        // Fill remaining slots with default instances if needed
                        for (; i < newSize; i++)
                        {
                            arrayCopy[i] = elementType.IsValueType ? Activator.CreateInstance(elementType) : null;
                        }
                    }

                    // Convert to proper collection type
                    if (fieldType.IsGenericType)
                    {
                        Type genericType = fieldType.GetGenericTypeDefinition();
                        Type constructedType = genericType.MakeGenericType(elementType);
                        newCollection = Activator.CreateInstance(constructedType);

                        // Add elements
                        MethodInfo addMethod = constructedType.GetMethod("Add");
                        foreach (object? item in arrayCopy)
                        {
                            addMethod.Invoke(newCollection, new[] { item });
                        }
                    }
                    else
                    {
                        // Fallback for non-generic collections
                        newCollection = arrayCopy;
                    }
                }

                // Handle specific index operations
                if (currentRoute.collectionKey >= 0 && currentRoute.collectionKey < newSize)
                {
                    if (idRoute.Count <= idToRead + 1) // Direct value assignment
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
                    else // Nested object inspection
                    {
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

            // Handle complex objects
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
                    if (_previousDictionaryStates.TryGetValue(dictionary, out var state))
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
            debug += matchingKey != null
                ? $"Found matching key: {matchingKey}\n"
                : "No matching key found!\n";

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

            consoleDebugger?.Invoke(debug);
            return obj;
        }

        private object FindMatchingKey(IDictionary dictionary, int keyHash)
        {
            foreach (object key in dictionary.Keys)
            {
                if (GetStableKeyHash(key) == keyHash)
                    return key;
            }
            return null;
        }

        private bool IsSimpleType(Type type)
        {
            return (type.IsValueType && type.IsPrimitive) || type == typeof(string) || type.IsEnum;
        }

        object WriteValueNullException(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int idToRead, object value)
        {
            //consoleDebugger?.Invoke($"WriteValueNullException - Field: {info.Name}, Type: {info.FieldType}, Value: {value}");

            RouteInfo currentRoute = idRoute[idToRead];
            Type fieldType = info.FieldType;

            // Handle simple types
            if ((fieldType.IsValueType && fieldType.IsPrimitive) || fieldType == typeof(string) || fieldType.IsEnum)
            {
                info.SetValue(obj, null);
                return obj;
            }

            bool isEmpty = value is NullOrEmpty.Empty || (value?.ToString() == "Empty");

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

        private void ProcessValue(object value, List<RouteInfo> route, NetVariable attribute)
        {
            string debug = "ProcessValue - ";
            debug += $"Value: {value ?? "null"}, Type: {value?.GetType()?.Name ?? "null"}, ";
            debug += $"Route: {string.Join("->", route.Select(r => r.route))}\n";

            if (value == null)
            {
                debug += "Sending NULL package\n";
                //consoleDebugger?.Invoke(debug);
                SendPackage(NullOrEmpty.Null, attribute, route);
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

                SendPackage(NullOrEmpty.Empty, attribute, route);
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

        private int GetCollectionCount(IEnumerable collection)
        {
            if (collection is ICollection coll) return coll.Count;

            // Fallback for non-ICollection enumerables
            int count = 0;
            foreach (object? item in collection) count++;
            return count;
        }

        private IEnumerable<int[]> GetArrayIndices(Array array)
        {
            int[] indices = new int[array.Rank];
            yield return indices; // Return first index (0,0,...)

            while (IncrementIndices(array, indices))
            {
                yield return (int[])indices.Clone();
            }
        }

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

        private Type GetElementType(Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }
            else if (type.IsGenericType)
            {
                return type.GetGenericArguments()[0];
            }

            return typeof(object);
        }

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

        // When I get the message invoke this
        private void InvokeReflectionMethod(int id, List<(string, string)> parameters, int objectId)
        {
            foreach (INetObj netObj in NetObjFactory.NetObjects)
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
    }

    public class NetMessageClass : Attribute
    {
        Type type;
        MessageType messageType;

        public NetMessageClass(Type type, MessageType messageType)
        {
            this.type = type;
            this.messageType = messageType;
        }
        public MessageType MessageType
        {
            get { return messageType; }
        }

        public Type Type
        {
            get { return type; }
        }
    }

    public class NetVariable : Attribute
    {
        int variableId;
        MessagePriority messagePriority;

        public NetVariable(int id, MessagePriority messagePriority = MessagePriority.Default)
        {
            variableId = id;
            this.messagePriority = messagePriority;
        }

        public MessagePriority MessagePriority
        {
            get { return messagePriority; }
        }

        public int VariableId
        {
            get { return variableId; }
        }
    }

    public class NetMethod : Attribute
    {
        int methodId;
        MessagePriority messagePriority;

        public NetMethod(int id, MessagePriority messagePriority = MessagePriority.Default)
        {
            methodId = id;
            this.messagePriority = messagePriority;
        }

        public MessagePriority MessagePriority
        {
            get { return messagePriority; }
        }

        public int MethodId
        {
            get { return methodId; }
        }
    }

    public class NetExtensionClass : Attribute
    {
        public NetExtensionClass()
        {

        }
    }

    public class NetExtensionMethod : Attribute
    {
        public Type extensionMethod;

        public NetExtensionMethod(Type type)
        {
            extensionMethod = type;
        }
    }
}