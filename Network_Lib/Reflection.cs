﻿using Network_Lib.BasicMessages;
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
using System.Xml.Linq;

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
            object fieldValue = info.GetValue(obj);
            Type fieldType = info.FieldType;

            if (fieldValue == null)
            {
                idRoute.Add(RouteInfo.CreateForProperty(attribute.VariableId));
                SendPackage(NullOrEmpty.Null, attribute, idRoute);
                return;
            }
            
            if ((fieldType.IsValueType && fieldType.IsPrimitive) || fieldType == typeof(string) || fieldType.IsEnum)
            {
                idRoute.Add(RouteInfo.CreateForProperty(attribute.VariableId));
                SendPackage(fieldValue, attribute, idRoute);
                return;
            }

            if (typeof(IEnumerable).IsAssignableFrom(fieldType))
            {
                IEnumerable collection = (IEnumerable)fieldValue;
                int count = GetCollectionCount(collection);

                if (count == 0)
                {
                    idRoute.Add(RouteInfo.CreateForCollection(attribute.VariableId, index: 0, size: 0, GetElementType(fieldType)));

                    SendPackage(fieldValue == null ? NullOrEmpty.Null : NullOrEmpty.Empty, attribute, idRoute);
                    return;
                }

                if (IsDictionary(fieldType))
                {
                    var dict = (IDictionary)fieldValue;
                    Type valueType = GetDictionaryValueType(fieldType);

                    foreach (DictionaryEntry entry in dict)
                    {
                        List<RouteInfo> currentRoute = new List<RouteInfo>(idRoute)
                        {
                            RouteInfo.CreateForDictionary(attribute.VariableId, GetStableKeyHash(entry.Key), valueType)
                        };

                        ProcessValue(entry.Value, currentRoute, attribute);
                    }
                    return;
                }

                if (fieldType.IsArray && fieldType.GetArrayRank() > 1)
                {
                    Array array = (Array)fieldValue;
                    Type elementType = fieldType.GetElementType();
                    int[] dimensions = new int[array.Rank];
                    for (int i = 0; i < dimensions.Length; i++)
                    {
                        dimensions[i] = array.GetLength(i);
                    }

                    foreach (object? item in array)
                    {
                        int[] indices = GetArrayIndices(array, item);
                        List<RouteInfo> currentRoute = new List<RouteInfo>(idRoute)
                        {
                            RouteInfo.CreateForMultiDimensionalArray(attribute.VariableId, indices, dimensions, elementType)
                        };

                        ProcessValue(item, currentRoute, attribute);
                    }
                    return;
                }

                if (IsJaggedArray(fieldType))
                {
                    Array jaggedArray = (Array)fieldValue;
                    Type elementType = GetJaggedArrayElementType(fieldType);

                    for (int i = 0; i < jaggedArray.Length; i++)
                    {
                        object innerArray = jaggedArray.GetValue(i);
                        if (innerArray == null) continue;

                        foreach (object? item in (IEnumerable)innerArray)
                        {
                            List<RouteInfo> currentRoute = new List<RouteInfo>(idRoute)
                            {
                                RouteInfo.CreateForJaggedArray(attribute.VariableId, new[] { i, GetJaggedArrayIndex(innerArray, item) }, elementType)
                            };

                            ProcessValue(item, currentRoute, attribute);
                        }
                    }
                    return;
                }

                int index = 0;
                foreach (var item in collection)
                {
                    List<RouteInfo> currentRoute = new List<RouteInfo>(idRoute)
                    {
                        RouteInfo.CreateForCollection(attribute.VariableId, index++, count, item?.GetType() ?? GetElementType(fieldType))
                    };

                    ProcessValue(item, currentRoute, attribute);
                }
                return;
            }
            else
            {
                idRoute.Add(new RouteInfo(attribute.VariableId));
                Inspect(fieldType, fieldValue, idRoute);
            }
        }

        public void SendPackage(object value, NetVariable attribute, List<RouteInfo> idRoute)
        {
            if (value is NullOrEmpty.Null)
            {
                NetNullMessage netNullMessage = new NetNullMessage(attribute.MessagePriority, null, idRoute);
                networkEntity.SendMessage(netNullMessage.Serialize());
                return;
            }

            if (value is NullOrEmpty.Empty)
            {
                NetEmptyMessage netEmptyMessage = new NetEmptyMessage(attribute.MessagePriority, new Empty(), idRoute);
                networkEntity.SendMessage(netEmptyMessage.Serialize());
                return;
            }

            Type packageType = value.GetType();

            foreach (Type type in executeAssembly.GetTypes())
            {
                if (type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(BaseReflectionMessage<>))
                {
                    Type[] genericTypes = type.BaseType.GetGenericArguments();
                    foreach (Type arg in genericTypes)
                    {
                        if (packageType == arg)
                        {
                            try
                            {
                                object[] parameters = new[] { attribute.MessagePriority, value, idRoute };
                                ConstructorInfo ctor = type.GetConstructor(new[] {typeof(MessagePriority), packageType, typeof(List<RouteInfo>)
                            });

                                if (ctor != null)
                                {
                                    var message = (ParentBaseMessage)ctor.Invoke(parameters);
                                    networkEntity.SendMessage(message.Serialize());
                                }
                                else
                                {

                                }
                            }
                            catch (Exception ex)
                            {

                            }
                            return;
                        }
                    }
                }
            }
        }

        public void OnReceivedReflectionMessage(byte[] data, IPEndPoint ip)
        {
            //DeserializeReflectionMessage(data);
            string debug = "";
            debug += "Data type received: " + MessageChecker.CheckMessageType(data) + "\n";
            //consoleDebugger.Invoke(debug);
            switch (MessageChecker.CheckMessageType(data))
            {
                case MessageType.Ulong:

                    NetULongMessage netULongMessage = new NetULongMessage(data);
                    VariableMapping(netULongMessage.GetMessageRoute(), netULongMessage.GetData());

                    break;
                case MessageType.Uint:

                    NetUIntMessage netUIntMessage = new NetUIntMessage(data);
                    VariableMapping(netUIntMessage.GetMessageRoute(), netUIntMessage.GetData());

                    break;
                case MessageType.Ushort:

                    NetUShortMessage netUShortMessage = new NetUShortMessage(data);
                    VariableMapping(netUShortMessage.GetMessageRoute(), netUShortMessage.GetData());

                    break;
                case MessageType.String:
                    NetStringMessage netStringMessage = new NetStringMessage(data);
                    VariableMapping(netStringMessage.GetMessageRoute(), netStringMessage.GetData());

                    break;
                case MessageType.Short:

                    NetShortMessage netShortMessage = new NetShortMessage(data);
                    VariableMapping(netShortMessage.GetMessageRoute(), netShortMessage.GetData());

                    break;
                case MessageType.Sbyte:

                    NetSByteMessage netSByteMessage = new NetSByteMessage(data);
                    VariableMapping(netSByteMessage.GetMessageRoute(), netSByteMessage.GetData());

                    break;
                case MessageType.Long:

                    NetLongMessage netLongMessage = new NetLongMessage(data);
                    VariableMapping(netLongMessage.GetMessageRoute(), netLongMessage.GetData());

                    break;
                case MessageType.Int:

                    NetIntMessage netIntMessage = new NetIntMessage(data);
                    VariableMapping(netIntMessage.GetMessageRoute(), netIntMessage.GetData());

                    break;
                case MessageType.Float:

                    NetFloatMessage netFloatMessage = new NetFloatMessage(data);
                    VariableMapping(netFloatMessage.GetMessageRoute(), netFloatMessage.GetData());

                    break;
                case MessageType.Double:

                    NetDoubleMessage netDoubleMessage = new NetDoubleMessage(data);
                    VariableMapping(netDoubleMessage.GetMessageRoute(), netDoubleMessage.GetData());

                    break;
                case MessageType.Decimal:

                    NetDecimalMessage netDecimalMessage = new NetDecimalMessage(data);
                    VariableMapping(netDecimalMessage.GetMessageRoute(), netDecimalMessage.GetData());

                    break;
                case MessageType.Char:

                    NetCharMessage netCharMessage = new NetCharMessage(data);
                    VariableMapping(netCharMessage.GetMessageRoute(), netCharMessage.GetData());

                    break;
                case MessageType.Byte:

                    NetByteMessage netByteMessage = new NetByteMessage(data);
                    VariableMapping(netByteMessage.GetMessageRoute(), netByteMessage.GetData());

                    break;
                case MessageType.Bool:

                    NetBoolMessage netBoolMessage = new NetBoolMessage(data);
                    VariableMapping(netBoolMessage.GetMessageRoute(), netBoolMessage.GetData());

                    break;
                case MessageType.Null:

                    NetNullMessage netNullMessage = new NetNullMessage(data);
                    VariableMappingNullException(netNullMessage.GetMessageRoute(), netNullMessage.GetData());
                    break;
                case MessageType.Empty:

                    NetEmptyMessage netEmptyMessage = new NetEmptyMessage(data);
                    VariableMappingEmpty(netEmptyMessage.GetMessageRoute(), netEmptyMessage.GetData());

                    break;

                case MessageType.Method:

                    NetMethodMessage netMethodMessage = new NetMethodMessage(data);
                    InvokeReflectionMethod(netMethodMessage.GetData().Item1, netMethodMessage.GetData().Item2, netMethodMessage.GetMessageRoute()[0].route);

                    break;
            }
        }

        void VariableMapping(List<RouteInfo> route, object variableValue)
        {
            if (route == null || route.Count == 0) return;

            INetObj objectRoot = NetObjFactory.GetINetObject(route[0].route);

            if (objectRoot.GetOwnerID() != networkEntity.clientID)
            {
                _ = (INetObj)InspectWrite(objectRoot.GetType(), objectRoot, route, 1, variableValue);
            }
        }

        void VariableMappingNullException(List<RouteInfo> route, object variableValue)
        {
            if (route == null || route.Count == 0) return;

            INetObj objectRoot = NetObjFactory.GetINetObject(route[0].route);
            if (objectRoot.GetOwnerID() != networkEntity.clientID)
            {
                _ = (INetObj)InspectWriteNullException(objectRoot.GetType(), objectRoot, route, 1, variableValue);
            }
        }

        void VariableMappingEmpty(List<RouteInfo> route, object variableValue)
        {
            if (route == null || route.Count == 0) return;

            INetObj objectRoot = NetObjFactory.GetINetObject(route[0].route);
            if (objectRoot.GetOwnerID() != networkEntity.clientID)
            {
                _ = (INetObj)InspectWriteEmpty(objectRoot.GetType(), objectRoot, route, 1, variableValue);
            }
        }

        public object InspectWrite(Type type, object obj, List<RouteInfo> idRoute, int idToRead, object value)
        {
            if (obj == null || idRoute.Count <= idToRead)
                return obj;

            RouteInfo currentRoute = idRoute[idToRead];

            foreach (FieldInfo info in type.GetFields(bindingFlags)) // Regular fields
            {
                NetVariable attributes = info.GetCustomAttribute<NetVariable>();
                if (attributes != null && attributes.VariableId == currentRoute.route)
                {
                    return WriteValue(info, obj, attributes, idRoute, idToRead, value);
                }
            }

            if (extensionMethods.TryGetValue(type, out MethodInfo methodInfo)) // Extension fields
            {
                object unitializedObject = FormatterServices.GetUninitializedObject(type);
                object fields = methodInfo.Invoke(null, new object[] { unitializedObject });

                if (fields is List<(FieldInfo, NetVariable)> values)
                {
                    foreach (var field in values)
                    {
                        if (field.Item2.VariableId == currentRoute.route)
                        {
                            return WriteValue(field.Item1, obj, field.Item2, idRoute, idToRead, value);
                        }
                    }
                }
            }

            return obj;
        }

        public object InspectWriteNullException(Type type, object obj, List<RouteInfo> idRoute, int idToRead, object value)
        {
            if (obj == null || idRoute.Count <= idToRead)
                return obj;

            RouteInfo currentRoute = idRoute[idToRead];

            foreach (FieldInfo info in type.GetFields(bindingFlags)) // Regular fields
            {
                NetVariable attributes = info.GetCustomAttribute<NetVariable>();
                if (attributes != null && attributes.VariableId == currentRoute.route)
                {
                    return WriteValueNullException(info, obj, attributes, idRoute, idToRead, value);
                }
            }

            return obj;
        }

        public object InspectWriteEmpty(Type type, object obj, List<RouteInfo> idRoute, int idToRead, object value)
        {
            if (obj == null || idRoute.Count <= idToRead)
                return obj;

            RouteInfo currentRoute = idRoute[idToRead];

            foreach (FieldInfo info in type.GetFields(bindingFlags)) // Regular fields
            {
                NetVariable attributes = info.GetCustomAttribute<NetVariable>();
                if (attributes != null && attributes.VariableId == currentRoute.route)
                {
                    return WriteValueNullException(info, obj, attributes, idRoute, idToRead, value);
                }
            }

            return obj;
        }

        object WriteValue(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int idToRead, object value)
        {
            RouteInfo currentRoute = idRoute[idToRead];
            Type fieldType = info.FieldType;

            if ((fieldType.IsValueType && fieldType.IsPrimitive) || fieldType == typeof(string) || fieldType.IsEnum) // Simple cases
            {
                info.SetValue(obj, value);
                return obj;
            }

            if (typeof(IEnumerable).IsAssignableFrom(fieldType))
            {
                return HandleCollectionWrite(info, obj, attribute, idRoute, idToRead, value);
            }

            return HandleComplexObjectWrite(info, obj, idRoute, idToRead, value);
        }

        private object HandleCollectionWrite(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int idToRead, object value)
        {
            RouteInfo currentRoute = idRoute[idToRead];
            Type fieldType = info.FieldType;

            if (currentRoute.IsDictionary) // Dictionaries
            {
                return HandleDictionaryWrite(info, obj, attribute, idRoute, idToRead, value);
            }

            if (currentRoute.IsMultiDimensionalArray)  // Multidimensional Array
            {
                return HandleMultiDimensionalArrayWrite(info, obj, attribute, idRoute, idToRead, value);
            }

            if (currentRoute.IsJaggedArray)  // Jagged Array
            {
                return HandleJaggedArrayWrite(info, obj, attribute, idRoute, idToRead, value);
            }

            return HandleRegularCollectionWrite(info, obj, attribute, idRoute, idToRead, value);  // Regular collections
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
                    collection = Array.CreateInstance(currentRoute.ElementType ?? typeof(object),currentRoute.collectionSize);
                }
                else
                {
                    Type collectionType = fieldType.IsGenericType ?fieldType.GetGenericTypeDefinition().MakeGenericType(currentRoute.ElementType ?? typeof(object)) : typeof(List<>).MakeGenericType(currentRoute.ElementType ?? typeof(object));

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
            if (value == null)
            {
                SendPackage(NullOrEmpty.Null, attribute, route);
                return;
            }

            Type valueType = value.GetType();

            if ((valueType.IsValueType && valueType.IsPrimitive) || valueType == typeof(string) || valueType == typeof(decimal) || valueType.IsEnum)
            {
                SendPackage(value, attribute, route);
            }
            else
            {
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