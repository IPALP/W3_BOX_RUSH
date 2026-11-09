using UnityEngine;
using System.Collections.Generic;
public class DeliveryZone : MonoBehaviour
{
  public int points = 10;
  [Tooltip("Seconds the box stays visible after landing, before if dissapears")]
  public float destroyDelay = 0.5f;

  readonly  HashSet<Rigidbody> counted = new HashSet<Rigidbody>();
    void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        
        if(rb == null || rb.isKinematic || !rb.useGravity) return;

        if(!counted.Add(rb)) return;

        if(GameManager.Instance != null)
            GameManager.Instance.AddScore(points);

        Destroy(rb.gameObject, destroyDelay);
    }
}
