using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ConveyorBelt : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 2f;
    [Tooltip("How quickly a box reaches belt speed (higher = snappier).")]
    [SerializeField] private float acceleration = 15f;
    [Tooltip("Belt direction in this object's local space. Forward = the blue arrow in the Scene view.")]
    [SerializeField] private Vector3 localDirection = Vector3.forward;

    [Header("Optional: scroll the belt texture")]
    [SerializeField] private Renderer beltRenderer;
    [SerializeField] private Vector2 scrollDirection = new Vector2(0f, -1f);
    [SerializeField] private float scrollMultiplier = 0.5f;

    private readonly HashSet<Rigidbody> bodiesOnBelt = new HashSet<Rigidbody>();
    private Material beltMaterial;
    private Vector2 textureOffset;

    private void Awake()
    {
        if (beltRenderer != null)
            beltMaterial = beltRenderer.material;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.rigidbody != null)
            bodiesOnBelt.Add(collision.rigidbody);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.rigidbody != null)
            bodiesOnBelt.Remove(collision.rigidbody);
    }

    private void FixedUpdate()
    {
        Vector3 direction = Vector3.ProjectOnPlane(transform.TransformDirection(localDirection), Vector3.up).normalized;
        Vector3 targetVelocity = direction * speed;

        bodiesOnBelt.RemoveWhere(rb => rb == null);

        foreach (Rigidbody rb in bodiesOnBelt)
        {
            if (rb.isKinematic) continue;

            Vector3 v = rb.linearVelocity;
            Vector3 flat = new Vector3(v.x, 0f, v.z);
            flat = Vector3.MoveTowards(flat, targetVelocity, acceleration * Time.fixedDeltaTime);
            rb.linearVelocity = new Vector3(flat.x, v.y, flat.z);

            // Adaugat trebuie testat 2 linii
            rb.constraints = RigidbodyConstraints.FreezeRotationX;
            rb.constraints = RigidbodyConstraints.FreezeRotationZ;
        }
    }

    private void Update()
    {
        if (beltMaterial == null) return;

        textureOffset += scrollDirection * (speed * scrollMultiplier * Time.deltaTime);
        beltMaterial.mainTextureOffset = textureOffset;
    }
}