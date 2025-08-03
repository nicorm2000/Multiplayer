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
                    NetVariable netVarAux = info.GetCustomAttribute<NetVariable>();

                    if (netVarAux != null)
                    {
                        debug += "___info field: " + info + "\n";
                        debug += "___info route: " + idRoute[0].route + "\n";
                        //reflection.debugger?.Log(debug);
                        if (netVarAux.syncAuthority == reflection.netAuthority)
                        {
                            if (reflection.extensionMethods.TryGetValue(info.FieldType, out MethodInfo methodInfo))
                            {
                                reflection.CheckAuthority(owner, netVarAux.syncAuthority, ReadValueEMAction, ReadValueEMAction);
                                void ReadValueEMAction()
                                {
                                    object actualObject = info.GetValue(obj);

                                    object fields = methodInfo.Invoke(null, new object[] { actualObject, netVarAux.syncAuthority });
                                    if (fields is List<(FieldInfo, NetVariable)> values)
                                    {
                                        foreach ((FieldInfo, NetVariable) field in values)
                                        {
                                            List<RouteInfo> newRoute = new List<RouteInfo>(idRoute);
                                            newRoute.Add(RouteInfo.CreateForProperty(netVarAux.VariableId));
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
                                reflection.CheckAuthority(owner, netVarAux.syncAuthority, ReadValueAction, ReadValueAction);
                                void ReadValueAction()
                                {
                                    List<RouteInfo> extendedRoute = new List<RouteInfo>(idRoute);
                                    //reflection.debugger?.Log($"Full Route: {string.Join("->", extendedRoute.Select(r => r.route))}\n");
                                    //reflection.debugger?.Log($"Inspect: {info.FieldType} {info.GetValue(obj)}\n");
                                    reflection.reflectionReader.ReadValue(info, obj, netVarAux, extendedRoute, owner);
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
        public object InspectWrite(Type type, object obj, List<RouteInfo> idRoute, int idToRead, object value, FieldInfo parentField = null, object parentObject = null)
        {
            string debug = $"[InspectWrite] Start - Type:{type.Name} RoutePos:{idToRead}/{idRoute.Count}\n";
            debug += $"Full Route: {string.Join("->", idRoute.Select(r => r.route))}\n";
            debug += $"Current Value: {obj}\n";
            debug += $"New Value: {value} (Type: {value?.GetType()?.Name ?? "null"})\n";

            try
            {
                if (obj == null)
                {
                    debug += "Target object is null\n";
                    reflection.debugger?.Log(debug);
                    return null;
                }

                if (idRoute.Count <= idToRead)
                {
                    debug += "Route exhausted without finding target\n";
                    reflection.debugger?.Log(debug);
                    return obj;
                }

                RouteInfo currentRoute = idRoute[idToRead];
                debug += $"Current Route: {currentRoute.route}\n";

                foreach (FieldInfo info in ReflectionHelperMethods.GetAllFields(type, reflection.bindingFlags))
                {
                    NetVariable attributes = info.GetCustomAttribute<NetVariable>();
                    if (attributes != null && attributes.VariableId == currentRoute.route)
                    {
                        debug += $"Found matching field: {info.Name} (Type: {info.FieldType.Name})\n";

                        if (reflection.extensionMethods.TryGetValue(info.FieldType, out MethodInfo methodInfo))
                        {
                            debug += $"Processing as extension method type\n";

                            object structInstance = info.GetValue(obj);
                            if (structInstance == null)
                            {
                                structInstance = ReflectionHelperMethods.ConstructObject(info.FieldType, reflection.bindingFlags);
                                info.SetValue(obj, structInstance);
                            }

                            // Get all nested fields through extension method
                            object fields = methodInfo.Invoke(null, new object[] { structInstance, attributes.syncAuthority });

                            if (fields is List<(FieldInfo, NetVariable)> nestedFields)
                            {
                                debug += $"Found {nestedFields.Count} nested fields\n";

                                if (idRoute.Count <= idToRead + 1)
                                {
                                    debug += "No more route segments for nested fields\n";
                                    reflection.debugger?.Log(debug);
                                    return obj;
                                }

                                RouteInfo nextRoute = idRoute[idToRead + 1];
                                debug += $"Next route segment: {nextRoute.route}\n";

                                foreach ((FieldInfo nestedField, NetVariable nestedAttr) in nestedFields)
                                {
                                    if (nestedAttr.VariableId == nextRoute.route)
                                    {
                                        debug += $"Found matching nested field: {nestedField.Name}\n";

                                        // Always route through WriteValue to handle recursive extension methods
                                        object result = reflection.reflectionWriter.WriteValue(
                                            nestedField,
                                            structInstance,
                                            nestedAttr,
                                            idRoute,
                                            idToRead + 1,
                                            value,
                                            info,
                                            obj
                                        );

                                        // Propagate struct changes if needed
                                        if (info.FieldType.IsValueType)
                                        {
                                            nestedField.SetValue(structInstance, result);
                                            info.SetValue(obj, structInstance);
                                        }

                                        reflection.debugger?.Log(debug);
                                        return obj;
                                    }
                                }
                            }
                        }
                        else
                        {
                            debug += $"Processing as regular field\n";
                            reflection.debugger?.Log(debug);

                            return reflection.reflectionWriter.WriteValue(
                                info,
                                obj,
                                attributes,
                                idRoute,
                                idToRead,
                                value,
                                parentField,
                                parentObject
                            );
                        }
                    }
                }

                debug += "No matching field found\n";
            }
            catch (Exception ex)
            {
                debug += $"ERROR: {ex.Message}\n{ex.StackTrace}\n";
            }

            reflection.debugger?.Log(debug);
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
        #endregion
    }
}