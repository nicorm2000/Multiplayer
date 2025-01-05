using UnityEngine;
using Net;

public class ReflectionSystem : MonoBehaviourSingleton<ReflectionSystem>
{
    public Reflection reflection;

    private void Start()
    {
        NetworkManager.Instance.onInitEntity += StartReflection;
    }

    void StartReflection()
    {
        reflection = new(NetworkManager.Instance.networkEntity);
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