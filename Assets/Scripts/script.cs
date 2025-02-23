using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class DriftingSword : MonoBehaviour
{
    public Camera mainCamera; // Reference to the main camera
    public float followSpeed = 2f; // Speed at which the sword catches up to the mouse
    public float delayFactor = 0.1f; // How much to delay the sword's response

    private Vector3 mouseWorldPosition;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        moveSwordXY();
    }
    
    //void moveSwordXY()
    //{
    //    // Get the mouse position in screen space
    //    Vector3 mousePosition = Input.mousePosition;

    //    // Convert the screen position to world position at a fixed distance from the camera
    //    mousePosition.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z); // Maintain sword distance
    //    mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

    //    // Sword drifts towards the mouse position (on XY plane)

    //    Vector3 targetPosition = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, transform.position.z); // Only affect XY
    //    transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    //}

    void moveSwordXY()
    {
        // Get the mouse position in screen space
        Vector3 mousePosition = Input.mousePosition;

        // Convert the screen position to world position at a fixed distance from the camera
        mousePosition.z = Mathf.Abs(mainCamera.transform.position.z - transform.position.z); // Maintain sword distance
        mouseWorldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        // Convert world position to local position relative to the player
        Vector3 localMousePosition = transform.parent.InverseTransformPoint(mouseWorldPosition);

        // Only affect XY, keeping Z unchanged
        var border = 0.75;
        Vector3 targetLocalPosition = new Vector3((float)Math.Clamp(localMousePosition.x, initialPosition.x - border, initialPosition.x + border), (float)Math.Clamp(localMousePosition.y, initialPosition.y - 0.1, initialPosition.y + border), transform.localPosition.z);
        Debug.Log(targetLocalPosition);

        // Sword drifts towards the mouse position in the player's local space
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocalPosition, followSpeed * Time.deltaTime);
    }
}
