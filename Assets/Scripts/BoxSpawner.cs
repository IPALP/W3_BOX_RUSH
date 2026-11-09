using UnityEngine;
using System.Collections.Generic;
public class BoxSpawner : MonoBehaviour
{
   public GameObject boxPrefabs;
   public float interval = 4f;
   [Tooltip("No new boxes spawn while this many spawned boxes still exist")]
   public int maxAlive = 10;
   [Tooltip("Each box is turned a random ammount so they don't all look identical")]
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

        Quaternion rotation = transform.rotation* Quaternion.Euler(0f, Random.Range(-randomYaw, randomYaw), 0f);
        alive.Add(Instantiate(boxPrefabs, transform.position,rotation ));
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, Vector3.one*0.6f);
    }
}
