using System.Collections.Generic;
using UnityEngine;

public interface IPrefabService
{
    GameObject GetPrefabById(int id);
    int GetIdByPrefab(GameObject prefab);
}

public class PrefabService : IPrefabService
{
    private static Dictionary<int, GameObject> idToPrefab;
    private static Dictionary<GameObject, int> prefabToId;
    
    public PrefabService(List<GameObject> prefabs)
    {
        idToPrefab = new Dictionary<int, GameObject>();
        prefabToId = new Dictionary<GameObject, int>();

        for (int i = 0; i < prefabs.Count; i++)
        {
            idToPrefab[i] = prefabs[i];
            prefabToId[prefabs[i]] = i;
        }
    }

    public GameObject GetPrefabById(int id)
    {
        idToPrefab.TryGetValue(id, out GameObject prefab);
        return prefab;
    }

    public int GetIdByPrefab(GameObject prefab)
    {
        if (prefabToId.TryGetValue(prefab, out int id))
        {
            return id;
        }

        return -1;
    }
}