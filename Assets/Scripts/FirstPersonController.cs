using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class FirstPersonController : MonoBehaviour
{
    [Header("Refrences")]
    public Transform cameraPivot;
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float jumpHeight = 1.1f;
    public float gravity = -20f;

    [Header("Look")]
    public float lookSensitivity = 0.1f;
    public float maxLookAngle = 85f;

    CharacterController controller;
    float verticalVelocity;
    float pitch;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        LockCursor(true);
    }

    void Update()
    {
        if(EscapePressed()) LockCursor(false);
        if(Cursor.lockState != CursorLockMode.Locked)
        {
            if(ClickPressed()) LockCursor(true);
            return;
        }
        Look();
        Move();
    }

    void Look()
    {
        Vector2 look = GetLook() * lookSensitivity;
        transform.Rotate(Vector3.up * look.x);
        pitch = Mathf.Clamp(pitch - look.y, -maxLookAngle, maxLookAngle);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

    }
    void Move()
    {
        Vector2 input = GetMove();
        Vector3 direction = transform.right * input.x +transform.forward * input.y;
        if(direction.sqrMagnitude > 1f)
            direction.Normalize();
        
    float speed = SprintHeld() ? sprintSpeed : walkSpeed;

        if(controller.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f;
        
    if(controller.isGrounded && JumpPressed())
        verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

    verticalVelocity += gravity * Time.deltaTime;

    Vector3 velocity = direction * speed + Vector3.up * verticalVelocity;
    controller.Move(velocity * Time.deltaTime); 
    }

    static void LockCursor(bool locked)
    {
        Cursor.lockState = locked ?
        CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    #if ENABLE_INPUT_SYSTEM
    static Vector2 GetMove()
    {
        var k = Keyboard.current;
        if(k == null) return Vector2.zero;
        bool right = k.dKey.isPressed || k.rightArrowKey.isPressed;
        bool left = k.aKey.isPressed || k.leftArrowKey.isPressed;
        bool up = k.wKey.isPressed || k.upArrowKey.isPressed;
        bool down = k.sKey.isPressed || k.downArrowKey.isPressed;
        float x = (right ? 1:0) - (left? 1:0);
        float y = (up ? 1:0 ) - (down ? 1:0);
        return new Vector2(x, y);

    }
    static Vector2 GetLook() => Mouse.current != null ? Mouse.current.delta.ReadValue() : Vector2.zero;
    static bool JumpPressed() => Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
    static bool SprintHeld() => Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
    static bool EscapePressed() => Keyboard.current !=null && Keyboard.current.escapeKey.wasPressedThisFrame;
    static bool ClickPressed() => Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
#else
    static Vector2 GetMove() => new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    static Vector2 GetLook() => new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
    static bool JumpPressed() => Input.GetKeyDown(KeyCode.Space);
    static bool SprintHeld() => Input.GetKey(KeyCode.Shift);
    static bool EscapePressed() => Input.GetKeyDown(KeyCode.Escape);
    static bool ClickPressed() => Input.GetMouseButtonDown(0);
#endif
}
