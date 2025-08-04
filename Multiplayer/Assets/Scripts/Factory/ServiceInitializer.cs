using System.Collections.Generic;
using UnityEngine;

public class ServiceInitializer : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabs;

    private void Awake()
    {
        IPrefabService prefabService = new PrefabService(prefabs);
        ServiceProvider.RegisterService(prefabService);
    }
}