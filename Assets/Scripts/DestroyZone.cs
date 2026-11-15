using UnityEngine;

public class DestroyZone : MonoBehaviour
{
   public float destroyDelay = 0.5f;

   void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if(rb == null || rb.isKinematic || !rb.useGravity) return;
        Destroy(rb.gameObject, destroyDelay);
    } 
}
