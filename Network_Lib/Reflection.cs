using System.Collections.Generic;
using Network_Lib.BasicMessages;
using System.Reflection;
using System.Linq;
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
        public ReflectionCallInvoker reflectionCallInvoker { get; private set; }
        public ReflectionCollectionHelper reflectionCollectionHelper { get; private set; }
        public ReflectionInspector reflectionInspector { get; private set; }
        public ReflectionMessageHandler reflectionMessageHandler { get; private set; }
        public ReflectionMapping reflectionMapping { get; private set; }
        public ReflectionReader reflectionReader { get; private set; }
        public ReflectionWriter reflectionWriter { get; private set; }
        public NetworkEntity networkEntity { get; private set; }
        public NETAUTHORITY netAuthority { get; private set; }
        public BindingFlags bindingFlags { get; private set; }

        public IReflectionDebugger debugger;
        public Dictionary<Type, MethodInfo> extensionMethods = new Dictionary<Type, MethodInfo>();
        public Dictionary<object, Dictionary<object, int>> previousDictionaryStates = new Dictionary<object, Dictionary<object, int>>();
        public readonly Dictionary<object, int> previousCollectionCounts = new Dictionary<object, int>();
        #endregion

        #region Initialization
        /// <summary>
        /// Provides reflection-based inspection and manipulation of network objects.
        /// Handles serialization, deserialization, and network communication of object states.
        /// </summary>
        public Reflection(NetworkEntity entity, NETAUTHORITY netAuthority, IReflectionDebugger debugger)
        {
            networkEntity = entity;
            reflectionCollectionHelper = new ReflectionCollectionHelper(this);
            reflectionCallInvoker = new ReflectionCallInvoker(this);
            reflectionInspector = new ReflectionInspector(this);
            reflectionMapping = new ReflectionMapping(this);
            reflectionMessageHandler = new ReflectionMessageHandler(this);
            reflectionReader = new ReflectionReader(this);
            reflectionWriter = new ReflectionWriter(this);
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
                            //debugger?.Log($"Registered extension for: {netExtensionMethod.extensionMethod.Name}");
                        }
                    }
                }
            }
        }
        #endregion

        #region Update
        /// <summary>
        /// Updates all fields currently marked for inspection and synchronization.
        /// </summary>
        public void UpdateReflection()
        {
            reflectionInspector.UpdateAllFields();
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

        #region Utilities
        /// <summary>
        /// Verifies if the current instance has authority to perform an action, then executes the appropriate callback.
        /// </summary>
        /// <param name="owner">The network owner ID of the data.</param>
        /// <param name="auxAuthority">The expected authority for this operation.</param>
        /// <param name="onClientAuthority">Callback executed if the client has authority.</param>
        /// <param name="onServerAuthority">Callback executed if the server has authority.</param>
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