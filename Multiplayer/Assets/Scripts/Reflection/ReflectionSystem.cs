using Net;

/// <summary>
/// Manages the reflection system for network synchronization of game objects.
/// </summary>
public class ReflectionSystem : MonoBehaviourSingleton<ReflectionSystem>
{
    public Reflection reflection;
    public NETAUTHORITY netAuthority;
    public bool showDebugs = true;
    private UnityReflectionDebugger debugger;

    /// <summary>
    /// Initializes the reflection debugger and sets up network entity events.
    /// </summary>
    private void Start()
    {
        debugger = new UnityReflectionDebugger(showDebugs);

        NetworkManager.Instance.onInitEntity += StartReflection;
    }

    /// <summary>
    /// Initializes the reflection system with the appropriate network authority.
    /// </summary>
    void StartReflection()
    {
#if SERVER
        netAuthority = NETAUTHORITY.SERVER;
#elif CLIENT
        netAuthority = NETAUTHORITY.CLIENT;
#endif
        reflection = new(NetworkManager.Instance.networkEntity, netAuthority, debugger);
    }

    /// <summary>
    /// Updates the reflection system during the late update phase of each frame.
    /// </summary>
    private void LateUpdate()
    {
        reflection?.UpdateReflection();
    }
}