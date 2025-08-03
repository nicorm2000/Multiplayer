using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System;
using System.Diagnostics;

namespace Net
{
    public class ReflectionWriter
    {
        private readonly Reflection reflection;

        /// <summary>
        /// Initializes the inspector with a reference to the core Reflection system.
        /// </summary>
        /// <param name="reflection">The main Reflection instance.</param>
        public ReflectionWriter(Reflection reflection)
        {
            this.reflection = reflection;
        }

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
            reflection.debugger?.Log($"WriteValue - Field: {info.Name}, ValueType: {value?.GetType().Name}");
            string debug = "";
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
                debug += $"IsSimpleType: true\n";

                if (parentField != null && parentObject != null)
                {
                    debug += "Handling nested field assignment\n";

                    // Get the parent struct instance
                    object parentStruct = parentField.GetValue(parentObject);
                    debug += $"Parent struct before: {parentStruct}\n";

                    // Set the value on the nested field
                    info.SetValue(parentStruct, Convert.ChangeType(value, fieldType));
                    debug += $"Parent struct after: {parentStruct}\n";

                    // Set the modified struct back to its parent
                    parentField.SetValue(parentObject, parentStruct);

                    // If we're multiple levels deep in structs, propagate up
                    if (parentObject != obj)
                    {
                        info.SetValue(obj, parentField.GetValue(parentObject));
                    }

                    debug += "Nested assignment complete\n";
                }
                else
                {
                    debug += "Direct field assignment\n";
                    info.SetValue(obj, Convert.ChangeType(value, fieldType));
                }

                reflection.debugger?.Log(debug);
                return obj;
            }

            if (value is Remove removeData)
            {
                object fieldValue = info.GetValue(obj);
                if (fieldValue == null)
                    return obj;

                if (typeof(IDictionary).IsAssignableFrom(info.FieldType))
                {
                    return reflection.reflectionCollectionHelper.HandleDictionaryRemove(info, obj, removeData.KeyHash);
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
                    return reflection.reflectionInspector.InspectWrite(fieldValue.GetType(), fieldValue, newRoute, idToRead + 1, value);
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
                                element = ReflectionHelperMethods.ConstructObject(fieldType.GetElementType(), reflection.bindingFlags);
                                newArray.SetValue(element, indices);
                            }
                            reflection.reflectionInspector.InspectWrite(element.GetType(), element, idRoute, idToRead + 1, value);
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
                    return reflection.reflectionCollectionHelper.HandleDictionaryWrite(info, obj, attribute, idRoute, idToRead, value);
                }
                else
                {
                    if (currentValue == null)
                    {
                        //debugger?.Log($"[WriteValue] Field '{info.Name}' is null on receiver. Attempting to construct new instance of {fieldType.Name}.");
                        currentValue = ReflectionHelperMethods.ConstructObject(fieldType, reflection.bindingFlags);
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
                        if (reflection.reflectionCollectionHelper.TrySetCollectionIndexValue(currentValue, currentRoute.collectionKey, value))
                        {
                            return obj;
                        }
                    }
                    else
                    {
                        object? nestedElement = reflection.reflectionCollectionHelper.TryGetCollectionIndexValue(currentValue, currentRoute.collectionKey);

                        if (nestedElement == null)
                        {
                            Type nestedElementType = ReflectionHelperMethods.GetElementType(fieldType) ?? typeof(object);
                            nestedElement = ReflectionHelperMethods.ConstructObject(nestedElementType, reflection.bindingFlags);
                            reflection.reflectionCollectionHelper.TrySetCollectionIndexValue(currentValue, currentRoute.collectionKey, nestedElement);
                        }

                        reflection.reflectionInspector.InspectWrite(nestedElement.GetType(), nestedElement, idRoute, idToRead + 1, value);
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
                            element = ReflectionHelperMethods.ConstructObject(ReflectionHelperMethods.GetElementType(fieldType), reflection.bindingFlags);
                            if (fieldType.IsArray)
                            {
                                ((Array)newCollection).SetValue(element, currentRoute.collectionKey);
                            }
                            else if (newCollection is IList list)
                            {
                                list[currentRoute.collectionKey] = element;
                            }
                        }

                        reflection.reflectionInspector.InspectWrite(element.GetType(), element, idRoute, idToRead + 1, value);
                    }
                }

                info.SetValue(obj, newCollection);
                return obj;
            }

            if (reflection.extensionMethods.TryGetValue(fieldType, out MethodInfo methodInfo))
            {
                reflection.debugger?.Log($"WriteValue: Handling extension method type: {fieldType.Name}");

                if (idRoute.Count <= idToRead + 1)
                {
                    reflection.debugger?.Log("WriteValue: No more route segments, can't process extension method");
                    return obj;
                }

                object fields = methodInfo.Invoke(null, new object[] { currentValue, attribute?.syncAuthority ?? NETAUTHORITY.SERVER });

                if (fields is List<(FieldInfo, NetVariable)> nestedFields)
                {
                    RouteInfo nextRoute = idRoute[idToRead + 1];
                    reflection.debugger?.Log($"WriteValue: Next route segment: {nextRoute.route}");

                    foreach ((FieldInfo nestedField, NetVariable nestedAttr) in nestedFields)
                    {
                        if (nestedAttr.VariableId == nextRoute.route)
                        {
                            reflection.debugger?.Log($"WriteValue: Found matching nested field: {nestedField.Name}");

                            // Get current nested value
                            object nestedValue = nestedField.GetValue(currentValue);

                            // Recursively process the nested field
                            object result = WriteValue(
                                nestedField,
                                currentValue,
                                nestedAttr,
                                idRoute,
                                idToRead + 1,
                                value,
                                info,
                                obj
                            );

                            // Propagate struct changes if needed
                            if (fieldType.IsValueType && info != null)
                            {
                                nestedField.SetValue(currentValue, result);
                                info.SetValue(obj, currentValue);
                            }

                            reflection.debugger?.Log("WriteValue: Completed nested field processing");
                            return obj;
                        }
                    }
                }
            }

            object objReference = info.GetValue(obj);
            if (objReference == null)
            {
                objReference = ReflectionHelperMethods.ConstructObject(info.FieldType, reflection.bindingFlags);
            }
            else if (idRoute.Count > idToRead + 1)
            {
                objReference = reflection.reflectionInspector.InspectWrite(info.FieldType, info.GetValue(obj), idRoute, idToRead + 1, value);
            }

            info.SetValue(obj, objReference);
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

                foreach (FieldInfo field in info.FieldType.GetFields(reflection.bindingFlags))
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
    }
}
