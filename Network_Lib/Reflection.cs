using Network_Lib.BasicMessages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
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
            if (typeof(IEnumerable).IsAssignableFrom(fieldType) && !typeof(string).IsAssignableFrom(fieldType))
            {
                debug += "Handling collection type\n";
                HandleCollectionRead(info, obj, attribute, idRoute, fieldValue, fieldType, debug);
                return;
            }

            // Handle complex objects
            debug += "Handling complex object type\n";
            idRoute.Add(RouteInfo.CreateForProperty(attribute.VariableId));
            //consoleDebugger?.Invoke(debug);
            Inspect(fieldType, fieldValue, idRoute);
        }

        private void HandleCollectionRead(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, object fieldValue, Type fieldType, string debug)
        {
            IEnumerable collection = (IEnumerable)fieldValue;
            int count = GetCollectionCount(collection);
            Type elementType = GetElementType(fieldType);
            debug += $"Collection Count: {count}, Element Type: {elementType?.Name ?? "null"}\n";

            if (count == 0)
            {
                debug += "Empty collection\n";
                idRoute.Add(RouteInfo.CreateForCollection(attribute.VariableId, index: 0, size: 0, elementType));

                //consoleDebugger?.Invoke(debug);
                SendPackage(fieldValue == null ? NullOrEmpty.Null : NullOrEmpty.Empty, attribute, idRoute);
                return;
            }

            debug += "Processing collection items:\n";
            int index = 0;
            foreach (object? item in collection)
            {
                List<RouteInfo> currentRoute = new List<RouteInfo>(idRoute)
                {
                    RouteInfo.CreateForCollection(attribute.VariableId, index++, count, item?.GetType() ?? elementType)};
                    debug += $"[{index - 1}] Value: {item ?? "null"}, Type: {item?.GetType()?.Name ?? "null"}\n";
                    //consoleDebugger?.Invoke(debug);
                    ProcessValue(item, currentRoute, attribute);
                    debug = ""; // Reset debug for next item
                }
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
                    consoleDebugger?.Invoke(debug);
                    VariableMapping(netULongMessage.GetMessageRoute(), netULongMessage.GetData());
                    break;

                case MessageType.Uint:
                    debug += "Processing Uint message\n";
                    NetUIntMessage netUIntMessage = new NetUIntMessage(data);
                    debug += $"Data: {netUIntMessage.GetData()}, Route: {string.Join("->", netUIntMessage.GetMessageRoute().Select(r => r.route))}\n";
                    consoleDebugger?.Invoke(debug);
                    VariableMapping(netUIntMessage.GetMessageRoute(), netUIntMessage.GetData());
                    break;

                case MessageType.Ushort:
                    debug += "Processing Ushort message\n";
                    NetUShortMessage netUShortMessage = new NetUShortMessage(data);
                    debug += $"Data: {netUShortMessage.GetData()}, Route: {string.Join("->", netUShortMessage.GetMessageRoute().Select(r => r.route))}\n";
                    consoleDebugger?.Invoke(debug);
                    VariableMapping(netUShortMessage.GetMessageRoute(), netUShortMessage.GetData());
                    break;

                case MessageType.String:
                    debug += "Processing String message\n";
                    NetStringMessage netStringMessage = new NetStringMessage(data);
                    debug += $"Data: {netStringMessage.GetData()}, Route: {string.Join("->", netStringMessage.GetMessageRoute().Select(r => r.route))}\n";
                    consoleDebugger?.Invoke(debug);
                    VariableMapping(netStringMessage.GetMessageRoute(), netStringMessage.GetData());
                    break;

                case MessageType.Short:
                    debug += "Processing Short message\n";
                    NetShortMessage netShortMessage = new NetShortMessage(data);
                    debug += $"Data: {netShortMessage.GetData()}, Route: {string.Join("->", netShortMessage.GetMessageRoute().Select(r => r.route))}\n";
                    consoleDebugger?.Invoke(debug);
                    VariableMapping(netShortMessage.GetMessageRoute(), netShortMessage.GetData());
                    break;

                case MessageType.Sbyte:
                    debug += "Processing Sbyte message\n";
                    NetSByteMessage netSByteMessage = new NetSByteMessage(data);
                    debug += $"Data: {netSByteMessage.GetData()}, Route: {string.Join("->", netSByteMessage.GetMessageRoute().Select(r => r.route))}\n";
                    consoleDebugger?.Invoke(debug);
                    VariableMapping(netSByteMessage.GetMessageRoute(), netSByteMessage.GetData());
                    break;

                case MessageType.Long:
                    debug += "Processing Long message\n";
                    NetLongMessage netLongMessage = new NetLongMessage(data);
                    debug += $"Data: {netLongMessage.GetData()}, Route: {string.Join("->", netLongMessage.GetMessageRoute().Select(r => r.route))}\n";
                    consoleDebugger?.Invoke(debug);
                    VariableMapping(netLongMessage.GetMessageRoute(), netLongMessage.GetData());
                    break;

                case MessageType.Int:
                    debug += "Processing Int message\n";
                    NetIntMessage netIntMessage = new NetIntMessage(data);
                    debug += $"Data: {netIntMessage.GetData()}, Route: {string.Join("->", netIntMessage.GetMessageRoute().Select(r => r.route))}\n";
                    consoleDebugger?.Invoke(debug);
                    VariableMapping(netIntMessage.GetMessageRoute(), netIntMessage.GetData());
                    break;

                case MessageType.Float:
                    debug += "Processing Float message\n";
                    NetFloatMessage netFloatMessage = new NetFloatMessage(data);
                    debug += $"Data: {netFloatMessage.GetData()}, Route: {string.Join("->", netFloatMessage.GetMessageRoute().Select(r => r.route))}\n";
                    consoleDebugger?.Invoke(debug);
                    VariableMapping(netFloatMessage.GetMessageRoute(), netFloatMessage.GetData());
                    break;

                case MessageType.Double:
                    debug += "Processing Double message\n";
                    NetDoubleMessage netDoubleMessage = new NetDoubleMessage(data);
                    debug += $"Data: {netDoubleMessage.GetData()}, Route: {string.Join("->", netDoubleMessage.GetMessageRoute().Select(r => r.route))}\n";
                    consoleDebugger?.Invoke(debug);
                    VariableMapping(netDoubleMessage.GetMessageRoute(), netDoubleMessage.GetData());
                    break;

                case MessageType.Decimal:
                    debug += "Processing Decimal message\n";
                    NetDecimalMessage netDecimalMessage = new NetDecimalMessage(data);
                    debug += $"Data: {netDecimalMessage.GetData()}, Route: {string.Join("->", netDecimalMessage.GetMessageRoute().Select(r => r.route))}\n";
                    consoleDebugger?.Invoke(debug);
                    VariableMapping(netDecimalMessage.GetMessageRoute(), netDecimalMessage.GetData());
                    break;

                case MessageType.Char:
                    debug += "Processing Char message\n";
                    NetCharMessage netCharMessage = new NetCharMessage(data);
                    debug += $"Data: {netCharMessage.GetData()}, Route: {string.Join("->", netCharMessage.GetMessageRoute().Select(r => r.route))}\n";
                    consoleDebugger?.Invoke(debug);
                    VariableMapping(netCharMessage.GetMessageRoute(), netCharMessage.GetData());
                    break;

                case MessageType.Byte:
                    debug += "Processing Byte message\n";
                    NetByteMessage netByteMessage = new NetByteMessage(data);
                    debug += $"Data: {netByteMessage.GetData()}, Route: {string.Join("->", netByteMessage.GetMessageRoute().Select(r => r.route))}\n";
                    consoleDebugger?.Invoke(debug);
                    VariableMapping(netByteMessage.GetMessageRoute(), netByteMessage.GetData());
                    break;

                case MessageType.Bool:
                    debug += "Processing Bool message\n";
                    NetBoolMessage netBoolMessage = new NetBoolMessage(data);
                    debug += $"Data: {netBoolMessage.GetData()}, Route: {string.Join("->", netBoolMessage.GetMessageRoute().Select(r => r.route))}\n";
                    consoleDebugger?.Invoke(debug);
                    VariableMapping(netBoolMessage.GetMessageRoute(), netBoolMessage.GetData());
                    break;

                case MessageType.Null:
                    debug += "Processing Null message\n";
                    NetNullMessage netNullMessage = new NetNullMessage(data);
                    debug += $"Data: {netNullMessage.GetData()}, Route: {string.Join("->", netNullMessage.GetMessageRoute().Select(r => r.route))}\n";
                    consoleDebugger?.Invoke(debug);
                    VariableMappingNullException(netNullMessage.GetMessageRoute(), netNullMessage.GetData());
                    break;

                case MessageType.Empty:
                    debug += "Processing Empty message\n";
                    NetEmptyMessage netEmptyMessage = new NetEmptyMessage(data);
                    debug += $"Data: {netEmptyMessage.GetData()}, Route: {string.Join("->", netEmptyMessage.GetMessageRoute().Select(r => r.route))}\n";
                    consoleDebugger?.Invoke(debug);
                    VariableMappingEmpty(netEmptyMessage.GetMessageRoute(), netEmptyMessage.GetData());
                    break;

                case MessageType.Method:
                    debug += "Processing Method message\n";
                    NetMethodMessage netMethodMessage = new NetMethodMessage(data);
                    var methodData = netMethodMessage.GetData();
                    debug += $"Method: {methodData.Item1}, Args: {string.Join(", ", methodData.Item2)}, Route: {netMethodMessage.GetMessageRoute()[0].route}\n";
                    consoleDebugger?.Invoke(debug);
                    InvokeReflectionMethod(methodData.Item1, methodData.Item2, netMethodMessage.GetMessageRoute()[0].route);
                    break;

                default:
                    debug += $"Unhandled message type: {messageType}\n";
                    consoleDebugger?.Invoke(debug);
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
                    var result = InspectWrite(objectRoot.GetType(), objectRoot, route, 1, variableValue);
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

                        var result = WriteValue(info, obj, attributes, idRoute, idToRead, value);
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
                        foreach (var field in values)
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
                consoleDebugger?.Invoke(debug);
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
            string debug = $"WriteValue - Start\n";
            debug += $"Field: {info.Name}, DeclaringType: {info.DeclaringType?.Name}, FieldType: {info.FieldType.Name}\n";
            debug += $"Current Value: {info.GetValue(obj)}, New Value: {value}\n";
            debug += $"Value Type: {value?.GetType()?.Name ?? "null"}\n";

            try
            {
                RouteInfo currentRoute = idRoute[idToRead];
                Type fieldType = info.FieldType;
                object currentValue = info.GetValue(obj);

                // Null assignment
                if (value == null)
                {
                    debug += "Performing null assignment\n";
                    info.SetValue(obj, null);
                    //consoleDebugger?.Invoke(debug);
                    return obj;
                }

                // Simple type handling
                if (IsSimpleType(fieldType))
                {
                    debug += "Handling simple type\n";
                    try
                    {
                        object convertedValue = Convert.ChangeType(value, fieldType);
                        info.SetValue(obj, convertedValue);
                        debug += $"Successfully set to: {convertedValue}\n";
                        consoleDebugger?.Invoke(debug);
                        return obj;
                    }
                    catch (Exception ex)
                    {
                        debug += $"Conversion failed: {ex.Message}\n";
                        //consoleDebugger?.Invoke(debug);
                        return obj;
                    }
                }

                // Collection handling
                if (typeof(IEnumerable).IsAssignableFrom(fieldType))
                {
                    debug += "Handling collection type\n";
                    //consoleDebugger?.Invoke(debug);
                    return HandleCollectionWrite(info, obj, currentValue, idRoute, idToRead, value);
                }

                // Complex object handling
                debug += "Handling complex object\n";
                if (currentValue == null)
                {
                    debug += "Creating new instance\n";
                    currentValue = CreateInstance(fieldType);
                    info.SetValue(obj, currentValue);
                }

                if (idRoute.Count > idToRead + 1)
                {
                    debug += "Recursing into object\n";
                    //consoleDebugger?.Invoke(debug);
                    InspectWrite(fieldType, currentValue, idRoute, idToRead + 1, value);
                }
            }
            catch (Exception ex)
            {
                debug += $"WriteValue error: {ex.Message}\n{ex.StackTrace}";
            }

            //consoleDebugger?.Invoke(debug);
            return obj;
        }

        private bool IsSimpleType(Type type)
        {
            return (type.IsValueType && type.IsPrimitive) || type == typeof(string) || type.IsEnum || type == typeof(decimal);
        }

        private object CreateInstance(Type type)
        {
            try
            {
                return Activator.CreateInstance(type) ?? FormatterServices.GetUninitializedObject(type);
            }
            catch
            {
                return FormatterServices.GetUninitializedObject(type);
            }
        }

        private object HandleCollectionWrite(FieldInfo info, object obj, object currentCollection, List<RouteInfo> idRoute, int idToRead, object value)
        {
            string debug = "HandleCollectionWrite - ";
            RouteInfo currentRoute = idRoute[idToRead];
            Type collectionType = info.FieldType;

            // 1. Handle empty/null collections
            if (currentCollection == null)
            {
                debug += "Creating new collection instance\n";
                currentCollection = CreateCollectionInstance(collectionType, currentRoute.collectionSize);
                info.SetValue(obj, currentCollection);
            }

            // 2. Handle array resize if needed
            if (collectionType.IsArray && currentCollection is Array array)
            {
                if (currentRoute.collectionSize > array.Length)
                {
                    debug += "Resizing array\n";
                    Array newArray = Array.CreateInstance(collectionType.GetElementType(), currentRoute.collectionSize);
                    Array.Copy(array, newArray, Math.Min(array.Length, newArray.Length));
                    currentCollection = newArray;
                    info.SetValue(obj, currentCollection);
                }
            }

            // 3. Process collection elements
            if (currentRoute.collectionKey >= 0) // Has specific index/key
            {
                debug += $"Processing collection index: {currentRoute.collectionKey}\n";

                if (collectionType.IsArray)
                {
                    HandleArrayWrite((Array)currentCollection, currentRoute.collectionKey, value);
                }
                else if (TryGetListAccessors(collectionType, out var accessors))
                {
                    accessors.SetItem(currentCollection, currentRoute.collectionKey, value);
                }
                else
                {
                    debug += "Using fallback enumeration method\n";
                    HandleEnumerableWrite(currentCollection, currentRoute.collectionKey, value);
                }
            }

            //consoleDebugger?.Invoke(debug);
            return obj;
        }

        private void HandleArrayWrite(Array array, int index, object value)
        {
            if (index >= 0 && index < array.Length)
            {
                Type elementType = array.GetType().GetElementType();
                try
                {
                    object convertedValue = Convert.ChangeType(value, elementType);
                    array.SetValue(convertedValue, index);
                }
                catch { /* Handle conversion error */ }
            }
        }

        private bool TryGetListAccessors(Type collectionType, out ListAccessors accessors)
        {
            accessors = default;

            // Check for IList<T>
            Type listInterface = collectionType.GetInterface(typeof(IList<>).Name);
            if (listInterface != null)
            {
                Type elementType = listInterface.GetGenericArguments()[0];
                accessors = new ListAccessors(
                    (list, index, value) =>
                    {
                        MethodInfo method = listInterface.GetMethod("set_Item");
                        method.Invoke(list, new[] { index, Convert.ChangeType(value, elementType) });
                    },
                    (list, index) => listInterface.GetMethod("get_Item").Invoke(list, new object[] { index })
                );
                return true;
            }

            // Check for non-generic IList
            if (typeof(IList).IsAssignableFrom(collectionType))
            {
                accessors = new ListAccessors(
                    (list, index, value) => ((IList)list)[index] = value,
                    (list, index) => ((IList)list)[index]
                );
                return true;
            }

            return false;
        }

        private void HandleEnumerableWrite(object enumerable, int index, object value)
        {
            // Fallback for non-indexed collections
            int currentIndex = 0;
            foreach (var item in (IEnumerable)enumerable)
            {
                if (currentIndex == index)
                {
                    // Try to find a way to set the value
                    // This is tricky for non-list enumerables
                    break;
                }
                currentIndex++;
            }
        }

        private object CreateCollectionInstance(Type collectionType, int size)
        {
            if (collectionType.IsArray)
            {
                return Array.CreateInstance(collectionType.GetElementType(), Math.Max(size, 0));
            }

            try
            {
                return Activator.CreateInstance(collectionType) ??
                       FormatterServices.GetUninitializedObject(collectionType);
            }
            catch
            {
                // Fallback to List<T> for interfaces
                if (collectionType.IsInterface && collectionType.IsGenericType &&
                    collectionType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    Type elementType = collectionType.GetGenericArguments()[0];
                    return Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                }
                return FormatterServices.GetUninitializedObject(collectionType);
            }
        }

        private struct ListAccessors
        {
            public Action<object, int, object> SetItem;
            public Func<object, int, object> GetItem;

            public ListAccessors(Action<object, int, object> setter, Func<object, int, object> getter)
            {
                SetItem = setter;
                GetItem = getter;
            }
        }

        private object HandleDictionaryWrite(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int idToRead, object value)
        {
            RouteInfo currentRoute = idRoute[idToRead];

            IDictionary dict = info.GetValue(obj) as IDictionary; // Get current dictionary or create a new one
            if (dict == null)
            {
                Type dictType = typeof(Dictionary<,>).MakeGenericType(typeof(object), currentRoute.ElementType ?? typeof(object));
                dict = (IDictionary)Activator.CreateInstance(dictType);
            }

            object key = FindKeyByHash(dict, currentRoute.collectionKey); // Get the key by a hash

            if (idRoute.Count <= idToRead + 1) // If we are in the final level, assign a value
            {
                if (key != null)
                {
                    dict[key] = value;
                }
            }
            else // If not keep going
            {
                object dictValue = key != null ? dict[key] : null;
                if (dictValue == null)
                {
                    dictValue = CreateDefaultInstance(currentRoute.ElementType);
                    if (key != null) dict[key] = dictValue;
                }

                InspectWrite(dictValue.GetType(), dictValue, idRoute, idToRead + 1, value);
            }

            info.SetValue(obj, dict);
            return obj;
        }

        private object HandleMultiDimensionalArrayWrite(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int idToRead, object value)
        {
            RouteInfo currentRoute = idRoute[idToRead];

            Array array = info.GetValue(obj) as Array; // Get current array or create a new one
            if (array == null)
            {
                array = Array.CreateInstance(currentRoute.ElementType ?? typeof(object), currentRoute.Dimensions.Length > 0 ? currentRoute.Dimensions : new[] { 0 });
            }

            int[] indices = currentRoute.GetMultiDimensionalIndices(); // Get multidimensional index

            if (idRoute.Count <= idToRead + 1) // If we are in the final level, assign a value
            {
                if (IsWithinBounds(array, indices))
                {
                    array.SetValue(value, indices);
                }
            }
            else // If not keep going
            {
                object element = array.GetValue(indices);
                if (element == null)
                {
                    element = CreateDefaultInstance(currentRoute.ElementType);
                    array.SetValue(element, indices);
                }

                InspectWrite(element.GetType(), element, idRoute, idToRead + 1, value);
            }

            info.SetValue(obj, array);
            return obj;
        }

        private object HandleJaggedArrayWrite(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int idToRead, object value)
        {
            RouteInfo currentRoute = idRoute[idToRead];

            Array jaggedArray = info.GetValue(obj) as Array; // Get current jagged array or create a new one
            if (jaggedArray == null)
            {
                jaggedArray = Array.CreateInstance(currentRoute.ElementType?.MakeArrayType() ?? typeof(object).MakeArrayType(), currentRoute.Dimensions.Length > 0 ? currentRoute.Dimensions[0] : 0);
            }

            int[] pathIndices = currentRoute.Dimensions; // Get jagged index

            object currentArray = jaggedArray;
            for (int i = 0; i < pathIndices.Length - 1; i++) // Navigate till the intern array
            {
                Array innerArray = ((Array)currentArray).GetValue(pathIndices[i]) as Array;
                if (innerArray == null)
                {
                    innerArray = Array.CreateInstance(currentRoute.ElementType ?? typeof(object), pathIndices[i + 1]); ((Array)currentArray).SetValue(innerArray, pathIndices[i]);
                }
                currentArray = innerArray;
            }

            int lastIndex = pathIndices[pathIndices.Length - 1]; // Assign last value
            if (idRoute.Count <= idToRead + 1)
            {
                ((Array)currentArray).SetValue(value, lastIndex);
            }
            else
            {
                object element = ((Array)currentArray).GetValue(lastIndex);
                if (element == null)
                {
                    element = CreateDefaultInstance(currentRoute.ElementType);
                    ((Array)currentArray).SetValue(element, lastIndex);
                }

                InspectWrite(element.GetType(), element, idRoute, idToRead + 1, value);
            }

            info.SetValue(obj, jaggedArray);
            return obj;
        }

        private object HandleRegularCollectionWrite(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int idToRead, object value)
        {
            RouteInfo currentRoute = idRoute[idToRead];
            Type fieldType = info.FieldType;

            IList collection = info.GetValue(obj) as IList; // Get current collection or create a new one
            if (collection == null)
            {
                if (fieldType.IsArray)
                {
                    collection = Array.CreateInstance(currentRoute.ElementType ?? typeof(object), currentRoute.collectionSize);
                }
                else
                {
                    Type collectionType = fieldType.IsGenericType ? fieldType.GetGenericTypeDefinition().MakeGenericType(currentRoute.ElementType ?? typeof(object)) : typeof(List<>).MakeGenericType(currentRoute.ElementType ?? typeof(object));

                    collection = (IList)Activator.CreateInstance(collectionType);
                }
            }

            if (currentRoute.collectionKey >= 0 && currentRoute.collectionKey >= collection.Count) // Verify & adjust size if necessary
            {
                for (int i = collection.Count; i <= currentRoute.collectionKey; i++)
                {
                    collection.Add(CreateDefaultInstance(currentRoute.ElementType));
                }
            }

            if (currentRoute.collectionKey >= 0) // Assign value
            {
                if (idRoute.Count <= idToRead + 1)
                {
                    collection[currentRoute.collectionKey] = value;
                }
                else
                {
                    object element = collection[currentRoute.collectionKey];
                    if (element == null)
                    {
                        element = CreateDefaultInstance(currentRoute.ElementType);
                        collection[currentRoute.collectionKey] = element;
                    }

                    InspectWrite(element.GetType(), element, idRoute, idToRead + 1, value);
                }
            }

            info.SetValue(obj, collection);
            return obj;
        }

        private object HandleComplexObjectWrite(FieldInfo info, object obj, List<RouteInfo> idRoute, int idToRead, object value)
        {
            object objReference = info.GetValue(obj);

            if (objReference == null)
            {
                objReference = ConstructObject(info.FieldType);
                info.SetValue(obj, objReference);
            }

            if (idRoute.Count > idToRead + 1)
            {
                InspectWrite(info.FieldType, objReference, idRoute, idToRead + 1, value);
            }

            return obj;
        }

        private object FindKeyByHash(IDictionary dict, int keyHash)
        {
            foreach (object key in dict.Keys)
            {
                if (GetStableKeyHash(key) == keyHash)
                {
                    return key;
                }
            }
            return null;
        }

        private bool IsWithinBounds(Array array, int[] indices)
        {
            if (array.Rank != indices.Length) return false;

            for (int i = 0; i < indices.Length; i++)
            {
                if (indices[i] < 0 || indices[i] >= array.GetLength(i))
                {
                    return false;
                }
            }
            return true;
        }

        private object CreateDefaultInstance(Type type)
        {
            if (type == null) return null;
            if (type.IsValueType) return Activator.CreateInstance(type);
            return FormatterServices.GetUninitializedObject(type);
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

        private bool IsDictionary(Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);

        private bool IsJaggedArray(Type type) => type.IsArray && type.GetElementType()?.IsArray == true;

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

        private int GetStableKeyHash(object key)
        {
            if (key == null) return 0;

            // Use consistent string representation for complex keys
            string keyString = key is IConvertible convertible ? convertible.ToString(CultureInfo.InvariantCulture) : key.ToString();

            return keyString.GetHashCode();
        }

        private int[] GetArrayIndices(Array array, object value)
        {
            int[] indices = new int[array.Rank];
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = -1; // Initialize
            }

            // Find the indices of the value in the array
            // Note: This is simplified - may need optimization for large arrays
            foreach (var item in array)
            {
                bool found = true;
                for (int i = 0; i < indices.Length; i++)
                {
                    if (!object.Equals(item, value))
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    for (int i = 0; i < indices.Length; i++)
                    {
                        indices[i] = (int)array.GetValue(i);
                    }
                    break;
                }
            }

            return indices;
        }

        private int GetJaggedArrayIndex(object array, object value)
        {
            if (!(array is IEnumerable enumerable)) return -1;

            int index = 0;
            foreach (var item in enumerable)
            {
                if (object.Equals(item, value)) return index;
                index++;
            }
            return -1;
        }

        private Type GetDictionaryValueType(Type dictionaryType)
        {
            return dictionaryType.IsGenericType ?
                dictionaryType.GetGenericArguments()[1] :
                typeof(object);
        }

        private Type GetJaggedArrayElementType(Type jaggedArrayType)
        {
            Type elementType = jaggedArrayType.GetElementType();
            while (elementType != null && elementType.IsArray)
            {
                elementType = elementType.GetElementType();
            }
            return elementType ?? typeof(object);
        }

        private object TranslateICollection<T>(object[] objs)
        {
            List<T> toReturn = new List<T>();

            foreach (object obj in objs)
            {
                toReturn.Add((T)obj);
            }

            return toReturn;
        }

        private object TranslateArray<T>(object[] objectsToCopy)
        {
            T[] array = new T[objectsToCopy.Length];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = (T)objectsToCopy[i];
            }

            return array;
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