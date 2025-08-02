using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

namespace Net
{
    public class ReflectionMapping
    {
        private readonly Reflection reflection;

        /// <summary>
        /// Initializes the inspector with a reference to the core Reflection system.
        /// </summary>
        /// <param name="reflection">The main Reflection instance.</param>
        public ReflectionMapping(Reflection reflection)
        {
            this.reflection = reflection;
        }

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
                debug += $"NetworkEntity ClientID: {reflection.networkEntity.clientID}\n";
                debug += "Proceeding with write operation\n";
                //object a = NetObjFactory.GetObject(route[0].route);
                reflection.reflectionInspector.InspectWrite(objectRoot.GetType(), objectRoot, route, 1, variableValue);
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

            debug += $"Root Object: {objectRoot.GetType().Name}, OwnerID: {objectRoot.GetOwnerID()}, NetworkEntity ClientID: {reflection.networkEntity.clientID}\n";

            debug += "Processing write operation for null exception\n";
            //debugger?.Log(debug);
            reflection.reflectionInspector.InspectWriteNullException(objectRoot.GetType(), objectRoot, route, 1, variableValue);
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

            debug += $"Root Object: {objectRoot.GetType().Name}, OwnerID: {objectRoot.GetOwnerID()}, NetworkEntity ClientID: {reflection.networkEntity.clientID}\n";

            debug += "Processing empty collection\n";
            reflection.reflectionInspector.InspectWriteEmpty(objectRoot.GetType(), objectRoot, route, 1, variableValue);
            //debugger?.Log(debug);
        }

        /// <summary>
        /// Handles TRS mapping of every INetObj synced.
        /// </summary>
        /// <param name="route">Route information for the TRS.</param>
        /// <param name="data">The TRS data.</param>
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
    }
}