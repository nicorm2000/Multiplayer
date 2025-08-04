using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;
using System;

namespace Net
{
    public class ReflectionReader
    {
        private readonly Reflection reflection;

        /// <summary>
        /// Initializes the inspector with a reference to the core Reflection system.
        /// </summary>
        /// <param name="reflection">The main Reflection instance.</param>
        public ReflectionReader(Reflection reflection)
        {
            this.reflection = reflection;
        }

        #region Value Processing
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

            if (value == null || value is Null)
            {
                //reflection.debugger?.Log("Sending NULL package\n");
                reflection.SendPackage(PossibleStates.Null, attribute, route);
                return;
            }

            if (value is IDictionary dict && dict.Count == 0)
            {
                RouteInfo lastRoute = route.Last();
                if (!lastRoute.IsDictionary && route.Count > 1)
                {
                    lastRoute = route[route.Count - 2]; // Nested structures, checks parent to update route
                }

                if (lastRoute.IsDictionary)
                {
                    RouteInfo dictRoute = RouteInfo.CreateForDictionary(lastRoute.route, -1, lastRoute.ElementType ?? typeof(object));

                    route[route.Count - 1] = dictRoute;
                }

                reflection.SendPackage(PossibleStates.Empty, attribute, route);
                return;
            }

            Type valueType = value.GetType();

            if ((valueType.IsValueType && valueType.IsPrimitive) || valueType == typeof(string) || valueType.IsEnum)
            {
                debug += "Sending primitive/string/enum package\n";
                //debugger?.Log(debug);
                reflection.SendPackage(value, attribute, route);
            }
            else
            {
                debug += "Inspecting complex object\n";
                //debugger?.Log(debug);
                reflection.reflectionInspector.Inspect(valueType, value, route, owner);
            }
        }

        /// <summary>
        /// Reads and processes the value of a field, sending appropriate network messages.
        /// </summary>
        /// <param name="info">Field information.</param>
        /// <param name="obj">Parent object containing the field.</param>
        /// <param name="attribute">NetVariable attribute of the field.</param>
        /// <param name="idRoute">Route information for network message routing.</param>
        public void ReadValue(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int owner, bool skipRouteAppend = false)
        {
            string debug = "ReadValue Start - ";
            debug += $"Field: {info.Name}, Type: {info.FieldType}, Current Route: {string.Join("->", idRoute.Select(r => r.route))}\n";
            //reflection.debugger?.Log(debug);

            object fieldValue = info.GetValue(obj);
            Type fieldType = info.FieldType;

            // Handle null case
            if (fieldValue == null || fieldValue is Null)
            {
                //reflection.debugger?.Log("Entered null case");
                idRoute.Add(RouteInfo.CreateForProperty(attribute.VariableId));
                reflection.SendPackage(PossibleStates.Null, attribute, idRoute);
                return;
            }

            // Handle simple types
            if (ReflectionHelperMethods.IsSimpleType(info.FieldType))
            {
                //reflection.debugger?.Log("Simple Type: " + fieldValue + fieldType);
                if (!skipRouteAppend)
                    idRoute.Add(RouteInfo.CreateForProperty(attribute.VariableId));
                //reflection.debugger?.Log($"SimpleType Full Route: {string.Join("->", idRoute.Select(r => r.route))}\n");
                reflection.SendPackage(fieldValue, attribute, idRoute);
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
                    // genericArgs[0] = typeof(string) (key type)
                    // genericArgs[1] = typeof(int)   (value type)

                    List<object> currentKeys = dictionary.Keys.Cast<object>().ToList();
                    debug += ($"Current Keys: {string.Join(",", currentKeys)}\n");

                    if (reflection.previousDictionaryStates.TryGetValue(dictionary, out Dictionary<object, int>? previousKeys)) // Check for removals FIRST
                    {
                        List<object> removedKeys = previousKeys.Keys.Except(currentKeys).ToList();
                        debug += ($"Removed Keys: {(removedKeys.Any() ? string.Join(",", removedKeys) : "none")}\n");

                        if (removedKeys.Any()) // Process removals FIRST and RETURN
                        {
                            foreach (object? key in removedKeys)
                            {
                                int keyHash = ReflectionHelperMethods.GetStableKeyHash(key);
                                debug += ($"Sending Remove for Key: {key} (Hash: {keyHash})\n");

                                List<RouteInfo> removeRoute = new List<RouteInfo>(idRoute)
                                {
                                    RouteInfo.CreateForDictionary(attribute.VariableId, keyHash, valueType)
                                };

                                NetRemoveMessage removeMessage = new NetRemoveMessage(attribute.MessagePriority, keyHash, removeRoute);

                                byte[] serialized = removeMessage.Serialize();
                                //debugger?.Log($"Sending Remove - Full Data: {BitConverter.ToString(serialized)}");
                                reflection.networkEntity.SendMessage(serialized);
                            }

                            // Update state and RETURN after processing removals
                            reflection.previousDictionaryStates[dictionary] = currentKeys.ToDictionary(k => k, ReflectionHelperMethods.GetStableKeyHash);
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
                        reflection.SendPackage(PossibleStates.Empty, attribute, new List<RouteInfo>(idRoute)
                        {
                            RouteInfo.CreateForDictionary(attribute.VariableId, -1, valueType)
                        });
                    }

                    // Update state
                    reflection.previousDictionaryStates[dictionary] = currentKeys.ToDictionary(k => k, ReflectionHelperMethods.GetStableKeyHash);

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
 
                    IEnumerator enumerator = collection.GetEnumerator(); // Get count via enumeration (works for any IEnumerable)
                    while (enumerator.MoveNext())
                    {
                        count++;
                    }

                    int previousCount = -1;
                    if (reflection.previousCollectionCounts.TryGetValue(fieldValue, out previousCount))
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
                                reflection.networkEntity.SendMessage(serialized);
                            }
                        }
                    }

                    reflection.previousCollectionCounts[fieldValue] = count;

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
                        reflection.SendPackage(PossibleStates.Empty, attribute, idRoute);
                    }
                }
            }

            if (reflection.extensionMethods.TryGetValue(info.FieldType, out MethodInfo methodInfo))
            {
                reflection.CheckAuthority(owner, attribute.syncAuthority, ReadValueEMAction, ReadValueEMAction);
                void ReadValueEMAction()
                {
                    object actualObject = info.GetValue(obj);

                    object fields = methodInfo.Invoke(null, new object[] { actualObject, attribute.syncAuthority });
                    if (fields is List<(FieldInfo, NetVariable)> values)
                    {
                        foreach ((FieldInfo, NetVariable) field in values)
                        {
                            List<RouteInfo> newRoute = new List<RouteInfo>(idRoute);
                            newRoute.Add(RouteInfo.CreateForProperty(field.Item2.VariableId));
                            object componentValue = field.Item1.GetValue(actualObject);
                            //reflection.debugger?.Log($"EM RV: {info.FieldType} {info.GetValue(obj)}\n");
                            //reflection.debugger?.Log($"EM Full Route: {string.Join("->", newRoute.Select(r => r.route))}\n");
                            reflection.reflectionReader.ReadValue(field.Item1, actualObject, field.Item2, newRoute, owner, true);
                        }
                        return;
                    }
                }
                return;
            }

            // Handle complex objects
            //debugger?.Log("Handling complex object type\n");
            idRoute.Add(RouteInfo.CreateForProperty(attribute.VariableId));
            //debugger?.Log("Complex object: " + fieldValue + fieldType);
            //debugger?.Log($"Full Route Read: {string.Join("->", idRoute.Select(r => r.route))}\n");
            reflection.reflectionInspector.Inspect(fieldType, fieldValue, idRoute, owner);
        }
        #endregion
    }
}