using UnityEngine;

public class PlayerMouseInputController : MonoBehaviour
{
    public Transform cameraTransform;
    public float mouseSensitivityX = 500f; //UpDown
    public float mouseSensitivityY = 1000f; //LeftRight
    float xRotation = 0f;
    float currentYRotation = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandlePlayerRotation();
    }

    private void HandlePlayerRotation()
    {
        //if (!isCameraLocked)
        //{
            HandleCameraInputs();
        //}
        //else
        //{
            //ResetCameraPosition();
        //}
    }

    private void ResetCameraPosition()
    {
        if (cameraTransform.localRotation != Quaternion.Euler(15f, 0f, 0f))
            cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, Quaternion.Euler(15f, 0f, 0f), Time.deltaTime * 5f);
    }

    public void HandleCameraInputs()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;

        currentYRotation += mouseX;
        Quaternion targetRotation = Quaternion.Euler(0f, currentYRotation, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);  // Smooth horizontal rotation

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, 5f, 15);
        cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, Quaternion.Euler(xRotation, 0f, 0f), Time.deltaTime * 5f); // Smooth vertical rotation
    }
}
