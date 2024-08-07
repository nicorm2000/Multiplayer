using System.Collections;
using UnityEngine;
using Net;
using System;

public class ReflectionSystem : MonoBehaviour
{
    Reflection reflection;

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
