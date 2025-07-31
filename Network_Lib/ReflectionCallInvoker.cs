using System.Collections.Generic;
using Network_Lib.BasicMessages;
using System.ComponentModel;
using System.Reflection;
using System;

namespace Net
{
    public class ReflectionCallInvoker
    {
        private readonly Reflection reflection;

        public ReflectionCallInvoker(Reflection reflection)
        {
            this.reflection = reflection;
        }

        #region Methods
        /// <summary>
        /// Sends a method invocation message over the network.
        /// </summary>
        /// <param name="iNetObj">The network object containing the method.</param>
        /// <param name="methodName">The name of the method to invoke.</param>
        /// <param name="parameters">The parameters to pass to the method.</param>
        /// <returns>The method's return value, or null for void methods.</returns>
        public object SendMethodMessage(INetObj iNetObj, string methodName, params object[] parameters)
        {
            string debug = "";
            object objectToReturn = null;

            MethodInfo method = iNetObj.GetType().GetMethod(methodName, reflection.bindingFlags);
            NetMethod netmethod = method.GetCustomAttribute<NetMethod>();

            if (netmethod != null)
            {
                reflection.CheckAuthority(iNetObj.GetOwnerID(), netmethod.syncAuthority, SendMethodMessageAction, SendMethodMessageAction);
            }

            void SendMethodMessageAction()
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
                //reflection.debugger?.Log(debug);

                NetMethodMessage messageToSend = new NetMethodMessage(MessagePriority.Default, messageData, idRoute);
                reflection.networkEntity.SendMessage(messageToSend.Serialize());
            }
            return objectToReturn;
        }
        /// <summary>
        /// Invokes a method on a network object when a method message is received.
        /// </summary>
        /// <param name="id">The method ID from the NetMethod attribute.</param>
        /// <param name="parameters">List of parameter type-value pairs.</param>
        /// <param name="objectId">The ID of the target network object.</param>
        public void InvokeReflectionMethod(int id, List<(string, string)> parameters, int objectId)
        {
            foreach (INetObj netObj in NetObjFactory.NetObjects())
            {
                List<object> parametersToApply = new List<object>();

                foreach ((string, string) param in parameters)
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(Type.GetType(param.Item1));
                    object parameterValue = converter.ConvertFromInvariantString(param.Item2);
                    parametersToApply.Add(parameterValue);
                }
                int length = netObj.GetType().GetMethods(reflection.bindingFlags).Length;

                for (int i = 0; i < length; i++)
                {
                    MethodInfo method = netObj.GetType().GetMethods(reflection.bindingFlags)[i];

                    NetMethod netMethod = method.GetCustomAttribute<NetMethod>();

                    if (netMethod != null && netMethod.MethodId == id)
                    {
                        object[] objectParameters = parametersToApply.ToArray();
                        object invokeMethod = method.Invoke(netObj, objectParameters);
                    }
                }
                //reflection.debugger?.Log("Invoked reflection method");
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Sends a C# event invocation message from the owner of a network object to all other clients.
        /// </summary>
        /// <param name="iNetObj">The network object that owns the event.</param>
        /// <param name="eventName">The name of the event to invoke (e.g. "OnEventX").</param>
        /// <param name="parameters">Optional parameters to serialize and send to listeners.</param>
        public void SendCSharpEventMessage(INetObj iNetObj, string eventName, params object[] parameters)
        {
            EventInfo eventInfo = iNetObj.GetType().GetEvent(eventName, reflection.bindingFlags);
            if (eventInfo == null) return;

            NetEvent netEvent = eventInfo.GetCustomAttribute<NetEvent>();
            if (netEvent == null) return;

            // Then check authority for network sending
            reflection.CheckAuthority(iNetObj.GetOwnerID(), netEvent.syncAuthority, SendEventMessageAction, SendEventMessageAction);

            void SendEventMessageAction()
            {
                string backingFieldName = netEvent.BackingFieldName ?? $"on{eventInfo.Name.Substring(2)}";
                FieldInfo field = iNetObj.GetType().GetField(backingFieldName, reflection.bindingFlags);
                if (field != null)
                {
                    Delegate eventDelegate = field.GetValue(iNetObj) as Delegate;
                    eventDelegate?.DynamicInvoke(parameters);
                }
                // Serialize and send the network message
                List<(string, string)> parametersList = new List<(string, string)>();
                foreach (object param in parameters)
                {
                    parametersList.Add((param.GetType().ToString(), param.ToString()));
                }

                (int EventId, List<(string, string)> parametersList) messageData = (netEvent.EventId, parametersList);
                List<RouteInfo> idRoute = new List<RouteInfo> { new RouteInfo(iNetObj.GetID()) };

                NetEventMessage messageToSend = new NetEventMessage(netEvent.MessagePriority, messageData, idRoute);
                reflection.networkEntity.SendMessage(messageToSend.Serialize());
            }
        }

        /// <summary>
        /// Invokes a previously declared C# event on a target network object by matching its event ID.
        /// </summary>
        /// <param name="eventId">The unique event ID specified in the NetEvent attribute.</param>
        /// <param name="parameters">Serialized parameter values sent from the sender.</param>
        /// <param name="objectId">The target network object ID whose event should be invoked.</param>
        public void InvokeCSharpEvent(int eventId, List<(string, string)> parameters, int objectId)
        {
            foreach (INetObj netObj in NetObjFactory.NetObjects())
            {
                Type targetType = netObj.GetType();
                EventInfo[] events = targetType.GetEvents(reflection.bindingFlags);

                if (netObj.GetID() != objectId)
                    continue;

                foreach (EventInfo evt in events)
                {
                    NetEvent netEventAttr = evt.GetCustomAttribute<NetEvent>();
                    if (netEventAttr == null || netEventAttr.EventId != eventId)
                        continue;
                    ExecuteEvent(parameters, netObj, targetType, evt, netEventAttr);
                }
            }

            void ExecuteEvent(List<(string, string)> parameters, INetObj netObj, Type targetType, EventInfo evt, NetEvent netEventAttr)
            {
                string backingFieldName = netEventAttr.BackingFieldName ?? $"on{evt.Name.Substring(2)}";
                FieldInfo field = targetType.GetField(backingFieldName, reflection.bindingFlags);
                if (field == null) return;

                Delegate eventDelegate = field.GetValue(netObj) as Delegate;
                if (eventDelegate == null) return;

                List<object> parsedParameters = new List<object>();
                for (int i = 0; i < parameters.Count; i++)
                {
                    Type type = Type.GetType(parameters[i].Item1);
                    if (type == null) continue;

                    TypeConverter converter = TypeDescriptor.GetConverter(type);
                    parsedParameters.Add(converter.ConvertFromInvariantString(parameters[i].Item2));
                }

                eventDelegate.DynamicInvoke(parsedParameters.ToArray());
            }
        }
        #endregion
    }
}
