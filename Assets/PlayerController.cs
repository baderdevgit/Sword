using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public CharacterController Controller;
    public Transform cameraTransform;

    [Header("Player Settings")]
    public float PlayerSpeed = 5.0f;
    public float JumpHeight = 2f;
    public float mouseSensitivityX = 500f; //UpDown
    public float mouseSensitivityY = 1000f; //LeftRight
    public float gravity = -30f;
    public bool isGrounded;

    float xRotation = 0f;
    float currentYRotation = 0f;
    private Vector3 velocity;  

    void Start()
    {
        Controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if(!Input.GetMouseButton(1)) {
            HandleCameraInputs();
        }
        MovePlayer();
    }

    public void MovePlayer()
    {
        //ensures y velocity doesn't get to large
        isGrounded = Controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (move.magnitude > 1)  
        {
            move.Normalize();
        }

        move = transform.TransformDirection(move);
        Controller.Move(move * Time.deltaTime * PlayerSpeed);

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(JumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        Controller.Move(velocity * Time.deltaTime);
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
}
