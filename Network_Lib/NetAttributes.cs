using System;

namespace Net
{
    #region Attributes
    /// <summary>
    /// Attribute for marking classes that handle specific message types.
    /// </summary>
    public class NetMessageClass : Attribute
    {
        Type type;
        MessageType messageType;

        /// <summary>
        /// Initializes a new instance of the NetMessageClass attribute.
        /// </summary>
        /// <param name="type">The message type.</param>
        /// <param name="messageType">The network message type enum value.</param>
        public NetMessageClass(Type type, MessageType messageType)
        {
            this.type = type;
            this.messageType = messageType;
        }

        /// <summary>
        /// Gets the network message type.
        /// </summary>
        public MessageType MessageType
        {
            get { return messageType; }
        }

        /// <summary>
        /// Gets the message class type.
        /// </summary>
        public Type Type
        {
            get { return type; }
        }
    }

    /// <summary>
    /// Attribute for marking fields that should be synchronized over the network.
    /// </summary>
    public class NetVariable : Attribute
    {
        int variableId;
        MessagePriority messagePriority;
        public NETAUTHORITY syncAuthority = NETAUTHORITY.SERVER;

        /// <summary>
        /// Initializes a new instance of the NetVariable attribute.
        /// </summary>
        /// <param name="id">The unique identifier for this variable.</param>
        /// <param name="netAuthority">The authority for this variable.</param>
        /// <param name="messagePriority">The priority for network messages.</param>
        public NetVariable(int id, NETAUTHORITY netAuthority = NETAUTHORITY.SERVER, MessagePriority messagePriority = MessagePriority.Default)
        {
            variableId = id;
            syncAuthority = netAuthority;
            this.messagePriority = messagePriority;
        }

        /// <summary>
        /// Gets the message priority for this variable.
        /// </summary>
        public MessagePriority MessagePriority
        {
            get { return messagePriority; }
        }

        /// <summary>
        /// Gets the unique identifier for this variable.
        /// </summary>
        public int VariableId
        {
            get { return variableId; }
            set { variableId = value; }
        }
    }

    /// <summary>
    /// Attribute for marking methods that should be invokable over the network.
    /// </summary>
    public class NetMethod : Attribute
    {
        int methodId;
        MessagePriority messagePriority;
        public NETAUTHORITY syncAuthority = NETAUTHORITY.SERVER;

        /// <summary>
        /// Initializes a new instance of the NetMethod attribute.
        /// </summary>
        /// <param name="id">The unique identifier for this method.</param>
        /// <param name="netAuthority">The authority for this method.</param>
        /// <param name="messagePriority">The priority for network messages.</param>
        public NetMethod(int id, NETAUTHORITY netAuthority = NETAUTHORITY.SERVER, MessagePriority messagePriority = MessagePriority.Default)
        {
            methodId = id;
            syncAuthority = netAuthority;
            this.messagePriority = messagePriority;
        }

        /// <summary>
        /// Gets the message priority for this method.
        /// </summary>
        public MessagePriority MessagePriority
        {
            get { return messagePriority; }
        }

        /// <summary>
        /// Gets the unique identifier for this method.
        /// </summary>
        public int MethodId
        {
            get { return methodId; }
        }
    }

    /// <summary>
    /// Attribute for marking classes that contain extension methods for network reflection.
    /// </summary>
    public class NetExtensionClass : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the NetExtensionClass attribute.
        /// </summary>
        public NetExtensionClass()
        {

        }
    }

    /// <summary>
    /// Attribute for marking extension methods used in network reflection.
    /// </summary>
    public class NetExtensionMethod : Attribute
    {
        /// <summary>
        /// Gets the type that this extension method extends.
        /// </summary>
        public Type extensionMethod;

        /// <summary>
        /// Initializes a new instance of the NetExtensionMethod attribute.
        /// </summary>
        /// <param name="type">The type that this method extends.</param>
        public NetExtensionMethod(Type type)
        {
            extensionMethod = type;
        }
    }

    /// <summary>
    /// Marks a C# event as network-synchronized. When invoked by the owner of the network object,
    /// it will be triggered on all other clients that have subscribed to it.
    /// </summary>
    public class NetEvent : Attribute
    {
        public int EventId { get; }
        public MessagePriority MessagePriority { get; }
        public string? BackingFieldName { get; }
        public NETAUTHORITY syncAuthority = NETAUTHORITY.SERVER;

        /// <summary>
        /// Creates a new NetEvent attribute instance.
        /// </summary>
        /// <param name="eventId">Unique identifier for the event.</param>
        /// <param name="netAuthority">The authority for this event.</param>
        /// <param name="priority">The priority of the network message.</param>
        /// <param name="backingFieldName">
        /// Optional backing field name (e.g. "onEventX"). If omitted, the system falls back to a convention.
        /// </param>
        public NetEvent(int eventId, NETAUTHORITY netAuthority = NETAUTHORITY.SERVER, MessagePriority priority = MessagePriority.Default, string? backingFieldName = null)
        {
            EventId = eventId;
            syncAuthority = netAuthority;
            MessagePriority = priority;
            BackingFieldName = backingFieldName;
        }
    }

    /// <summary>
    /// Attribute that marks a field for TRS (Transform/Rotation/Scale/IsActive) synchronization over the network.
    /// </summary>
    public class NetTRS : Attribute
    {
        [Flags]
        public enum SYNC
        {
            DEFAULT = 0,
            NOTPOSITION = 1,
            NOTROTATION = 2,
            NOTSCALE = 4,
            NOTTRS = 7,
            NOTISACTIVE = 8,
            NOTALL = 15
        }

        public SYNC syncData = SYNC.DEFAULT;
        public NETAUTHORITY syncAuthority = NETAUTHORITY.SERVER;

        /// <summary>
        /// Initializes a new instance of the NetTRS attribute with sync flags and optional authority.
        /// </summary>
        /// <param name="value">The sync flags to apply.</param>
        /// <param name="netAuthority">The authority level. Defaults to SERVER.</param>
        public NetTRS(SYNC value, NETAUTHORITY netAuthority = NETAUTHORITY.SERVER)
        {
            syncData = value;
            syncAuthority = netAuthority;
        }
    }
    #endregion
}