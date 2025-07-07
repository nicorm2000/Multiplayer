using UnityEngine;
using Net;

public class ReflectionSystem : MonoBehaviourSingleton<ReflectionSystem>
{
    public Reflection reflection;
    public NETAUTHORITY netAuthority;

    private void Start()
    {
        NetworkManager.Instance.onInitEntity += StartReflection;
    }

    void StartReflection()
    {
#if SERVER
        netAuthority = NETAUTHORITY.SERVER;
#elif CLIENT
        netAuthority = NETAUTHORITY.CLIENT;
#endif
        reflection = new(NetworkManager.Instance.networkEntity, netAuthority);
        Reflection.consoleDebugger += WriteConsoleDebugger;
        Reflection.consoleDebuggerPause+= PauseConsoleDebugger;
    }

    private void LateUpdate()
    {
        reflection?.UpdateReflection();
    }

    void PauseConsoleDebugger()
    {
        Debug.Break();
    }

    void WriteConsoleDebugger(string message)
    {
        Debug.Log(message);
    }
}