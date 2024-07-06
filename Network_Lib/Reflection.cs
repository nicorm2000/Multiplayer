﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Net
{
    public class Reflection
    {
        BindingFlags bindingFlags;
        Assembly executeAssembly;

        public Action<string> consoleDebugger;
        public Action consoleDebuggerPause;
            
        NetworkEntity networkEntity;

        public Reflection(NetworkEntity entity)
        {
            networkEntity = entity;
            networkEntity.OnReceivedMessage += OnReceivedReflectionMessage;

            executeAssembly = Assembly.GetExecutingAssembly();

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
                        new RouteInfo(netObj.GetID())
                    };
                    Inspect(netObj.GetType(), netObj, idRoute);
                }
            }
        }

        public void Inspect(Type type, object obj, List<RouteInfo> idRoute)
        {
            if (obj != null)
            {
                foreach (FieldInfo info in type.GetFields(bindingFlags))
                {
                    IEnumerable<Attribute> attributes = info.GetCustomAttributes();
                    foreach (Attribute attribute in attributes)
                    {
                        if (attribute is NetVariable)
                        {
                            consoleDebugger.Invoke($"Inspect: {type} - {obj} - {info} - {info.GetType()}");
                            ReadValue(info, obj, (NetVariable)attribute, new List<RouteInfo>(idRoute));
                        }
                    }
                    if (type.BaseType != null)
                    {
                        Inspect(type.BaseType, obj, new List<RouteInfo>(idRoute));
                    }
                }
            }
        }

        public void ReadValue(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute)
        {
            if ((info.FieldType.IsValueType && info.FieldType.IsPrimitive) || info.FieldType == typeof(string) || info.FieldType.IsEnum) //TODO: Chequear esto a futuro
            {
                idRoute.Add(new RouteInfo(attribute.VariableId));

                string debug = "";
                debug += "Read values from Root Player (Owner: " + NetObjFactory.GetINetObject(idRoute[0].route).GetOwnerID().ToString() + ") \n";
                debug += "Se modifica la variable " + info + " que tiene un valor de " + info.GetValue(obj) + "\n";

                debug += "La ruta de la variable es: ";
                foreach (RouteInfo item in idRoute)
                {
                    debug += item + " - ";
                }

                consoleDebugger.Invoke(debug);

                SendPackage(info, obj, attribute, idRoute);
            }
            else if (typeof(System.Collections.ICollection).IsAssignableFrom(info.FieldType))
            {
                int index = 0;
                
                foreach (object elementOfCollection in (info.GetValue(obj) as System.Collections.ICollection))
                {
                    idRoute.Add(new RouteInfo(attribute.VariableId, index));
                    consoleDebugger.Invoke("Object: " + elementOfCollection.GetType().GetField("value") + " - " + info + " - " + elementOfCollection + ", " + elementOfCollection.GetType() + ", " + info.GetValue(obj));

                    foreach (FieldInfo item in elementOfCollection.GetType().GetFields())
                    {
                        consoleDebugger.Invoke("Item: " + item.Name + "\n");
                    }

                    //Inspect(elementOfCollection.GetType(), elementOfCollection, idRoute); //TODO: ver qe onda las colleciones, tiene qe agregarse a idRoute
                    //ReadValue(info.GetValue(elementOfCollection), obj, attribute, new List<RouteInfo>(idRoute));
                    index++;
                }
            }
            else
            {
                idRoute.Add(new RouteInfo(attribute.VariableId));
                Inspect(info.FieldType, info.GetValue(obj), idRoute);
            }
        }

        public void SendPackage(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute)
        {
            Type packageType = info.GetValue(obj).GetType();

            string debug = "";
            foreach (Type type in executeAssembly.GetTypes())
            {
                if (type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(BaseReflectionMessage<>))
                {
                    Type[] genericTypes = type.BaseType.GetGenericArguments();
                    foreach (Type arg in genericTypes)
                    {
                        if (packageType == arg)
                        {
                            Type[] parametersToApply = { typeof(MessagePriority), packageType, typeof(List<RouteInfo>) };

                            object[] parameters = new[] { attribute.MessagePriority, info.GetValue(obj), idRoute };

                            ConstructorInfo? ctor = type.GetConstructor(parametersToApply);
                            if (ctor != null)
                            {
                                object message = ctor.Invoke(parameters);
                                ParentBaseMessage a = message as ParentBaseMessage;
                                networkEntity.SendMessage(a.Serialize());

                                debug += "SEND PACKAGE Root Player (Owner: " + NetObjFactory.GetINetObject(idRoute[0].route).GetOwnerID().ToString() + ") \n";
                                debug += "Se modifica la variable " + info + " que tiene un valor de " + info.GetValue(obj) + "\n";
                                debug += "El tipo de mensajes " + MessageChecker.CheckMessageType(a.Serialize()) + "\n";
                                debug += "Al constructor se le paso: " + attribute.MessagePriority + " - " + info.GetValue(obj) + " - La ruta " + "\n";

                                debug += "La ruta de la variable es: ";
                                foreach (RouteInfo item in idRoute)
                                {
                                    debug += item + " - ";
                                }

                                //consoleDebugger.Invoke(debug);
                            }
                        }
                    }
                }
            }
        }

        public void OnReceivedReflectionMessage(byte[] data, IPEndPoint ip)
        {
            //DeserializeReflectionMessage(data);

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
                    string debug = "Message header size value:" + netFloatMessage.GetMessageHeaderSize() + "\n";
                    debug += "Message deserialize value:" + netFloatMessage.Deserialize(data) + "\n";
                    consoleDebugger.Invoke(debug);
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
            }
        }

        void DeserializeReflectionMessage(byte[] data)
        {
            MessageType messageType = MessageChecker.CheckMessageType(data);  //Por reflection hay qe obtener todos los tipos de mensajes y creo el tipo de mensaje que coincida con getType

            foreach (Type type in executeAssembly.GetTypes())
            {
                if (type.BaseType != null && type.BaseType.IsGenericType && type.BaseType.GetGenericTypeDefinition() == typeof(BaseReflectionMessage<>))
                {
                    NetMessageClass attribute = type.GetCustomAttribute<NetMessageClass>();

                    if (attribute.MessageType == messageType)
                    {
                        Type[] parametersToApply = { typeof(byte[]) };

                        object[] parameters = new[] { data };

                        ConstructorInfo? ctor = attribute.GetType().GetConstructor(parametersToApply); //Attribute.GetType me da la clase que necesito,

                        if (ctor != null)
                        {
                            object message = ctor.Invoke(parameters);
                            //ParentBaseMessage a = message as ParentBaseMessage;

                            consoleDebugger.Invoke("Se creo el Message " + message.ToString());
                            CastToCorrectMessage(message, data);
                        }
                    }
                }
            }
        }

        void CastToCorrectMessage(object obj, byte[] data)
        {
            //if (obj.GetType() == typeof(NetFloatMessage))
            //{
            //    consoleDebugger.Invoke("Se casteo " + obj.GetType());
            //
            //    NetFloatMessage netFloatMessage = new NetFloatMessage(data);
            //    VariableMapping(netFloatMessage.GetMessageRoute(), netFloatMessage.GetData());
            //}
        }

        void VariableMapping(List<RouteInfo> route, object variableValue)
        {
            if (route.Count > 0)
            {
                INetObj objectRoot = NetObjFactory.GetINetObject(route[0].route);

                string debug = "Variable Mapping obejct root: " + objectRoot + "\n";

                if (objectRoot.GetOwnerID() != networkEntity.clientID)
                {
                    debug += "Variable Mapping obejct root owner ID distinto del clientID\n";
                    debug += "Variable Mapping variable value:" + variableValue + "\n";
                    consoleDebugger.Invoke(debug);
                    InspectWrite(objectRoot.GetType(), objectRoot, route, 1, variableValue);
                }
            }
        }

        public void InspectWrite(Type type, object obj, List<RouteInfo> idRoute, int idToRead, object value)
        {
            string debug = "";
            if (obj != null)
            {
                foreach (FieldInfo info in type.GetFields(bindingFlags))
                {
                    IEnumerable<Attribute> attributes = info.GetCustomAttributes();
                    foreach (Attribute attribute in attributes)
                    {
                        if (attribute is NetVariable && ((NetVariable)attribute).VariableId == idRoute[idToRead].route)
                        {
                            debug += "Inspect write: " + obj + "\n";
                            debug += "Inspect write attribute: " + attribute + "\n";
                            debug += "Inspect write route: " + idRoute[idToRead].route + "\n";
                            debug += "Inspect write variable ID: " + ((NetVariable)attribute).VariableId + "\n";
                            debug += "Inspect write value: " + value + "\n";
                            consoleDebugger.Invoke(debug);

                            WriteValue(info, obj, (NetVariable)attribute, idRoute, idToRead, value);
                            break;
                        }
                    }

                    ///if (type.BaseType != null)
                    ///{
                    ///    InspectWrite(type.BaseType, obj, idRoute, idToRead++, value);
                    ///}
                }
            }
        }

        void WriteValue(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int idToRead, object value)
        {
            if (info.FieldType.IsValueType || info.FieldType == typeof(string) || info.FieldType.IsEnum)
            {
                string debug = "";
                debug += "Write values from Root Player (Owner: " + NetObjFactory.GetINetObject(idRoute[0].route).GetOwnerID().ToString() + ") \n";
                debug += "Se modifica la variable " + info + " que tiene un valor de " + info.GetValue(obj) + ". El nuevo valor a asignar es: " + value + "\n";

                debug += "La ruta de la variable es: ";
                foreach (RouteInfo item in idRoute)
                {
                    debug += item + " - ";
                }

                consoleDebugger.Invoke(debug);
                info.SetValue(obj, value);
            }
            else if (typeof(System.Collections.ICollection).IsAssignableFrom(info.FieldType))
            {
                foreach (object item in (info.GetValue(obj) as System.Collections.ICollection))
                {
                    InspectWrite(item.GetType(), item, idRoute, idToRead, value);
                }
            }
            else
            {
                idToRead++;
                InspectWrite(info.FieldType, info.GetValue(obj), idRoute, idToRead, value);
            }
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
}