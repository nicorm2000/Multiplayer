using UnityEngine;
using Net;

/// <summary>
/// Provides debugging functionality for the reflection system with Unity integration.
/// </summary>
public class UnityReflectionDebugger : MonoBehaviour, IReflectionDebugger
{
    private bool showDebugs;

    /// <summary>
    /// Initializes a new instance of the UnityReflectionDebugger.
    /// </summary>
    /// <param name="showDebugs">Whether debug messages should be shown.</param>
    public UnityReflectionDebugger(bool showDebugs)
    {
        this.showDebugs = showDebugs;
    }

    /// <summary>
    /// Logs a message to the Unity console if debugging is enabled.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void Log(string message)
    {
        if (showDebugs)
            Debug.Log(message);
    }

    /// <summary>
    /// Pauses the editor execution (enters break mode).
    /// </summary>
    public void Pause()
    {
        Debug.Break();
    }
}