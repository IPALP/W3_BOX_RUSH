using UnityEngine;
using System.Collections.Generic;

public class BoxSpawner: MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] goodBoxPrefabs;
    public GameObject[] brokenBoxPrefabs;
    public GameObject woodenCratePrefab;

    [Header("Chances (the rest is good boxes)")]
    [Range(0f, 1f)] public float brokenChance = 0.25f;
    [Range(0f, 1f)] public float crateChance = 0.15f;

    [Header("Spawning")]
    public float interval = 4f;
    [Tooltip("No new boxes spawn while this many spawned boxes still exist")]
    public int maxAlive = 10;
    [Tooltip("Each box is turned a random amount so they don't all look identical")]
    public float randomYaw = 15f;

    float timer;
    readonly List<GameObject> alive = new List<GameObject>();

    void Update()
    {
        timer += Time.deltaTime;
        if(timer < interval) return;
        timer = 0f;

        alive.RemoveAll(b => b == null);
        if(alive.Count >= maxAlive) return;

        GameObject prefab = PickPrefab();
        if(prefab == null) return;

        Quaternion rotation = transform.rotation * Quaternion.Euler(0f, Random.Range(-randomYaw, randomYaw), 0f);
        alive.Add(Instantiate(prefab, transform.position, rotation));
    }

    GameObject PickPrefab()
    {
        float roll = Random.value;
        if(roll < crateChance)
            return woodenCratePrefab;
        if(roll < crateChance + brokenChance)
            return RandomFrom(brokenBoxPrefabs);
        
        return RandomFrom(goodBoxPrefabs);
    }

    static GameObject RandomFrom(GameObject[] list)
    {
        if(list == null || list.Length == 0) return null;
        return list[Random.Range(0, list.Length)];
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.6f);
    }

}