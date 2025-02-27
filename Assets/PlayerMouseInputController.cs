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

        cursorLine.AppendPoint(new Vector2(cursor.mousePos.x, cursor.mousePos.y));

        float distanceToCenter = Vector2.Distance(centerBox.center, cursor.mousePos);
        if (distanceToCenter <= 550 && !Input.GetMouseButton(1))
        {
            inCircle = true;
            ResetCameraPosition();

            moveSwordXY();
            RotateSwordXY(GetMouseAngleFromCenter());



            if (frameCounter % 5 == 0)
            {
                Vector2? latestPoint = cursorLine.GetLatestPoint();
                //CreateDot(latestPoint.Value);
                //Debug.Log(cursorLine.GetCurvedLineDirection());

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
            RotatePlayerWithMouse();
        }
    }

    public void CreateDot(Vector2 screenPosition)
    {
        // Instantiate the dot prefab
        GameObject newDot = Instantiate(dotPrefab, canvas.transform);

        // Set the dot's position (screen space)
        RectTransform rectTransform = newDot.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector3(screenPosition.x, screenPosition.y, 0); // This assumes your Canvas is set to Screen Space - Overlay
    }

    private void RotateSwordXY(float angle = 0)
    {
        Debug.Log(angle);
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, -1*angle);
        SwordRotationXY.transform.localRotation = Quaternion.RotateTowards(SwordRotationXY.transform.localRotation, targetRotation, 1000 * Time.deltaTime);
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
        var x = (cursor.mousePos.x - Screen.width / 2f) / 600;
        var y = (cursor.mousePos.y - Screen.height / 2f + 150) / 600;

        var targetPos = new Vector3(x, y, SwordCOG.transform.localPosition.z);
        if (!inCircle)
        {
            SwordCOG.transform.DOLocalMove(targetPos, 0.1f);
        }
        else
        {
            SwordCOG.transform.localPosition = Vector3.Lerp(SwordCOG.transform.localPosition, targetPos, 5f * Time.deltaTime);
        }
    }

    public float GetMouseAngleFromCenter()
    {
        // Get the screen center
        Vector2 screenCenter = new Vector2((Screen.width / 2f), (Screen.height / 2f)-150);

        // Get the mouse position
        Vector2 mousePos = Input.mousePosition;

        // Calculate the vector from the center of the screen to the mouse position
        Vector2 direction = mousePos - screenCenter;

        // Get the angle in radians (Mathf.Atan2 returns the angle in radians)
        float angleRadians = Mathf.Atan2(direction.y, direction.x);

        // Convert the angle to degrees
        float angleDegrees = angleRadians * Mathf.Rad2Deg;

        // Adjust the angle so that 0° is "up" (north)
        // Atan2 returns 0° at right (east), so we subtract 90° to make 0° point up
        angleDegrees -= 90f;

        // Return the angle, which is now correctly oriented:
        // 0° is up, 90° is right, -90° is left
        //Debug.Log(angleDegrees);
        return angleDegrees;
    }


    struct Line
    {
        Queue<Vector2?> que;

        public Line(bool b = true)
        {
            que = new Queue<Vector2?>();
        }

        public void AppendPoint(Vector2? p)
        {
            if (que.Count >= 3)
            {
                que.Dequeue();
            }

            que.Enqueue(p);
        }

        public Vector2? GetLatestPoint()
        {
            if (que.Count == 0)
                return null;

            return que.ToArray()[que.Count - 1];
        }

        public float GetCurvedLineDirection()
        {
            if (que.Count < 3)
                throw new InvalidOperationException("Not enough points to calculate the angle.");

            Vector2?[] points = que.ToArray();
            Vector2 p1 = points[0].Value;
            Vector2 p2 = points[1].Value;
            Vector2 p3 = points[2].Value;

            // Vector from p2 to p1
            Vector2 v1 = p1 - p2;

            // Vector from p2 to p3
            Vector2 v2 = p3 - p2;

            // Use the cross product to determine the direction of the curve
            float cross = v1.x * v2.y - v1.y * v2.x;

            // Return positive for clockwise, negative for counterclockwise
            return cross;
        }
    }
}
