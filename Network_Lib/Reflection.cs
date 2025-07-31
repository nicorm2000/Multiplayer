using System.Runtime.Serialization;
using System.Collections.Generic;
using Network_Lib.BasicMessages;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Net;
using System;

namespace Net
{
    /// <summary>
    /// Provides reflection-based inspection and manipulation of network objects.
    /// Handles serialization, deserialization, and network communication of object states.
    /// </summary>
    public class Reflection
    {
        #region Fields and Properties
        private Assembly executeAssembly;
        private Assembly gameAssembly;
        private NETAUTHORITY netAuthority;
        private ReflectionCollectionHelper reflectionCollectionHelper;
        private ReflectionInspector reflectionInspector;
        private ReflectionMapping reflectionMapping;
        private ReflectionReader reflectionReader;
        private ReflectionWriter reflectionWriter;
        public ReflectionMessageHandler reflectionMessageHandler;
        public ReflectionCallInvoker reflectionCallInvoker;
        public NetworkEntity networkEntity { get; private set; }
        public BindingFlags bindingFlags { get; private set; }

        public IReflectionDebugger debugger;
        public Dictionary<Type, MethodInfo> extensionMethods = new Dictionary<Type, MethodInfo>();
        private Dictionary<object, Dictionary<object, int>> previousDictionaryStates = new Dictionary<object, Dictionary<object, int>>();
        private readonly Dictionary<object, int> previousCollectionCounts = new Dictionary<object, int>();
        #endregion

        #region Initialization
        /// <summary>
        /// Provides reflection-based inspection and manipulation of network objects.
        /// Handles serialization, deserialization, and network communication of object states.
        /// </summary>
        public Reflection(NetworkEntity entity, NETAUTHORITY netAuthority, IReflectionDebugger debugger)
        {
            reflectionCollectionHelper = new ReflectionCollectionHelper(this);
            reflectionInspector = new ReflectionInspector(this);
            reflectionMapping = new ReflectionMapping(this);
            reflectionMessageHandler = new ReflectionMessageHandler(this);
            reflectionReader = new ReflectionReader(this);
            reflectionWriter = new ReflectionWriter(this);
            reflectionCallInvoker = new ReflectionCallInvoker(this);

            networkEntity = entity;
            if (netAuthority == NETAUTHORITY.CLIENT)
                networkEntity.OnReceivedMessage += reflectionMessageHandler.OnReceivedReflectionMessage;
            this.netAuthority = netAuthority;
            this.debugger = debugger;

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
                            debugger?.Log($"Registered extension for: {netExtensionMethod.extensionMethod.Name}");
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
            if (NetObjFactory.NetObjects().Count <= 0)
            {
                return;
            }

            foreach (INetObj netObj in NetObjFactory.NetObjects())
            {
                List<RouteInfo> idRoute = new List<RouteInfo>
                {
                    RouteInfo.CreateForProperty(netObj.GetID())
                };
                Inspect(netObj.GetType(), netObj, idRoute, netObj.GetOwnerID());

                if (netObj.GetTRS() != null && netAuthority == NETAUTHORITY.SERVER)
                {
                    TRS trs = netObj.GetTRS();
                    NetTRSMessage netTRSMessage = new NetTRSMessage(MessagePriority.Default, trs, idRoute);
                    networkEntity.SendMessage(netTRSMessage.Serialize());
                }
            }
        }

        /// <summary>
        /// Inspects an object and its fields recursively, sending network messages for any changes.
        /// </summary>
        /// <param name="type">The type of the object to inspect.</param>
        /// <param name="obj">The object instance to inspect.</param>
        /// <param name="idRoute">The route information for network message routing.</param>
        public void Inspect(Type type, object obj, List<RouteInfo> idRoute, int owner)
        {
            string debug = "";
            if (obj != null)
            {
                foreach (FieldInfo info in ReflectionHelperMethods.GetAllFields(type, bindingFlags))
                {
                    NetVariable netVarAux = info.GetCustomAttribute<NetVariable>();
                    if (netVarAux != null)
                    {
                        debug += "___info field: " + info + "\n";
                        debug += "___info route: " + idRoute[0].route + "\n";
                        //debugger?.Log(debug);
                        if (netVarAux.syncAuthority == netAuthority)
                        {
                            //debugger?.Log($"Inspect: {owner}, {networkEntity.clientID}");
                            if (extensionMethods.TryGetValue(info.FieldType, out MethodInfo methodInfo))
                            {
                                CheckAuthority(owner, netVarAux.syncAuthority, ReadValueEMAction, ReadValueEMAction);
                                void ReadValueEMAction()
                                {
                                    object actualObject = info.GetValue(obj);

                                    object fields = methodInfo.Invoke(null, new object[] { actualObject, netVarAux.syncAuthority });
                                    if (fields is List<(FieldInfo, NetVariable)> values)
                                    {
                                        foreach ((FieldInfo, NetVariable) field in values)
                                        {
                                            List<RouteInfo> structRoute = new List<RouteInfo>(idRoute);
                                            structRoute.Add(RouteInfo.CreateForProperty(netVarAux.VariableId));
                                            structRoute.Add(RouteInfo.CreateForProperty(field.Item2.VariableId));
                                            object componentValue = field.Item1.GetValue(actualObject);

                                            //debugger?.Log($"Inspect: {info.FieldType} {info.GetValue(obj)}\n");
                                            ReadValue(field.Item1, actualObject, field.Item2, structRoute, owner);

                                            info.SetValue(obj, actualObject);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                CheckAuthority(owner, netVarAux.syncAuthority, ReadValueAction, ReadValueAction);
                                void ReadValueAction()
                                {
                                    List<RouteInfo> extendedRoute = new List<RouteInfo>(idRoute);
                                    //extendedRoute.Add(RouteInfo.CreateForProperty(aux.VariableId));
                                    //debugger?.Log($"Full Route: {string.Join("->", extendedRoute.Select(r => r.route))}\n");
                                    //debugger?.Log($"Inspect: {info.FieldType} {info.GetValue(obj)}\n");
                                    ReadValue(info, obj, netVarAux, extendedRoute, owner);
                                }
                            }
                        }

                        if (type.BaseType != null)
                        {
                            Inspect(type.BaseType, obj, new List<RouteInfo>(idRoute), owner);
                        }
                    }
                }
                debug += "Exit foreach: " + obj + "\n";
                //debugger?.Log(debug);
            }
            else
            {
                debug += "Object is NULL";
                //debugger?.Log(debug);
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
        public void ReadValue(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int owner)
        {
            string debug = "ReadValue Start - ";
            debug += $"Field: {info.Name}, Type: {info.FieldType}, Current Route: {string.Join("->", idRoute.Select(r => r.route))}\n";
            //debugger?.Log(debug);

            object fieldValue = info.GetValue(obj);
            Type fieldType = info.FieldType;

            // Handle null case
            if (fieldValue == null)
            {
                idRoute.Add(RouteInfo.CreateForProperty(attribute.VariableId));
                SendPackage(PossibleStates.Null, attribute, idRoute);
                return;
            }

            // Handle simple types
            if (ReflectionHelperMethods.IsSimpleType(info.FieldType))
            {
                //debugger?.Log("Simple Type: " + fieldValue + fieldType);
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

                    foreach (int[] indices in ReflectionHelperMethods.GetArrayIndices(mdArray))
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
                        ProcessValue(element, currentRoute, attribute, owner);
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
                                int keyHash = ReflectionHelperMethods.GetStableKeyHash(key);
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
                                //debugger?.Log($"Sending Remove - Full Data: {BitConverter.ToString(serialized)}");
                                networkEntity.SendMessage(serialized);
                            }

                            // Update state and RETURN after processing removals
                            previousDictionaryStates[dictionary] = currentKeys.ToDictionary(k => k, ReflectionHelperMethods.GetStableKeyHash);
                            debug += ("--- REMOVALS PROCESSED ---");
                            //debugger?.Log(debug.ToString());
                            return;
                        }
                    }

                    // Only process current values if no removals occurred
                    foreach (DictionaryEntry entry in dictionary)
                    {
                        ProcessValue(entry.Value, new List<RouteInfo>(idRoute)
                        {
                            RouteInfo.CreateForDictionary(attribute.VariableId, ReflectionHelperMethods.GetStableKeyHash(entry.Key), valueType)
                        }, attribute, owner);
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
                    previousDictionaryStates[dictionary] = currentKeys.ToDictionary(k => k, ReflectionHelperMethods.GetStableKeyHash);

                    debug += ("--- INSPECTION COMPLETE ---");
                    //debugger?.Log(debug);
                    return;
                }
                else
                {
                    //debugger?.Log($"Processing as generic collection: {fieldType.Name}");

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
                                        elementType: ReflectionHelperMethods.GetElementType(fieldType))
                                };

                                NetRemoveMessage removeMessage = new NetRemoveMessage(
                                    attribute.MessagePriority,
                                    -1,
                                    removeRoute);

                                byte[] serialized = removeMessage.Serialize();
                                //debugger?.Log($"[ReadValue] Sending Remove for index {removedIndex} (count reduced) - Data: {BitConverter.ToString(serialized)}");
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
                                elementType: item?.GetType() ?? ReflectionHelperMethods.GetElementType(fieldType))
                        };

                        //debugger?.Log($"Processing collection item [{index - 1}]: " +
                        //                      $"Type: {item?.GetType()?.Name ?? "null"}, " +
                        //                      $"Value: {item ?? "null"}");

                        ProcessValue(item, currentRoute, attribute, owner);
                    }

                    if (count == 0)
                    {
                        //debugger?.Log("Collection is empty");
                        idRoute.Add(new RouteInfo(
                            attribute.VariableId,
                            collectionKey: -1,
                            collectionSize: 0,
                            elementType: ReflectionHelperMethods.GetElementType(fieldType)));
                        SendPackage(PossibleStates.Empty, attribute, idRoute);
                    }
                }
            }

            // Handle complex objects
            //debugger?.Log("Handling complex object type\n");
            idRoute.Add(RouteInfo.CreateForProperty(attribute.VariableId));
            //debugger?.Log("Complex object: " + fieldValue + fieldType);
            //debugger?.Log($"Full Route Read: {string.Join("->", idRoute.Select(r => r.route))}\n");
            Inspect(fieldType, fieldValue, idRoute, owner);
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
            //debugger?.Log(debug);

            if (value is PossibleStates.Null)
            {
                //debugger?.Log("Sending NullMessage\n");
                NetNullMessage netNullMessage = new NetNullMessage(attribute.MessagePriority, null, idRoute);
                networkEntity.SendMessage(netNullMessage.Serialize());
                return;
            }

            if (value is PossibleStates.Empty)
            {
                //debugger?.Log("Sending EmptyMessage\n");
                NetEmptyMessage netEmptyMessage = new NetEmptyMessage(attribute.MessagePriority, new Empty(), idRoute);
                networkEntity.SendMessage(netEmptyMessage.Serialize());
                return;
            }

            if (value is PossibleStates.Remove)
            {
                //debugger?.Log("Sending RemoveMessage\n");
                int keyHash = idRoute.Last().collectionKey;
                NetRemoveMessage netRemoveMessage = new NetRemoveMessage(attribute.MessagePriority, keyHash, idRoute);
                byte[] serialized = netRemoveMessage.Serialize();
                //debugger?.Log($"Sending Remove - KeyHash: {keyHash}, Data: {BitConverter.ToString(serialized)}");
                networkEntity.SendMessage(serialized);
                return;
            }

            if (value is Enum enumValue)
            {
                //debugger?.Log("Sending Enum package\n");
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
                                    //debugger?.Log(debug);
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
            //debugger?.Log(debug);
        }
        #endregion

        #region Variable Mapping
        /// <summary>
        /// Maps received network values to their corresponding object fields.
        /// </summary>
        /// <param name="route">Route information for the value.</param>
        /// <param name="variableValue">The value to map.</param>
        public void VariableMapping(List<RouteInfo> route, object variableValue)
        {
            string debug = $"VariableMapping - Start\n";
            debug += $"Type: {variableValue?.GetType()?.Name ?? "null"}, Value: {variableValue}\n";
            debug += $"Route: {string.Join("->", route.Select(r => r.route))}\n";

            try
            {
                if (route == null || route.Count == 0)
                {
                    //debugger?.Log("Empty route, aborting\n");
                    return;
                }

                INetObj objectRoot = NetObjFactory.GetINetObject(route[0].route);
                if (objectRoot == null)
                {
                    //debugger?.Log($"No INetObj found for ID: {route[0].route}\n");
                    return;
                }

                debug += $"Found root object: {objectRoot.GetType().Name} (OwnerID: {objectRoot.GetOwnerID()})\n";
                debug += $"NetworkEntity ClientID: {networkEntity.clientID}\n";

                debug += "Proceeding with write operation\n";
                //object a = NetObjFactory.GetObject(route[0].route);
                InspectWrite(objectRoot.GetType(), objectRoot, route, 1, variableValue);
                //debug += $"InspectWrite completed. Result: {a}\n";
                //debugger?.Log(debug);
            }
            catch (Exception ex)
            {
                //debugger?.Log($"VariableMapping error: {ex.Message}\n{ex.StackTrace}");
            }

            //debugger?.Log(debug);
        }

        /// <summary>
        /// Handles mapping of null values received from the network.
        /// </summary>
        /// <param name="route">Route information for the value.</param>
        /// <param name="variableValue">The null value to map.</param>
        public void VariableMappingNullException(List<RouteInfo> route, object variableValue)
        {
            string debug = "VariableMappingNullException - ";
            debug += $"Value: {variableValue}, Type: {variableValue?.GetType()?.Name ?? "null"}, ";
            debug += $"Route: {string.Join("->", route.Select(r => r.route))}\n";

            if (route == null || route.Count == 0)
            {
                //debugger?.Log("Empty route\n");
                return;
            }

            INetObj objectRoot = NetObjFactory.GetINetObject(route[0].route);
            if (objectRoot == null)
            {
                //debugger?.Log($"No INetObj found for ID: {route[0].route}\n");
                return;
            }

            debug += $"Root Object: {objectRoot.GetType().Name}, OwnerID: {objectRoot.GetOwnerID()}, NetworkEntity ClientID: {networkEntity.clientID}\n";

            debug += "Processing write operation for null exception\n";
            //debugger?.Log(debug);
            InspectWriteNullException(objectRoot.GetType(), objectRoot, route, 1, variableValue);
        }

        /// <summary>
        /// Handles mapping of empty values (collections) received from the network.
        /// </summary>
        /// <param name="route">Route information for the value.</param>
        /// <param name="variableValue">The empty value to map.</param>
        public void VariableMappingEmpty(List<RouteInfo> route, object variableValue)
        {
            string debug = "VariableMappingEmpty - ";
            debug += $"Value: {variableValue}, Type: {variableValue?.GetType()?.Name ?? "null"}, ";
            debug += $"Route: {string.Join("->", route.Select(r => r.route))}\n";

            if (route == null || route.Count == 0)
            {
                //debugger?.Log("Empty route\n");
                return;
            }

            INetObj objectRoot = NetObjFactory.GetINetObject(route[0].route);
            if (objectRoot == null)
            {
                //debugger?.Log($"No INetObj found for ID: {route[0].route}\n");
                return;
            }

            debug += $"Root Object: {objectRoot.GetType().Name}, OwnerID: {objectRoot.GetOwnerID()}, NetworkEntity ClientID: {networkEntity.clientID}\n";

            debug += "Processing empty collection\n";
            InspectWriteEmpty(objectRoot.GetType(), objectRoot, route, 1, variableValue);
            //debugger?.Log(debug);
        }

        public void TRSMapping(List<RouteInfo> route, TRS data)
        {
            if (route == null || route.Count == 0)
            {
                //debugger?.Log("Empty route\n");
                return;
            }

            INetObj objectRoot = NetObjFactory.GetINetObject(route[0].route);
            if (objectRoot == null)
            {
                //debugger?.Log($"No INetObj found for ID: {route[0].route}\n");
                return;
            }

            NetTRS auxSync = objectRoot.GetType().GetCustomAttribute<NetTRS>();
            objectRoot.SetTRS(data, auxSync != null ? auxSync.syncData : NetTRS.SYNC.DEFAULT);
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
                    //debugger?.Log("Target object is null\n");
                    return null;
                }

                if (idRoute.Count <= idToRead)
                {
                    //debugger?.Log("Route exhausted without finding target\n");
                    return obj;
                }

                RouteInfo currentRoute = idRoute[idToRead];
                debug += $"Current Route Info: {currentRoute}\n";
                // Regular fields check
                foreach (FieldInfo info in ReflectionHelperMethods.GetAllFields(type, bindingFlags))
                {
                    NetVariable attributes = info.GetCustomAttribute<NetVariable>();
                    if (attributes != null)
                    {
                        if (attributes.VariableId == currentRoute.route)
                        {
                            if (extensionMethods.TryGetValue(info.FieldType, out MethodInfo methodInfo))
                            {
                                object structInstance = info.GetValue(obj);

                                object fields = methodInfo.Invoke(null, new object[] { structInstance, attributes.syncAuthority });

                                if (fields is List<(FieldInfo, NetVariable)> values)
                                {
                                    foreach ((FieldInfo, NetVariable) field in values)
                                    {
                                        if (idRoute[idToRead + 1].route == field.Item2.VariableId)
                                        {
                                            //debugger?.Log($"InspectWrite: Writing to {info.Name}.{field.Item1.Name}");
                                            object currentStruct = info.GetValue(obj);
                                            return WriteValue(field.Item1, currentStruct, field.Item2, idRoute, idToRead + 1, value, info, obj);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                object structInstance = info.GetValue(obj);
                                if (structInstance == null)
                                {
                                    structInstance = ReflectionHelperMethods.ConstructObject(info.FieldType, bindingFlags);
                                    info.SetValue(obj, structInstance);
                                }
                                //debug += $"Found matching field: {info.Name} (Type: {info.FieldType.Name})\n";
                                //debug += $"Current field value: {info.GetValue(obj)}\n";
                                //debugger?.Log(debug);
                                return WriteValue(info, obj, attributes, idRoute, idToRead, value);
                            }
                        }
                        // Extension fields check
                    }
                }

                debug += "No matching field found in this type\n";
            }
            catch (Exception ex)
            {
                debug += $"InspectWrite error: {ex.Message}\n{ex.StackTrace}";
            }

            //debugger?.Log(debug);
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
                //debugger?.Log($"Exit condition - obj null: {obj == null}, route count: {idRoute.Count}, idToRead: {idToRead}\n");
                return obj;
            }

            RouteInfo currentRoute = idRoute[idToRead];
            debug += $"Current Route Info: {currentRoute}\n";

            foreach (FieldInfo info in ReflectionHelperMethods.GetAllFields(type, bindingFlags))
            {
                NetVariable attributes = info.GetCustomAttribute<NetVariable>();
                if (attributes != null && attributes.VariableId == currentRoute.route)
                {
                    debug += $"Found matching field: {info.Name}, VariableId: {attributes.VariableId}\n";
                    //debugger?.Log(debug);
                    return WriteValueNullException(info, obj, attributes, idRoute, idToRead, value);
                }
            }

            debug += "No matching field found\n";
            //debugger?.Log(debug);
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
                //debugger?.Log($"Exit condition - obj null: {obj == null}, route count: {idRoute.Count}, idToRead: {idToRead}\n");
                return obj;
            }

            RouteInfo currentRoute = idRoute[idToRead];
            debug += $"Current Route Info: {currentRoute}\n";

            foreach (FieldInfo info in ReflectionHelperMethods.GetAllFields(type, bindingFlags))
            {
                NetVariable attributes = info.GetCustomAttribute<NetVariable>();
                if (attributes != null && attributes.VariableId == currentRoute.route)
                {
                    debug += $"Found matching field: {info.Name}, VariableId: {attributes.VariableId}\n";
                    //debugger?.Log(debug);
                    return WriteValueNullException(info, obj, attributes, idRoute, idToRead, value);
                }
            }

            debug += "No matching field found\n";
            //debugger?.Log(debug);
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
        public object WriteValue(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int idToRead, object value, FieldInfo parentField = null, object parentObject = null)
        {
            //debugger?.Log($"WriteValue - Field: {info.Name}, ValueType: {value?.GetType().Name}");

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
            if (ReflectionHelperMethods.IsSimpleType(fieldType))
            {
                if (parentField != null && parentObject != null)
                {
                    //debugger?.Log($"Parent field: {parentField.Name}, Parent object: {parentObject}");

                    object boxedStruct = parentField.GetValue(parentObject);
                    //debugger?.Log($"Boxed struct before: {boxedStruct}");

                    info.SetValue(boxedStruct, value);
                    //debugger?.Log($"After field set (before parent set): {boxedStruct}");

                    parentField.SetValue(parentObject, boxedStruct);

                    object verifiedStruct = parentField.GetValue(parentObject);
                    object verifiedValue = info.GetValue(verifiedStruct);
                    //debugger?.Log($"Verification - Struct: {verifiedStruct}, Field: {verifiedValue}");
                    info.SetValue(obj, verifiedValue);
                }
                else
                {
                    info.SetValue(obj, value);
                    //debugger?.Log($"Direct field set completed");
                }
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
                        //debugger?.Log($"[WriteValue] Removing item at index {currentRoute.collectionKey} from '{info.Name}'");
                        list.RemoveAt(currentRoute.collectionKey);
                    }
                    else
                    {
                        //debugger?.Log($"[WriteValue] Cannot remove at index {currentRoute.collectionKey} — out of bounds or invalid list");
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
                //debugger?.Log($"[WriteValue] Attempting to assign collection element at index {currentRoute.collectionKey} with value: {value}");
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
                                element = ReflectionHelperMethods.ConstructObject(fieldType.GetElementType(), bindingFlags);
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
                        //debugger?.Log($"[WriteValue] Field '{info.Name}' is null on receiver. Attempting to construct new instance of {fieldType.Name}.");
                        currentValue = ReflectionHelperMethods.ConstructObject(fieldType, bindingFlags);
                        info.SetValue(obj, currentValue);
                    }

                    // Always check if we need to prefill it
                    if (currentRoute.IsCollection && (currentValue as ICollection)?.Count < currentRoute.collectionSize)
                    {
                        Type elementTypeToFill = ReflectionHelperMethods.GetElementType(fieldType) ?? typeof(object);
                        MethodInfo addMethod = fieldType.GetMethod("Add");

                        int currentCount = (currentValue as ICollection)?.Count ?? 0;
                        int fillCount = currentRoute.collectionSize - currentCount;

                        for (int i = 0; i < fillCount; i++)
                        {
                            object defaultValue = elementTypeToFill.IsValueType ? Activator.CreateInstance(elementTypeToFill) : null;

                            addMethod?.Invoke(currentValue, new object[] { defaultValue });
                        }

                        //debugger?.Log($"[WriteValue] Pre-filled {fieldType.Name} with {fillCount} additional default elements (now has {currentRoute.collectionSize})");
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
                            Type nestedElementType = ReflectionHelperMethods.GetElementType(fieldType) ?? typeof(object);
                            nestedElement = ReflectionHelperMethods.ConstructObject(nestedElementType, bindingFlags);
                            TrySetCollectionIndexValue(currentValue, currentRoute.collectionKey, nestedElement);
                        }

                        InspectWrite(nestedElement.GetType(), nestedElement, idRoute, idToRead + 1, value);
                        return obj;
                    }

                    Type elementType = ReflectionHelperMethods.GetElementType(fieldType);
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

                    //debugger?.Log($"[WriteValue] Successfully constructed and assigned new {fieldType.Name} to field '{info.Name}'");

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
                    //debugger?.Log($"[WriteValue] Skipping write to invalid index {currentRoute.collectionKey} in collection '{info.Name}'");
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
                            //debugger?.Log($"[WriteValue] Skipping InspectWrite for invalid collectionKey {currentRoute.collectionKey} in '{info.Name}'");
                            return obj;
                        }

                        object element = fieldType.IsArray ? ((Array)newCollection).GetValue(currentRoute.collectionKey) : ((IList)newCollection)[currentRoute.collectionKey];

                        if (element == null)
                        {
                            element = ReflectionHelperMethods.ConstructObject(ReflectionHelperMethods.GetElementType(fieldType), bindingFlags);
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
                objReference = ReflectionHelperMethods.ConstructObject(info.FieldType, bindingFlags);
            }
            else if (idRoute.Count > idToRead + 1)
            {
                objReference = InspectWrite(info.FieldType, info.GetValue(obj), idRoute, idToRead + 1, value);
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
        public object? TryGetCollectionIndexValue(object collection, int index)
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
        public bool TrySetCollectionIndexValue(object collection, int index, object value)
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
                    //debugger?.Log($"[TrySetCollectionIndexValue] Set via indexer [{index}] = {value} on {type.Name}");
                    return true;
                }
                catch (Exception ex)
                {
                    //debugger?.Log($"[TrySetCollectionIndexValue] Indexer set failed: {ex.Message}");
                }
            }

            // 2. IList
            if (collection is IList list && index < list.Count)
            {
                try
                {
                    list[index] = value;
                    //debugger?.Log($"[TrySetCollectionIndexValue] Set via IList at index {index} to {value}");
                    return true;
                }
                catch (Exception ex)
                {
                    //debugger?.Log($"[TrySetCollectionIndexValue] IList set failed: {ex.Message}");
                }
            }

            // 3. Insert(int, T)
            MethodInfo? insertMethod = type.GetMethod("Insert", new[] { typeof(int), typeof(object) });
            if (insertMethod != null)
            {
                try
                {
                    insertMethod.Invoke(collection, new object[] { index, value });
                    //debugger?.Log($"[TrySetCollectionIndexValue] Inserted value at index {index}");
                    return true;
                }
                catch (Exception ex)
                {
                    //debugger?.Log($"[TrySetCollectionIndexValue] Insert failed: {ex.Message}");
                }
            }

            //debugger?.Log($"[TrySetCollectionIndexValue] Failed to set index {index} on {type.Name}");
            return false;
        }

        /// <summary>
        /// Handles removal of dictionary entries based on key hash.
        /// </summary>
        /// <param name="info">Field information for the dictionary.</param>
        /// <param name="obj">Parent object containing the dictionary.</param>
        /// <param name="keyHash">Hash of the key to remove.</param>
        /// <returns>The modified object.</returns>
        public object HandleDictionaryRemove(FieldInfo info, object obj, int keyHash)
        {
            IDictionary dictionary = (IDictionary)info.GetValue(obj);
            if (dictionary == null)
            {
                //debugger?.Log("ERROR: Dictionary is null");
                return obj;
            }

            //debugger?.Log($"Target Dictionary: {info.Name} | Current Keys: {string.Join(",", dictionary.Keys.Cast<object>())}");
            //debugger?.Log($"Searching for key with hash: {keyHash}");

            bool found = false;
            foreach (object key in dictionary.Keys)
            {
                int currentHash = ReflectionHelperMethods.GetStableKeyHash(key);
                if (currentHash == keyHash)
                {
                    //debugger?.Log($"FOUND KEY: {key} (Hash: {currentHash}) - REMOVING");
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

            //if (!found) debugger?.Log("WARNING: No matching key found");
            //debugger?.Log($"Final Keys: {string.Join(",", dictionary.Keys.Cast<object>())}");
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
        public object HandleDictionaryWrite(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int idToRead, object value)
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
            debug += $"Current dictionary keys: {string.Join(", ", dictionary.Keys.Cast<object>().Select(k => $"{k}(hash:{ReflectionHelperMethods.GetStableKeyHash(k)})"))}\n";

            object matchingKey = ReflectionHelperMethods.FindMatchingKey(dictionary, currentRoute.collectionKey);
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
                object element = dictionary[matchingKey] ?? ReflectionHelperMethods.ConstructObject(valueType, bindingFlags);
                dictionary[matchingKey] = element;
                InspectWrite(element.GetType(), element, idRoute, idToRead + 1, value);
            }

            info.SetValue(obj, dictionary);
            debug += $"Final dictionary state: {string.Join(", ", dictionary.Keys.Cast<object>().Select(k => $"{k}={dictionary[k]}"))}\n";

            //debugger?.Log(debug);
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
        public object WriteValueNullException(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int idToRead, object value)
        {
            //debugger?.Log($"WriteValueNullException - Field: {info.Name}, Type: {info.FieldType}, Value: {value}");

            RouteInfo currentRoute = idRoute[idToRead];
            Type fieldType = info.FieldType;

            // Handle simple types
            if (ReflectionHelperMethods.IsSimpleType(fieldType))
            {
                info.SetValue(obj, null);
                return obj;
            }

            bool isEmpty = value is Empty || (value?.ToString() == "Empty");

            if (isEmpty)
            {
                //debugger?.Log("Processing EMPTY state");

                object currentValue = info.GetValue(obj);
                if (currentValue is IEnumerable enumerable && currentValue != null)
                {
                    //debugger?.Log("Processing collection");
                    MethodInfo clearMethod = currentValue.GetType().GetMethod("Clear");
                    if (clearMethod != null)
                    {
                        //debugger?.Log("Invoking Clear()");
                        clearMethod.Invoke(currentValue, null);
                    }
                    else
                    {
                        //debugger?.Log("No Clear() found - creating new instance");
                        currentValue = FormatterServices.GetUninitializedObject(info.FieldType);
                        info.SetValue(obj, currentValue);
                    }
                }
                else if (currentValue == null)
                {
                    //debugger?.Log("Creating new empty instance");
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
        /// Processes a value and sends appropriate network messages.
        /// </summary>
        /// <param name="value">The value to process.</param>
        /// <param name="route">Route information for the value.</param>
        /// <param name="attribute">NetVariable attribute containing metadata.</param>
        private void ProcessValue(object value, List<RouteInfo> route, NetVariable attribute, int owner)
        {
            string debug = "ProcessValue - ";
            debug += $"Value: {value ?? "null"}, Type: {value?.GetType()?.Name ?? "null"}, ";
            debug += $"Route: {string.Join("->", route.Select(r => r.route))}\n";

            if (value == null)
            {
                debug += "Sending NULL package\n";
                //debugger?.Log(debug);
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
                //debugger?.Log(debug);
                SendPackage(value, attribute, route);
            }
            else
            {
                debug += "Inspecting complex object\n";
                //debugger?.Log(debug);
                Inspect(valueType, value, route, owner);
            }
        }

        public void CheckAuthority(int owner, NETAUTHORITY auxAuthority, Action onClientAuthority, Action onServerAuthority)
        {
            if (netAuthority != auxAuthority)
                return;

            if (netAuthority == NETAUTHORITY.CLIENT && owner == networkEntity.clientID)
            {
                onClientAuthority?.Invoke();
            }
            else if (netAuthority == NETAUTHORITY.SERVER)
            {
                onServerAuthority?.Invoke();
            }
        }
        #endregion
    }
}