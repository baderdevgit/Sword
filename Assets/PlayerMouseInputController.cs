using System;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMouseInputController : MonoBehaviour
{
    public CursorIcon cursor;

    //PlayerRotation Variables
    public Camera playerCamera;
    public float mouseSensitivityX = 500f; //UpDown
    public float mouseSensitivityY = 1000f; //LeftRight
    float xRotation = 0f;
    float currentYRotation = 0f;
    Rect centerBox;
    float centerBoxSize = 800f;

    //Sword Variables
    public float followSpeed = 2f; 
    public float delayFactor = 0.1f;
    private Vector3 mouseWorldPosition;
    private Vector3 initialSwordPosition;
    private Quaternion initialCameraRotation;
    public GameObject SwordCOG;
    private GUIStyle boxStyle;

    void Start()
    {
        float centerBoxWidth = centerBoxSize;
        float centerBoxHeight = centerBoxSize;
        centerBox = new Rect(
            (Screen.width - centerBoxWidth) / 2,
            (Screen.height - centerBoxHeight) / 2,
            centerBoxWidth,
            centerBoxHeight);

        initialSwordPosition = SwordCOG.transform.localPosition;
        initialCameraRotation = playerCamera.transform.localRotation;
    }

    void OnGUI()
    {
        GUI.Box(centerBox, GUIContent.none);
    }

    void FixedUpdate()
    {
        if (centerBox.Contains(cursor.mousePos) && !Input.GetMouseButton(1))
        {
            ResetCameraPosition();
            moveSwordXY();
        }
        else
        {
            // Outside the center box: rotate the player
            //RotatePlayerWithMouse();
        }
    }

    private void ResetCameraPosition()
    {
        if (playerCamera.transform.localRotation != initialCameraRotation)
            playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, initialCameraRotation, Time.deltaTime * 5f);
    }

    void RotatePlayerWithMouse()
    {
        float screenCenterX = Screen.width / 2;
        float distanceFromCenter = cursor.mousePos.x - screenCenterX;
     
        // Adjust speed by how far the mouse is from the center
        float rotationSpeedMultiplier = (Mathf.Abs(distanceFromCenter)) / screenCenterX;

        // Rotate left if the mouse is left of the center, right if to the right
        float rotationDirection = Mathf.Sign(distanceFromCenter);

        // Apply rotation
        currentYRotation += rotationDirection * 500 * rotationSpeedMultiplier * Time.deltaTime;
        Quaternion targetRotation = Quaternion.Euler(0f, currentYRotation, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);  // Smooth horizontal rotation

        //xRotation -= mouseY;
        //xRotation = Mathf.Clamp(xRotation, 0f, 15);
        //playerCamera.transform.localRotation = Quaternion.Lerp(playerCamera.transform.localRotation, Quaternion.Euler(xRotation, 0f, 0f), Time.deltaTime * 5f); // Smooth vertical rotation

    }

    void moveSwordXY()
    {
        (float angle, float distance) = GetMouseAngleFromCenter();
        Debug.Log($"angle: {angle}, distance: {distance}");
        int xdir = Math.Abs(angle)>90 ? -1 : 1;
        int ydir = angle > 0 ? 1 : -1;
        SwordCOG.transform.localPosition = new Vector3(xdir*(distance/500f), ydir*(distance / 500f), SwordCOG.transform.localPosition.z);

        //mouseWorldPosition = playerCamera.ScreenToWorldPoint(cursor.mousePos);
        //Vector3 targetLocalPosition = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, SwordCOG.transform.localPosition.z);
        //SwordCOG.transform.localPosition = targetLocalPosition;
    }

    (float angle, float distance) GetMouseAngleFromCenter()
    {
        // Get the center of the screen
        Vector2 screenCenter = new Vector2(Screen.width / 2f, (Screen.height / 2f) - 150);

        // Calculate the direction from the center of the screen to the mouse position
        Vector2 direction = cursor.mousePos - (Vector3)screenCenter;

        // Calculate the distance
        float distance = direction.magnitude;

        // Calculate the angle (in degrees) using Atan2
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return (angle, distance);
    }

    struct Point
    {
        public float x;
        public float y;

        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    struct Line
    {
        //a point begins at p1 and ends at p3
        //p1 ----------> p3
        public Point? p1;
        public Point? p2;
        public Point? p3;

        public void AppendPoint(Point? p)
        {
            if (p1 == null)
                p1 = p;
            else if(p2 == null)
                p2 = p;
            else if(p3 == null)
                p3 = p;
            else
            {
                p1 = p2;
                p2 = p3;
                p3 = p;
            }
        }

        public float GetAngle()
        {
            if (p1 == null || p2 == null || p3 == null)
            {
                //throw new InvalidOperationException("Not enough points to calculate the angle.");
                return -1;
            }

            // Vector A (from p2 to p1)
            float Ax = p1.Value.x - p2.Value.x;
            float Ay = p1.Value.y - p2.Value.y;

            // Vector B (from p2 to p3)
            float Bx = p3.Value.x - p2.Value.x;
            float By = p3.Value.y - p2.Value.y;

            // Dot product of vectors A and B
            float dotProduct = (Ax * Bx) + (Ay * By);

            // Magnitude of vector A and vector B
            float magnitudeA = (float)Math.Sqrt(Ax * Ax + Ay * Ay);
            float magnitudeB = (float)Math.Sqrt(Bx * Bx + By * By);

            // Calculate the cosine of the angle
            float cosTheta = dotProduct / (magnitudeA * magnitudeB);

            // Convert the cosine to an angle in degrees
            float angleInRadians = (float)Math.Acos(cosTheta);
            float angleInDegrees = angleInRadians * (180f / (float)Math.PI);

            return angleInDegrees;
        }
    }
}
