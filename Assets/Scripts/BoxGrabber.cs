using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class BoxGrabber : MonoBehaviour
{
    [Header("Refrences")]
    [Tooltip("Main Camera")]
    public Camera playerCamera;

    [Header("Grab")]
    public float grabRange = 3f;
    public float followStrength = 15f;
    public float maxMass = 20f;
    [Tooltip("If the box gets stuck this far from the hold position, it is dropped")]
    public float breakDistance = 3f;
    public float holdDistance = 1.8f;

    [Header("Throw")]
    public float throwSpeed = 14f;

    Rigidbody held;
    bool heldHadGravity;
    Collider[] heldColliders;
    CharacterController playerController;

    void Awake()
    {
        if(playerCamera == null) playerCamera = Camera.main;
        playerController = GetComponentInParent<CharacterController>();
    }

    void Update()
    {
        if(Cursor.lockState != CursorLockMode.Locked) return;

        if(GrabPressed())
        {
            if(held == null) TryGrab();
            else Release();
        }
        else if(held != null && ThrowPressed())
        {
            Throw();
        }
    }
    void FixedUpdate()
    {
        if(held == null) return;
        Transform cam = playerCamera.transform;
        Vector3 target = cam.position + cam.forward * holdDistance;
        Vector3 toTarget = target - held.position;

        if(toTarget.magnitude > breakDistance)
        {
            Release();
            return;
        }

        SetVelocity(held, toTarget * followStrength);
        SetAngularVelocity(held, Vector3.zero);
    }
    void TryGrab()
    {
        Transform cam = playerCamera.transform;
        if(!Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, grabRange, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            return;
        Rigidbody rb = hit.rigidbody;
        if(rb == null || rb.isKinematic || rb.mass > maxMass) return;

        held = rb;
        heldHadGravity = rb.useGravity;
        rb.useGravity = false;
        heldColliders = rb.GetComponentsInChildren<Collider>();
        SetIgnorePlayer(true);
    }

    void Release()
    {
        if(held == null) return;

        SetIgnorePlayer(false);
        held.useGravity = heldHadGravity;
        held = null;
    }

    void Throw()
    {
        Rigidbody rb = held;
        Release();
        SetVelocity(rb, playerCamera.transform.forward * throwSpeed);
    }

    void SetIgnorePlayer(bool ignore)
    {
        if(playerController == null || heldColliders == null) return;
        foreach(Collider c in heldColliders)
            Physics.IgnoreCollision(c, playerController, ignore);
    }

    static void SetVelocity(Rigidbody rb, Vector3 v)
    {
        #if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = v;
        #else
            rb.velocity = v;
        #endif
    }

    static void SetAngularVelocity(Rigidbody rb, Vector3 v)
    {
        rb.angularVelocity = v;
    }

#if ENABLE_INPUT_SYSTEM
    static bool GrabPressed() => Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
    static bool ThrowPressed() => Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
    static bool GrabPressed() => Input.GetKeyDown(KeyCode.E);
    static bool ThrowPressed() => Input.GetMouseButtonDown(0);
#endif
}
