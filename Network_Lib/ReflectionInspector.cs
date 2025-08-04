using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

namespace Net
{
    public class ReflectionInspector
    {
        private readonly Reflection reflection;

        /// <summary>
        /// Initializes the inspector with a reference to the core Reflection system.
        /// </summary>
        /// <param name="reflection">The main Reflection instance.</param>
        public ReflectionInspector(Reflection reflection)
        {
            this.reflection = reflection;
        }

        #region Update
        /// <summary>
        /// Updates the reflection state by inspecting all network objects owned by this client.
        /// </summary>
        public void UpdateAllFields()
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

                if (netObj.GetTRS() != null && reflection.netAuthority == NETAUTHORITY.SERVER)
                {
                    TRS trs = netObj.GetTRS();
                    NetTRSMessage netTRSMessage = new NetTRSMessage(MessagePriority.Default, trs, idRoute);
                    reflection.networkEntity.SendMessage(netTRSMessage.Serialize());
                }
            }
        }
        #endregion

        #region Inspect
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
                foreach (FieldInfo info in ReflectionHelperMethods.GetAllFields(type, reflection.bindingFlags))
                {
                    NetVariable netVarAttribute = info.GetCustomAttribute<NetVariable>();

                    if (netVarAttribute != null)
                    {
                        debug += "___info field: " + info + "\n";
                        debug += "___info route: " + idRoute[0].route + "\n";
                        //reflection.debugger?.Log(debug);
                        if (netVarAttribute.syncAuthority == reflection.netAuthority)
                        {
                            if (reflection.extensionMethods.TryGetValue(info.FieldType, out MethodInfo methodInfo))
                            {
                                reflection.CheckAuthority(owner, netVarAttribute.syncAuthority, ReadValueEMAction, ReadValueEMAction);
                                void ReadValueEMAction()
                                {
                                    object actualObject = info.GetValue(obj);

                                    object fields = methodInfo.Invoke(null, new object[] { actualObject, netVarAttribute.syncAuthority });
                                    if (fields is List<(FieldInfo, NetVariable)> values)
                                    {
                                        foreach ((FieldInfo, NetVariable) field in values)
                                        {
                                            List<RouteInfo> newRoute = new List<RouteInfo>(idRoute);
                                            newRoute.Add(RouteInfo.CreateForProperty(netVarAttribute.VariableId));
                                            newRoute.Add(RouteInfo.CreateForProperty(field.Item2.VariableId));
                                            object componentValue = field.Item1.GetValue(actualObject);
                                            //reflection.debugger?.Log($"EM Inspect: {info.FieldType} {info.GetValue(obj)}\n");
                                            //reflection.debugger?.Log($"EM Full Route: {string.Join("->", newRoute.Select(r => r.route))}\n");
                                            reflection.reflectionReader.ReadValue(field.Item1, actualObject, field.Item2, newRoute, owner);
                                            info.SetValue(obj, actualObject);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                reflection.CheckAuthority(owner, netVarAttribute.syncAuthority, ReadValueAction, ReadValueAction);
                                void ReadValueAction()
                                {
                                    List<RouteInfo> extendedRoute = new List<RouteInfo>(idRoute);
                                    //reflection.debugger?.Log($"Full Route: {string.Join("->", extendedRoute.Select(r => r.route))}\n");
                                    //reflection.debugger?.Log($"Inspect: {info.FieldType} {info.GetValue(obj)}\n");
                                    reflection.reflectionReader.ReadValue(info, obj, netVarAttribute, extendedRoute, owner);
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
                //reflection.debugger?.Log(debug);
            }
            else
            {
                debug += "Object is NULL";
                //reflection.debugger?.Log(debug);
            }
        }

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
                foreach (FieldInfo info in ReflectionHelperMethods.GetAllFields(type, reflection.bindingFlags))
                {
                    NetVariable attributes = info.GetCustomAttribute<NetVariable>();
                    if (attributes != null)
                    {
                        if (attributes.VariableId == currentRoute.route)
                        {
                            if (reflection.extensionMethods.TryGetValue(info.FieldType, out MethodInfo methodInfo))
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

                                            object modifiedStruct;
                                            if (reflection.extensionMethods.TryGetValue(field.Item1.FieldType, out MethodInfo _))
                                            {
                                                modifiedStruct = InspectExtensionMethod(field.Item1, currentStruct, field.Item2, idRoute, idToRead + 1, value);
                                            }
                                            else
                                            {
                                                modifiedStruct = reflection.reflectionWriter.WriteValue(field.Item1, currentStruct, field.Item2, idRoute, idToRead + 1, value, info, obj);
                                            }

                                            field.Item1.SetValue(currentStruct, modifiedStruct);

                                            return currentStruct;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                object structInstance = info.GetValue(obj);
                                if (structInstance == null)
                                {
                                    structInstance = ReflectionHelperMethods.ConstructObject(info.FieldType, reflection.bindingFlags);
                                    info.SetValue(obj, structInstance);
                                }
                                //debug += $"Found matching field: {info.Name} (Type: {info.FieldType.Name})\n";
                                //debug += $"Current field value: {info.GetValue(obj)}\n";
                                //debugger?.Log(debug);
                                return reflection.reflectionWriter.WriteValue(info, obj, attributes, idRoute, idToRead, value);
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

            foreach (FieldInfo info in ReflectionHelperMethods.GetAllFields(type, reflection.bindingFlags))
            {
                NetVariable attributes = info.GetCustomAttribute<NetVariable>();
                if (attributes != null && attributes.VariableId == currentRoute.route)
                {
                    debug += $"Found matching field: {info.Name}, VariableId: {attributes.VariableId}\n";
                    //debugger?.Log(debug);
                    return reflection.reflectionWriter.WriteValueNullException(info, obj, attributes, idRoute, idToRead, value);
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

            foreach (FieldInfo info in ReflectionHelperMethods.GetAllFields(type, reflection.bindingFlags))
            {
                NetVariable attributes = info.GetCustomAttribute<NetVariable>();
                if (attributes != null && attributes.VariableId == currentRoute.route)
                {
                    debug += $"Found matching field: {info.Name}, VariableId: {attributes.VariableId}\n";
                    //debugger?.Log(debug);
                    return reflection.reflectionWriter.WriteValueNullException(info, obj, attributes, idRoute, idToRead, value);
                }
            }

            debug += "No matching field found\n";
            //debugger?.Log(debug);
            return obj;
        }


        /// <summary>
        /// Recursively inspects and modifies fields of a struct using extension methods for network synchronization.
        /// </summary>
        /// <param name="info">The FieldInfo representing the field to inspect.</param>
        /// <param name="obj">The parent object containing the field.</param>
        /// <param name="nestedAttr">The NetVariable attribute associated with the field.</param>
        /// <param name="idRoute">The route information specifying the path to the target field.</param>
        /// <param name="idToRead">The current position in the route path.</param>
        /// <param name="value">The new value to set for the target field.</param>
        /// <returns>
        /// The modified struct instance if successful, or the original object if no modifications were made.
        /// </returns>
        private object InspectExtensionMethod(FieldInfo info, object obj, NetVariable nestedAttr, List<RouteInfo> idRoute, int idToRead, object? value)
        {
            if (reflection.extensionMethods.TryGetValue(info.FieldType, out MethodInfo methodInfo))
            {
                object structInstance = info.GetValue(obj);
                object fields = methodInfo.Invoke(null, new object[] { structInstance, nestedAttr.syncAuthority });

                if (fields is List<(FieldInfo, NetVariable)> values)
                {
                    foreach ((FieldInfo, NetVariable) field in values)
                    {
                        if (idRoute[idToRead + 1].route == field.Item2.VariableId)
                        {
                            object modifiedSubStruct;

                            if (reflection.extensionMethods.TryGetValue(field.Item1.FieldType, out MethodInfo _))
                            {
                                modifiedSubStruct = InspectExtensionMethod(field.Item1, structInstance, field.Item2, idRoute, idToRead + 1, value);
                            }
                            else
                            {
                                modifiedSubStruct = reflection.reflectionWriter.WriteValue(field.Item1, structInstance, field.Item2, idRoute, idToRead + 1, value, info, obj);
                            }

                            field.Item1.SetValue(structInstance, modifiedSubStruct);
                            return structInstance;
                        }
                    }
                }
            }
            return obj;
        }
        #endregion
    }
}