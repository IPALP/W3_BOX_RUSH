using UnityEngine;

public class MultiConveyorBelt : MonoBehaviour
{
   public float speed = 1.5f;
   public Vector3[] directions = { new Vector3(0f, 0f, 1f), new Vector3(1f, 0f, 0f) };

   Collider[] colliders;

   void Awake()
    {
        colliders = GetComponents<Collider>();
    }

    void OnCollisionStay(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;

        if(rb == null || rb.isKinematic || !rb.useGravity) return;

        for(int i = 0; i< collision.contactCount; i++)
        {
            ContactPoint contact = collision.GetContact(i);

            if(Mathf.Abs(contact.normal.y) < 0.7f) continue;

            int index = System.Array.IndexOf(colliders, contact.thisCollider);
            if(index < 0 || index >= directions.Length) continue;

            Vector3 dir = directions[index];
            dir.y =0f;
            dir.Normalize();

            Vector3 vertical = Vector3.Project(GetVelocity(rb), Vector3.up);
            SetVelocity(rb, dir * speed + vertical);
            return;
        }

    }

    static Vector3 GetVelocity(Rigidbody rb)
    {
#if UNITY_6000_0_OR_NEWER
    return rb.linearVelocity;
#else
    return rb.velocity;
#endif
    }

    static void SetVelocity(Rigidbody rb, Vector3 v)
    {
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = v;
#else
        rb.velocity = v;
#endif
    }

    void OnDrawGizmos()
{
    Collider[] cols = GetComponents<Collider>();
    Gizmos.color = Color.yellow;
    for(int i = 0; i< cols.Length && i < directions.Length; i++)
    {
        Bounds b = cols[i].bounds;
        Vector3 start = b.center + Vector3.up * (b.extents.y + 0.5f);
        Vector3 dir = directions[i];
        dir.y = 0f;
        dir.Normalize();
        Vector3 end = start + dir*2f;
        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 0.15f);
    }
}    
}
