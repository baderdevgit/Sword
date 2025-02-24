using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;

    [Header("Player Settings")]
    public float PlayerSpeed = 1f;
    public float SprintSpeed = 5f;
    public float JumpHeight = 1f;
    public bool isGrounded;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //handles falling off platform
        if(gameObject.transform.position.y < -5)
        {
            gameObject.transform.position = new Vector3(1, 1, -10);
        }

        GetPreMoveInfo();

        HandlePlayerMovements();
    }

    public void HandlePlayerMovements()
    {
        if (isGrounded)
        {
            HandleMovement();
            HandleJump();
        }

    }

    private void HandleMovement()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (move.magnitude > 1)
        {
            move.Normalize();
        }
        move = transform.TransformDirection(move);

        float speed = Input.GetKey(KeyCode.LeftShift) ? SprintSpeed : PlayerSpeed;

        rb.AddForce(move * speed, ForceMode.VelocityChange);

        //prevent player from exceeding that speed in the case of holding down W and A
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);  // Ignore Y (vertical) axis
        horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, speed);  // Clamp only the X and Z axes
        rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
    }

    private void GetPreMoveInfo()
    {
        //isCameraLocked = Input.GetMouseButton(1);
        isGrounded = IsGrounded();
    }  

    public bool IsGrounded()
    {
        RaycastHit hit;
        float rayLength = 1.5f; // Adjust based on your character's size
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
        }
    }

    

}
