using System;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SwordController : MonoBehaviour
{

    public PlayerController playerController;
    public GameObject cog;

    public float SwordMovementSpeed = 0.1f;
    public float SwordRotationSpeed = 10f;
    public float SwordSlashSpeed = 1f;

    public float moveDistance = 2f;

    //mouse stuff
    private Vector3 lastMousePosition;
    private Vector3 mouseVelocity;
    public float velocityThreshold = 0.1f;  

    private Vector3 startingPosition;
    private Vector3 currentPosition;
    private Quaternion startingRotation;
    private Quaternion currentRotation;
    private Vector3 startingTransform;

    private float currentAngle = 0f;
    private float targetAngle = 90f;

    bool isCutting = false;

    float initial_angle = 25f;
    float cutting_angle = 90f;
    Quaternion targetRotation = Quaternion.Euler(90, 0, 0);


    Quaternion originalRotation;
    bool rotatingDown = true;
    private Vector2 accumulatedMouseDelta;

    void Start()
    {
        lastMousePosition = Input.mousePosition;
        originalRotation = transform.localRotation;


    }

    void Update()
    {
        if (playerController.isCameraLocked)
        {
            rotateSword();
        }
        HandleSlash();   
    }

    private void HandleSlash()
    {
        transform.GetLocalPositionAndRotation(out currentPosition, out currentRotation);
        if (Input.GetKey(KeyCode.Mouse0))
        {
            extendSword();
        }
        else
        {
            retractSword();
        }
    }

    private void rotateSword()
    {

        //transform.RotateAround(cog.transform.position, playerController.transform.forward, SwordRotationSpeed );
        //transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, SwordSlashSpeed * Time.deltaTime);

        // Step 1: Get the mouse delta movement for X and Y axes
        float mouseX = Input.GetAxis("Mouse X") * 20f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * 20f * Time.deltaTime;

        // Step 2: Accumulate the mouse delta in a 2D vector (like you're tracking cursor movement)
        accumulatedMouseDelta += new Vector2(mouseX, mouseY);

        // Step 3: Calculate the angle from the center (Atan2 gives you the angle in radians)
        currentAngle = Mathf.Atan2(accumulatedMouseDelta.y, accumulatedMouseDelta.x) * Mathf.Rad2Deg;

        // Step 4: Ensure the angle is in the range of -360 to 360 degrees (normalize if needed)
        if (currentAngle > 360f) currentAngle -= 360f;
        if (currentAngle < -360f) currentAngle += 360f;

        // Step 5: Rotate the sword based on the calculated angle (side to side)
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, 90f);

        // Step 6: Smoothly rotate the sword towards the target angle
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, SwordSlashSpeed * Time.deltaTime);

        
    }



    public void extendSword()
    {
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, SwordSlashSpeed * Time.deltaTime);
    }

    public void retractSword()
    {
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, originalRotation, SwordSlashSpeed * Time.deltaTime);
    }






}
