using System;

namespace Net
{
    #region Enums
    /// <summary>
    /// Defines flags for message priority behavior in the reflection system.
    /// </summary>
    [Flags]
    public enum MessagePriority
    {
        Default = 0,
        Sorteable = 1,
        NonDisposable = 2
    }

    /// <summary>
    /// Represents different types of messages used in network communication and synchronization.
    /// </summary>
    public enum MessageType
    {
        Default = -100,
        Ulong = -99,
        Uint = -98,
        Ushort = -97,
        String = -96,
        Short = -95,
        Sbyte = -94,
        Long = -93,
        Int = -92,
        Float = -91,
        Double = -90,
        Decimal = -89,
        Char = -88,
        Byte = -87,
        Bool = -86,
        Null = -85,
        Empty = -84,
        Method = -83,
        Enum = -82,
        Remove = -81,
        Event = -80,
        TRS = -79,

        DisconnectAll = -13,
        DestroyNetObj = -12,
        MatchMakerPlayerListUpdate = -11,
        Instance = -10,
        InstanceRequest = -9,
        Object = -8,
        Ping = -7,
        AssignServer = -6,
        Confirm = -5,
        Error = -4,
        MatchMakerToClientHandShake = -3,
        ServerToClientHandShake = -2,
        ClientToServerHandShake = -1,
        Console = 0,
        Position = 1,
        BulletInstatiate = 2,
        Disconnection = 3,
        UpdateLobbyTimer = 4,
        UpdateGameplayTimer = 5,
        UpdateLobbyTimerForNewPlayers = 6,
        Winner = 7,
        MatchMakerIp = 8,
    };

    /// <summary>
    /// Represents the possible states for null, empty or remove values in network communication.
    /// </summary>
    public enum PossibleStates
    {
        Null,
        Empty,
        Remove
    }


    /// <summary>
    /// Defines whether the CLIENT or SERVER has authority over a variable or event.
    /// </summary>
    public enum NETAUTHORITY
    {
        CLIENT = 0,
        SERVER = 1,
    }
    #endregion
}