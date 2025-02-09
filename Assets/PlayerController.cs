using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public Rigidbody rb;

    [Header("Player Settings")]
    public float PlayerSpeed = 5.0f;
    public float JumpHeight = 0.5f;
    public float mouseSensitivityX = 500f; //UpDown
    public float mouseSensitivityY = 1000f; //LeftRight
    public bool isGrounded;
    public bool isCameraLocked;

    float xRotation = 0f;
    float currentYRotation = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //rb.linearDamping = 5f;
        
        Cursor.lockState = CursorLockMode.Locked;
        isCameraLocked = false;
    }

    void Update()
    {
        GetPreMoveInfo();

        if (!isCameraLocked) {
            HandleCameraInputs();
        }

        HandlePlayerMovements();
    }

    public void HandlePlayerMovements()
    {
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (move.magnitude > 1)
        {
            move.Normalize();
        }
        move = transform.TransformDirection(move);

        //if (isGrounded)
        //{
            // Apply force with VelocityChange to instantly change velocity
            rb.AddForce(move * PlayerSpeed, ForceMode.VelocityChange);

            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);  // Ignore Y (vertical) axis
            horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, PlayerSpeed);  // Clamp only the X and Z axes

            // Combine clamped horizontal velocity with the original Y-axis velocity
            rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);

        //}
    }

    private void GetPreMoveInfo()
    {
        isCameraLocked = Input.GetMouseButton(1);
        isGrounded = IsGrounded();
    }

    public void HandleCameraInputs() {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;
        
        currentYRotation += mouseX;  
        Quaternion targetRotation = Quaternion.Euler(0f, currentYRotation, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);  // Smooth horizontal rotation

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -10f, 20f);  
        cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, Quaternion.Euler(xRotation, 0f, 0f), Time.deltaTime * 5f); // Smooth vertical rotation

    }

    public bool IsGrounded()
    {
        RaycastHit hit;
        float rayLength = 1.1f; // Adjust based on your character's size
        if (Physics.Raycast(transform.position, Vector3.down, out hit, rayLength))
        {
            return true;
        }
        return false;
    }

    private void HandleJump()
    {
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * JumpHeight, ForceMode.Impulse);
            Debug.Log("Jumping");
        }
    }
}
