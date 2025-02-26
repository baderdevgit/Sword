using System;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections.Generic;

public class PlayerMouseInputController : MonoBehaviour
{
    [Header("Referenced Objects")]
    public GameObject SwordCOG;
    public CursorIcon cursor;
    public GameObject SwordRotationXY;

    [Header("Temp")]
    public GameObject canvas;
    public GameObject dotPrefab;

    [Header("Player Rotation Variables")]
    public Camera playerCamera;
    public float mouseSensitivityX = 500f; //UpDown
    public float mouseSensitivityY = 1000f; //LeftRight
    float xRotation = 0f;
    float currentYRotation = 0f;
    Rect centerBox;
    float centerBoxSize = 800f;

    [Header("Sword Variables")]
    public float followSpeed = 2f; 
    public float delayFactor = 0.1f;
    private Vector3 mouseWorldPosition;
    private Vector3 initialSwordPosition;
    private Quaternion initialCameraRotation;
    private GUIStyle boxStyle;
    Line cursorLine;

    bool inCircle = true;
    long frameCounter = 0;

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

        cursorLine = new Line(true);
    }


    void FixedUpdate()
    {
        frameCounter++;

        cursorLine.AppendPoint(new Point(cursor.mousePos.x, cursor.mousePos.y));

        float distanceToCenter = Vector2.Distance(centerBox.center, cursor.mousePos);
        if (distanceToCenter <= 550 && !Input.GetMouseButton(1))
        {
            inCircle = true;
            ResetCameraPosition();

            moveSwordXY();

            if(frameCounter % 10 == 0)
            {
                //CreateDot(new Vector2(cursorLine.p3.Value.x, cursorLine.p3.Value.y));
                var angle = cursorLine.GetAngle();
                Debug.Log(angle);
            }

            if (frameCounter >= long.MaxValue)
                frameCounter = 0;
        }
        else
        {
            if (inCircle && !Input.GetMouseButton(1))
            {
                inCircle = false;
                moveSwordXY(false);
            }
            // Outside the center box: rotate the player
            RotatePlayerWithMouse();
        }
    }

    public void CreateDot(Vector2 screenPosition)
    {
        Debug.Log(screenPosition);

        // Instantiate the dot prefab
        GameObject newDot = Instantiate(dotPrefab, canvas.transform);

        // Set the dot's position (screen space)
        RectTransform rectTransform = newDot.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector3(screenPosition.x, screenPosition.y, 0); // This assumes your Canvas is set to Screen Space - Overlay
    }

    private void rotateSwordXY(float angle)
    {
        if (float.IsNaN(angle))
        {
            Debug.Log("NAN");
            return;
        }

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, 180f);
        SwordRotationXY.transform.localRotation = Quaternion.RotateTowards(SwordRotationXY.transform.localRotation, targetRotation, 10 * angle * Time.deltaTime);
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

    void moveSwordXY(bool inCircle = true)
    {
        var x = (cursor.mousePos.x - Screen.width/2f) / 600;
        var y = (cursor.mousePos.y - Screen.height / 2f + 150) / 600;

        var targetPos = new Vector3(x, y, SwordCOG.transform.localPosition.z);
        if (!inCircle )
        {
            SwordCOG.transform.DOLocalMove(targetPos, 0.1f);
        }
        else
        {
            SwordCOG.transform.localPosition = Vector3.Lerp(SwordCOG.transform.localPosition, targetPos, 5f * Time.deltaTime);
        }
        
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
        Queue<Point?> que;

        public Line(bool b = true)
        {
            que = new Queue<Point?>();
        }

        public void AppendPoint(Point? p)
        {
            if (que.Count >= 3)
            {
                que.Dequeue();
            }
            else
            {
                que.Enqueue(p);
            }
        }

        public float GetAngle()
        {
            Point? p1;
            Point? p2;
            Point? p3;
            try
            {
                var arr = que.ToArray();
                p1 = arr[0];
                p2 = arr[1];
                p3 = arr[2];

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

                // Calculate the cross product of A and B (Z-component in 2D)
                float crossProductZ = (Ax * By) - (Ay * Bx);

                // If crossProductZ > 0, it's counterclockwise, else it's clockwise
                if (crossProductZ < 0)
                {
                    // Clockwise, make the angle negative
                    angleInDegrees = -angleInDegrees;
                }

                return angleInDegrees;
            }
            catch (Exception e)
            {
                //line not full;
            }
            return -1;
        }


        //public override string ToString()
        //{
        //    return $"p1: x:{p1.Value.x} y:{p1.Value.y} - p2: x:{p2.Value.x} y:{p2.Value.x} - p3: x:{p3.Value.x} y:{p3.Value.x}";
        //}
    }
}
