using UnityEngine;
using Net;

public class ReflectionSystem : MonoBehaviourSingleton<ReflectionSystem>
{
    public Reflection reflection;
    public NETAUTHORITY netAuthority = NETAUTHORITY.CLIENT;
    private UnityReflectionDebugger debugger;

    private void Start()
    {
        NetworkManager.Instance.onInitEntity += StartReflection;
    }

    void StartReflection()
    {
        reflection = new(NetworkManager.Instance.networkEntity, netAuthority, debugger);
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