using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    static Transform[] spawnpoints;

    void Awake()
    {
        spawnpoints = GetComponentsInChildren<Transform>();
    }

    public static Transform GetSpawnpoint()
    {
        return spawnpoints[Random.Range(0, spawnpoints.Length)];
    }
}
