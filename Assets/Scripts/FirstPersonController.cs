using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    
    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 200f;
    [SerializeField] private Transform cameraTransform;

    [Header("Components")]
    [SerializeField] private Rigidbody rb;

    private float xRotation = 0f;
    private float inputX;
    private float inputZ;
    private float mouseX;
    private float mouseY;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        
        // If you didn't assign the camera in inspector, try to find it in children
        if (cameraTransform == null && transform.childCount > 0)
        {
            cameraTransform = GetComponentInChildren<Camera>().transform;
        }

        // Lock and hide the mouse cursor so it doesn't wander off screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. Gather Keyboard Input
        inputX = Input.GetAxisRaw("Horizontal");
        inputZ = Input.GetAxisRaw("Vertical");

        // 2. Gather Mouse Input (Multiply by Time to keep it smooth regardless of framerate)
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime * 5;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 3. Calculate Looking Up/Down (Clamp it so you can't flip your neck upside down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);
    }

    void FixedUpdate()
    {
        // 4. Turn the Player body Left/Right, and Camera Up/Down
        transform.Rotate(Vector3.up * mouseX);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 5. Move relative to where the player is currently looking
        Vector3 moveDirection = (transform.forward * inputZ + transform.right * inputX).normalized;
        
        Vector3 targetVelocity = moveDirection * moveSpeed;
        targetVelocity.y = rb.linearVelocity.y; // Keep gravity working

        rb.linearVelocity = targetVelocity;
    }
}