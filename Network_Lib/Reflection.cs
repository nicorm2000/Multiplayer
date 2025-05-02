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

namespace Net
{
    public enum NullOrEmpty
    {
        Null,
        Empty
    }

    public class Reflection
    {
        BindingFlags bindingFlags;
        Assembly executeAssembly;
        Assembly gameAssembly;

        public static Action<string> consoleDebugger;
        public static Action consoleDebuggerPause;

        public Dictionary<Type, MethodInfo> extensionMethods = new Dictionary<Type, MethodInfo>();

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
                            foreach (var field in values)
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
                //consoleDebugger?.Invoke(debug);
                SendPackage(NullOrEmpty.Null, attribute, idRoute);
                return;
            }

            debug += $"Field Value: {fieldValue}\n";

            // Handle simple types
            if ((fieldType.IsValueType && fieldType.IsPrimitive) || fieldType == typeof(string) || fieldType.IsEnum)
            {
                debug += "Handling primitive/string/enum type\n";
                idRoute.Add(RouteInfo.CreateForProperty(attribute.VariableId));
                //consoleDebugger?.Invoke(debug);
                SendPackage(fieldValue, attribute, idRoute);
                return;
            }

            // Handle collections
            if (typeof(IEnumerable).IsAssignableFrom(fieldType))
            {
                if (fieldType.IsArray && fieldType.GetArrayRank() > 1)
                {
                    // Handle multi-dimensional array
                    Array mdArray = (Array)fieldValue;
                    int[] dimensions = new int[mdArray.Rank];
                    for (int i = 0; i < mdArray.Rank; i++)
                    {
                        dimensions[i] = mdArray.GetLength(i);
                    }

                    // Send each element with proper indices
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
                else
                {
                    IEnumerable collection = (IEnumerable)fieldValue;
                    int count = GetCollectionCount(collection);

                    // Always send current size with collection elements
                    int index = 0;
                    foreach (var item in collection)
                    {
                        List<RouteInfo> currentRoute = new List<RouteInfo>(idRoute)
                    {
                        new RouteInfo(
                            route: attribute.VariableId,
                            collectionKey: index++,
                            collectionSize: count,
                            elementType: item?.GetType() ?? GetElementType(fieldType))
                    };

                        ProcessValue(item, currentRoute, attribute);
                    }

                    if (count == 0)
                    {
                        idRoute.Add(new RouteInfo(
                            attribute.VariableId,
                            collectionKey: -1,
                            collectionSize: 0,
                            elementType: GetElementType(fieldType)));

                        SendPackage(NullOrEmpty.Empty, attribute, idRoute);
                    }
                    return;
                }
            }

            // Handle complex objects
            debug += "Handling complex object type\n";
            idRoute.Add(RouteInfo.CreateForProperty(attribute.VariableId));
            //consoleDebugger?.Invoke(debug);
            Inspect(fieldType, fieldValue, idRoute);
        }

        public void SendPackage(object value, NetVariable attribute, List<RouteInfo> idRoute)
        {
            string debug = "SendPackage - ";
            debug += $"Value: {value ?? "null"}, Type: {value?.GetType()?.Name ?? "null"}, ";
            debug += $"Route: {string.Join("->", idRoute.Select(r => r.route))}\n";

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

            if (value is Enum enumValue)
            {
                debug += "Sending Enum package\n";
                consoleDebugger?.Invoke(debug);
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
                                    var message = (ParentBaseMessage)ctor.Invoke(parameters);
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
            string debug = "OnReceivedReflectionMessage - ";
            MessageType messageType = MessageChecker.CheckMessageType(data);
            debug += $"Message Type: {messageType}\n";

            switch (MessageChecker.CheckMessageType(data))
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

                case MessageType.Enum:
                    debug += "Processing Enum message\n";
                    NetEnumMessage netEnumMessage = new NetEnumMessage(data);
                    Enum enumValue = netEnumMessage.GetData();
                    debug += $"Enum: {enumValue.GetType().Name}.{enumValue}, Route: {string.Join("->", netEnumMessage.GetMessageRoute().Select(r => r.route))}\n";
                    VariableMapping(netEnumMessage.GetMessageRoute(), enumValue);
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
                    var methodData = netMethodMessage.GetData();
                    debug += $"Method: {methodData.Item1}, Args: {string.Join(", ", methodData.Item2)}, Route: {netMethodMessage.GetMessageRoute()[0].route}\n";
                    //consoleDebugger?.Invoke(debug);
                    InvokeReflectionMethod(methodData.Item1, methodData.Item2, netMethodMessage.GetMessageRoute()[0].route);
                    break;

                default:
                    debug += $"Unhandled message type: {messageType}\n";
                    //consoleDebugger?.Invoke(debug);
                    break;
            }
        }

        void VariableMapping(List<RouteInfo> route, object variableValue)
        {
            string debug = $"VariableMapping - Start\n";
            debug += $"Type: {variableValue?.GetType()?.Name ?? "null"}, Value: {variableValue}\n";
            debug += $"Route: {string.Join("->", route.Select(r => r.route))}\n";

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
                debug += "Processing write operation for empty value\n";
                //consoleDebugger?.Invoke(debug);
                _ = InspectWriteEmpty(objectRoot.GetType(), objectRoot, route, 1, variableValue);
            }
        }

        public object InspectWrite(Type type, object obj, List<RouteInfo> idRoute, int idToRead, object value)
        {
            string debug = $"InspectWrite - Start\n";
            debug += $"Target Type: {type.Name}, Current Value: {obj}\n";
            debug += $"New Value: {value} (Type: {value?.GetType()?.Name ?? "null"})\n";
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
                        foreach (var item in arrayCopy)
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

        private bool IsSimpleType(Type type)
        {
            return (type.IsValueType && type.IsPrimitive) || type == typeof(string) || type.IsEnum;
        }

        object WriteValueNullException(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int idToRead, object value)
        {
            RouteInfo currentRoute = idRoute[idToRead];
            Type fieldType = info.FieldType;

            if ((fieldType.IsValueType && fieldType.IsPrimitive) || fieldType == typeof(string) || fieldType.IsEnum) // Simple cases
            {
                info.SetValue(obj, null);
                return obj;
            }

            if (typeof(IEnumerable).IsAssignableFrom(fieldType))
            {
                return HandleCollectionNullException(info, obj, attribute, idRoute, idToRead, value);
            }

            info.SetValue(obj, null);
            return obj;
        }

        private object HandleCollectionNullException(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int idToRead, object value)
        {
            RouteInfo currentRoute = idRoute[idToRead];
            Type fieldType = info.FieldType;

            if (currentRoute.IsDictionary)
            {
                return HandleDictionaryNullException(info, obj, attribute, idRoute, idToRead, value);
            }

            if (currentRoute.IsMultiDimensionalArray)
            {
                return HandleMultiDimensionalArrayNullException(info, obj, attribute, idRoute, idToRead, value);
            }

            if (currentRoute.IsJaggedArray)
            {
                return HandleJaggedArrayNullException(info, obj, attribute, idRoute, idToRead, value);
            }

            return HandleRegularCollectionNullException(info, obj, attribute, idRoute, idToRead, value);
        }

        private object HandleDictionaryNullException(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int idToRead, object value)
        {
            RouteInfo currentRoute = idRoute[idToRead]; // Create empty dictionary

            Type dictType = typeof(Dictionary<,>).MakeGenericType(typeof(object), currentRoute.ElementType ?? typeof(object)); // Create empty dictionary correct type
            IDictionary emptyDict = (IDictionary)Activator.CreateInstance(dictType);

            info.SetValue(obj, emptyDict);
            return obj;
        }

        private object HandleMultiDimensionalArrayNullException(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int idToRead, object value)
        {
            RouteInfo currentRoute = idRoute[idToRead];

            Array emptyArray = Array.CreateInstance(currentRoute.ElementType ?? typeof(object), currentRoute.Dimensions.Length > 0 ? currentRoute.Dimensions : new[] { 0 }); // Correct dimensions empty array

            info.SetValue(obj, emptyArray);
            return obj;
        }

        private object HandleJaggedArrayNullException(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int idToRead, object value)
        {
            RouteInfo currentRoute = idRoute[idToRead];

            Array emptyJaggedArray = Array.CreateInstance(currentRoute.ElementType?.MakeArrayType() ?? typeof(object).MakeArrayType(), currentRoute.Dimensions.Length > 0 ? currentRoute.Dimensions[0] : 0); // Empty jagged array

            info.SetValue(obj, emptyJaggedArray);
            return obj;
        }

        private object HandleRegularCollectionNullException(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int idToRead, object value)
        {
            RouteInfo currentRoute = idRoute[idToRead];
            Type fieldType = info.FieldType;

            object emptyCollection;

            if (fieldType.IsArray)
            {
                emptyCollection = Array.CreateInstance(currentRoute.ElementType ?? typeof(object), 0); // Empty array
            }
            else if (fieldType.IsGenericType)
            {
                Type collectionType = fieldType.GetGenericTypeDefinition().MakeGenericType(currentRoute.ElementType ?? typeof(object)); // Generic empty collection
                emptyCollection = Activator.CreateInstance(collectionType);
            }
            else
            {
                Type listType = typeof(List<>).MakeGenericType(currentRoute.ElementType ?? typeof(object)); // Empty list by default
                emptyCollection = Activator.CreateInstance(listType);
            }

            info.SetValue(obj, emptyCollection);
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
            foreach (var item in collection) count++;
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

                foreach (var parameter in parameters)
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

                foreach (var item in messageData.Item2)
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