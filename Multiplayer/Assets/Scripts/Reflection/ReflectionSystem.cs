using UnityEngine;
using Net;

public class ReflectionSystem : MonoBehaviourSingleton<ReflectionSystem>
{
    public Reflection reflection;
    public NETAUTHORITY netAuthority;
    public bool showDebugs = true;
    private UnityReflectionDebugger debugger;

    private void Start()
    {
        debugger = new UnityReflectionDebugger(showDebugs);

        NetworkManager.Instance.onInitEntity += StartReflection;
    }

    void StartReflection()
    {
#if SERVER
        netAuthority = NETAUTHORITY.SERVER;
#elif CLIENT
        netAuthority = NETAUTHORITY.CLIENT;
#endif
        reflection = new(NetworkManager.Instance.networkEntity, netAuthority, debugger);
    }

    private void LateUpdate()
    {
        reflection?.UpdateReflection();
    }
}