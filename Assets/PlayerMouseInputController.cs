using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMouseInputController : MonoBehaviour
{
    public Transform cameraTransform;
    public float mouseSensitivityX = 500f; //UpDown
    public float mouseSensitivityY = 1000f; //LeftRight
    float xRotation = 0f;
    float currentYRotation = 0f;
    Line MouseLine;

    int fixedUpdateCounter = 0;
    private float mouseX;
    private float mouseY;

    void Start()
    {
        MouseLine = new Line();
        //Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
        GetMouseInput();

        //fixedUpdateCounter++;
        //if (fixedUpdateCounter % 2 == 0)
        //{
        //    SetMouseLine();
        //}
        HandleCameraInputs();
    }

    public void GetMouseInput()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;
    }

    private void ResetCameraPosition()
    {
        if (cameraTransform.localRotation != Quaternion.Euler(15f, 0f, 0f))
            cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, Quaternion.Euler(15f, 0f, 0f), Time.deltaTime * 5f);
    }

    public void SetMouseLine()
    {
        Point p = new Point(mouseX, mouseY);
        MouseLine.AppendPoint(p);

        float angle = MouseLine.GetAngle();
        if (!float.IsNaN(angle))
            Debug.Log($"Angle: {angle}");
    }

    public void HandleCameraInputs()
    {
        Debug.Log($"X: {mouseX}, Y: {mouseY}");
        RotatePlayerCamera();
    }

    void RotatePlayerCamera()
    {
        currentYRotation += mouseX;
        Quaternion targetRotation = Quaternion.Euler(0f, currentYRotation, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);  // Smooth horizontal rotation

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, 5f, 15);
        cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, Quaternion.Euler(xRotation, 0f, 0f), Time.deltaTime * 5f); // Smooth vertical rotation
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
