using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public CharacterController Controller;
    public Transform cameraTransform;

    [Header("Player Settings")]
    public float PlayerSpeed = 5.0f;
    public float mouseSensitivityX = 500f; //UpDown
    public float mouseSensitivityY = 1000f; //LeftRight

    float xRotation = 0f;
    float currentYRotation = 0f;

    void Start()
    {
        Controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        MovePlayer();

        if(!Input.GetMouseButton(1)) {
            HandleCameraInputs();
        }
    }

    public void MovePlayer()
{
    // Get input for movement
    Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

    // Normalize the movement vector to ensure consistent speed
    if (move.magnitude > 1)  // If the vector length is greater than 1 (e.g., diagonal movement)
    {
        move.Normalize();  // Normalize the vector to make the total speed consistent
    }

    // Make movement relative to the player's facing direction
    move = transform.TransformDirection(move);

    // Move the player using the CharacterController
    Controller.Move(move * Time.deltaTime * PlayerSpeed);
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
