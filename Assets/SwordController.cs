using System;
using TMPro;
using UnityEngine;

public class SwordController : MonoBehaviour
{

    public CharacterController swordController;
    public PlayerController playerController;
    public GameObject cog;

    public float SwordMovementSpeed = 0.1f;
    public float SwordRotationSpeed = 1f;
    public float SwordSlashSpeed = 1f;

    public float moveDistance = 2f;

    //mouse stuff
    private Vector3 lastMousePosition;
    private Vector3 mouseVelocity;
    private Vector3 targetPosition;
    public float velocityThreshold = 0.1f;  // Threshold to detect significant movement
    private Transform startingPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastMousePosition = Input.mousePosition;
        startingPosition = transform;
        targetPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.isCameraLocked)
        {
            rotateSword();
        }
    }

    private void GetMouseVelocity()
    {
        // Get current mouse position
        Vector3 currentMousePosition = Input.mousePosition;

        // Calculate the mouse's velocity (movement per second)
        Vector3 deltaPosition = currentMousePosition - lastMousePosition;
        mouseVelocity = deltaPosition / Time.deltaTime;


        lastMousePosition = currentMousePosition;
    }

    private void rotateSword()
    {
        transform.RotateAround(cog.transform.position, transform.forward, 10);
    }
}
