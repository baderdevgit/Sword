using System;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class PlayerMouseInputController : MonoBehaviour
{
    [Header("Referenced Objects")]
    public GameObject SwordCOG;
    public CursorIcon cursor;
    public GameObject SwordRotationXY;
    public GameObject SwordExtension;

    [Header("Temp")]
    public GameObject canvas;
    public GameObject dotPrefab;

    [Header("Player Rotation Variables")]
    public Camera playerCamera;
    public float mouseSensitivityX = 750f; 
    public float mouseSensitivityY = 1000f; 
    float xRotation = 0f;
    float currentYRotation = 0f;
    Rect centerBox;
    float centerBoxSize = 800f;
    int radius = 550;

    [Header("Sword Variables")]
    public float followSpeed = 2f;
    public float delayFactor = 0.1f;
    private Vector3 mouseWorldPosition;
    private Vector3 initialSwordPosition;
    private Quaternion initialCameraRotation;
    private GUIStyle boxStyle;
    Line cursorLine;
    SwordMovement SwordMovementHandler;

    //other stuff
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

        SwordMovementHandler = new SwordMovementVersion1(SwordCOG, cursor, SwordRotationXY, SwordExtension);

        cursorLine = new Line(true);
    }

    void FixedUpdate()
    {
        frameCounter++;
        cursorLine.AppendPoint(new Vector2(cursor.mousePos.x, cursor.mousePos.y));
        float distanceToCenter = Vector2.Distance(centerBox.center, cursor.mousePos);

        //sword and camera movement
        if (distanceToCenter <= radius && !Input.GetMouseButton(1)) //if inside the circle move da sword
        {
            if (frameCounter % 5 == 0)
            {
                Vector2? latestPoint = cursorLine.GetLatestPoint();
            }

            inCircle = true;
            ResetCameraPosition();
            SwordMovementHandler.MoveSword(inCircle);

            if (frameCounter >= long.MaxValue)
                frameCounter = 0;
        }
        else //mouse is outside the circle
        {
            if(!Input.GetMouseButton(1))
            {
                if (inCircle) //Moves the sword to edge of circle if it goes out of bounds
                {
                    inCircle = false;
                    SwordMovementHandler.MoveSwordXY(false);
                }
                RotatePlayerWithMouse();
            }
            else
            {
                //TODO new player rotation
            }
        }

        //sword extension
        if (Input.GetMouseButton(0))
        {
            SwordMovementHandler.Extend();
        }
        else
        {
            SwordMovementHandler.Retract();
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

    public void CreateDot(Vector2 screenPosition)
    {
        // Instantiate the dot prefab
        GameObject newDot = Instantiate(dotPrefab, canvas.transform);

        // Set the dot's position (screen space)
        RectTransform rectTransform = newDot.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector3(screenPosition.x, screenPosition.y, 0); // This assumes your Canvas is set to Screen Space - Overlay
    }
}
