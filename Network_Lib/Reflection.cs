﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

                //consoleDebugger.Invoke(debug);

                SendPackage(info.GetValue(obj), attribute, idRoute);//PASO DIRECTO EL INFO.GETVALUE(OBJ) EN VEZ DE EL INFO Y EL OBJ CONTENEDOR PORQUE NO OPUEDO SACAR EL ILEDO INO DE UNA COLECCION, A SU VEZ ES AL PEDO PASARSE EL FIELD INFO PORQUE LO PUEDO SACAR ACA
            }
            else if (typeof(System.Collections.ICollection).IsAssignableFrom(info.FieldType))
            {
                int index = 0;

                foreach (object elementOfCollection in (ICollection)info.GetValue(obj))
                {
                    string debug = "";
                    idRoute.Add(new RouteInfo(attribute.VariableId, index++));
                    Type elementType = elementOfCollection.GetType();
                    debug += "Collection info field type " + info.FieldType.ToString() + ") \n";
                    debug += "Collection element value " + elementType.IsValueType + ") \n";
                    debug += "Collection info field type primtive " + elementType.IsPrimitive + ") \n";
                    debug += "Collection info field type enum " + elementType.IsEnum + ") \n";
                    //consoleDebugger.Invoke(debug);
                    if ((elementType.IsValueType && elementType.IsPrimitive) || elementType == typeof(string) || elementType.IsEnum)
                    {
                        debug += "Collections Read values from Root Player (Owner: " + NetObjFactory.GetINetObject(idRoute[0].route).GetOwnerID().ToString() + ") \n";
                        debug += "Se modifica la variable " + elementType + " que tiene un valor de " + elementOfCollection + "\n";
                        debug += "La ruta de la variable es: ";
                        foreach (RouteInfo item in idRoute)
                        {
                            debug += item + " - ";
                        }

                        //consoleDebugger.Invoke(debug);

                        SendPackage(elementOfCollection, attribute, idRoute);//PASO DIRECTO EL ELEMENTOFCOLLECTION PORQUE YA ES EL VALOR DE LA COLECCION
                        idRoute.RemoveAt(idRoute.Count - 1);
                    }
                    else
                    {
                        Inspect(info.FieldType, info.GetValue(obj), idRoute);
                    }
                }
            }
            else
            {
                idRoute.Add(new RouteInfo(attribute.VariableId));
                Inspect(info.FieldType, info.GetValue(obj), idRoute);
            }
        }

        public void SendPackage(object value, NetVariable attribute, List<RouteInfo> idRoute)
        {
            Type packageType = value.GetType();

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

                            object[] parameters = new[] { attribute.MessagePriority, value, idRoute };

                            ConstructorInfo? ctor = type.GetConstructor(parametersToApply);
                            if (ctor != null)
                            {
                                object message = ctor.Invoke(parameters);
                                ParentBaseMessage a = message as ParentBaseMessage;
                                networkEntity.SendMessage(a.Serialize());

                                debug += "SEND PACKAGE Root Player (Owner: " + NetObjFactory.GetINetObject(idRoute[0].route).GetOwnerID().ToString() + ") \n";
                                debug += "Se modifica la variable " + value.GetType() + " que tiene un valor de " + value + "\n";
                                debug += "El tipo de mensajes " + MessageChecker.CheckMessageType(a.Serialize()) + "\n";
                                debug += "Al constructor se le paso: " + attribute.MessagePriority + " - " + value + " - La ruta " + "\n";

                                debug += "La ruta de la variable es: ";
                                foreach (RouteInfo item in idRoute)
                                {
                                    debug += item + " - ";
                                }

                                consoleDebugger.Invoke(debug);
                            }
                        }
                    }
                }
            }
        }

        public void OnReceivedReflectionMessage(byte[] data, IPEndPoint ip)
        {
            //DeserializeReflectionMessage(data);
            string debug = "";
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
                    //consoleDebugger.Invoke(debug);
                    objectRoot = (INetObj)InspectWrite(objectRoot.GetType(), objectRoot, route, 1, variableValue);
                }
            }
        }

        public object InspectWrite(Type type, object obj, List<RouteInfo> idRoute, int idToRead, object value)
        {
            string debug = "";
            debug += "Inspect write value: " + value + "\n";
            //consoleDebugger.Invoke(debug);
            if (obj != null)
            {
                foreach (FieldInfo info in type.GetFields(bindingFlags))
                {
                    NetVariable attributes = info.GetCustomAttribute<NetVariable>();
                    if (attributes != null && attributes.VariableId == idRoute[idToRead].route)
                    {
                        debug += "Inspect write: " + obj + "\n";
                        debug += "Inspect write route: " + idRoute[idToRead].route + "\n";
                        debug += "Inspect write variable ID: " + attributes.VariableId + "\n";
                        //consoleDebugger.Invoke(debug);

                        obj = WriteValue(info, obj, attributes, idRoute, idToRead, value);
                        break;
                    }
                }
            }

            return obj;
        }

        object WriteValue(FieldInfo info, object obj, NetVariable attribute, List<RouteInfo> idRoute, int idToRead, object value)
        {
            if ((info.FieldType.IsValueType && info.FieldType.IsPrimitive) || info.FieldType == typeof(string) || info.FieldType.IsEnum)
            {
                string debug = "";
                debug += "Write values from Root Player (Owner: " + NetObjFactory.GetINetObject(idRoute[0].route).GetOwnerID().ToString() + ") \n";
                debug += "Se modifica la variable " + info + " que tiene un valor de " + info.GetValue(obj) + ". El nuevo valor a asignar es: " + value + "\n";

                debug += "La ruta de la variable es: ";
                foreach (RouteInfo item in idRoute)
                {
                    debug += item + " - ";
                }

                //consoleDebugger.Invoke(debug);
                info.SetValue(obj, value);
            }
            else if (typeof(System.Collections.ICollection).IsAssignableFrom(info.FieldType))
            {
                if (info.FieldType.IsArray)
                {
                    object objectReference = info.GetValue(obj);
                    object[] arrayCopyCollection = new object[((ICollection)info.GetValue(obj)).Count];
                    ((ICollection)info.GetValue(obj)).CopyTo(arrayCopyCollection, 0);

                    for (int i = 0; i < arrayCopyCollection.Length; i++)
                    {
                        if (idRoute[idToRead].collectionIndex != i)
                        {
                            continue;
                        }

                        if ((arrayCopyCollection[i].GetType().IsValueType && arrayCopyCollection[i].GetType().IsPrimitive) || arrayCopyCollection[i].GetType() == typeof(string) || arrayCopyCollection[i].GetType().IsEnum)
                        {
                            arrayCopyCollection[i] = value;
                        }
                        else
                        {
                            object item = InspectWrite(arrayCopyCollection[i].GetType(), arrayCopyCollection[i], idRoute, idToRead + 1, value);

                            arrayCopyCollection[i] = item;
                        }
                    }

                    object arrayAsGeneric = typeof(Reflection).GetMethod(nameof(TranslateArray), bindingFlags).MakeGenericMethod(info.FieldType.GetElementType()).Invoke(this, new[] { arrayCopyCollection });
                    object goNext = Array.CreateInstance(info.FieldType.GetElementType(), ((Array)arrayAsGeneric).Length);
                    Array.Copy((Array)arrayAsGeneric, (Array)goNext, (arrayAsGeneric as ICollection).Count);
                    info.SetValue(obj, goNext);
                }
                else
                {
                    object objectReference = info.GetValue(obj);
                    object[] arrayCopyCollection = new object[((ICollection)info.GetValue(obj)).Count];
                    ((ICollection)info.GetValue(obj)).CopyTo(arrayCopyCollection, 0);

                    for (int i = 0; i < arrayCopyCollection.Length; i++)
                    {
                        if (idRoute[idToRead].collectionIndex != i)
                        {
                            continue;
                        }

                        if ((arrayCopyCollection[i].GetType().IsValueType && arrayCopyCollection[i].GetType().IsPrimitive) || arrayCopyCollection[i].GetType() == typeof(string) || arrayCopyCollection[i].GetType().IsEnum)
                        {
                            arrayCopyCollection[i] = value;
                        }
                        else
                        {
                            object item = InspectWrite(arrayCopyCollection[i].GetType(), arrayCopyCollection[i], idRoute, idToRead + 1, value);

                            arrayCopyCollection[i] = item;
                        }
                    }
                    object genericObjects = typeof(Reflection).GetMethod(nameof(TranslateICollection), bindingFlags).MakeGenericMethod(info.FieldType.GenericTypeArguments[0]).Invoke(this, new[] { arrayCopyCollection });
                    object reference = Activator.CreateInstance(info.FieldType, genericObjects as ICollection);
                    info.SetValue(obj, reference);
                }
            }
            else
            {
                idToRead++;
                object objReference = InspectWrite(info.FieldType, info.GetValue(obj), idRoute, idToRead, value);
                info.SetValue(obj, objReference);
            }

            return obj;
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