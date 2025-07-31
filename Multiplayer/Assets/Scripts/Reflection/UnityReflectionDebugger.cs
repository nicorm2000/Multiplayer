using UnityEngine;
using Net;

public class UnityReflectionDebugger : MonoBehaviour, IReflectionDebugger
{
    private readonly bool showDebugs;

    public UnityReflectionDebugger(bool showDebugs)
    {
        this.showDebugs = showDebugs;
    }

    public void Log(string message)
    {
        if (showDebugs)
            Debug.Log(message);
    }

    public void Pause()
    {
        Debug.Break();
    }
}