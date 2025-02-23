using UnityEngine;

public class CubeMovement : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // Get the main camera
    }

    void Update()
    {
        MoveCube();
    }

    void MoveCube()
    {
        // Create a ray from the camera through the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray hits something (like the ground or plane)
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;  // Get the point on the plane where the ray hits

            // Move the cube to the mouse position on the XZ plane (Y remains unchanged)
            transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        }
    }
}
