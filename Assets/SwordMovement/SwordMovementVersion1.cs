using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class SwordMovementVersion1 : SwordMovement
{
    public GameObject SwordCOG;
    public CursorIcon Cursor;
    public GameObject SwordRotationXY;
    public GameObject SwordExtension;

    public SwordMovementVersion1(GameObject swordCOG, CursorIcon cursor, GameObject swordRotationXY, GameObject swordExtension)
    {
        SwordCOG = swordCOG;
        Cursor = cursor;
        SwordRotationXY = swordRotationXY;
        SwordExtension = swordExtension;
    }

    public override void MoveSword(bool cursorInCircle)
    {
        MoveSwordXY(cursorInCircle);
        RotateSwordXY(GetMouseAngleFromCenter());
    }

    public override void MoveSwordXY(bool cursorInCircle)
    {
        var x = (Cursor.mousePos.x - Screen.width / 2f) / 600;
        var y = (Cursor.mousePos.y - Screen.height / 2f + 150) / 600;

        var targetPos = new Vector3(x, y, SwordCOG.transform.localPosition.z);
        if (!cursorInCircle)
        {
            SwordCOG.transform.DOLocalMove(targetPos, 0.1f);
        }
        else
        {
            SwordCOG.transform.localPosition = Vector3.Lerp(SwordCOG.transform.localPosition, targetPos, 5f * Time.deltaTime);
        }
    }

    public override void RotateSwordXY(float angle = 0)
    {
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, -1 * angle);
        SwordRotationXY.transform.localRotation = Quaternion.RotateTowards(SwordRotationXY.transform.localRotation, targetRotation, 1000 * Time.deltaTime);
    }

    public float GetMouseAngleFromCenter()
    {
        // Get the screen center
        Vector2 screenCenter = new Vector2((Screen.width / 2f), (Screen.height / 2f) - 150);

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

    public override void Extend()
    {
        Quaternion targetRotation = Quaternion.Euler(-120f, 0f, 0);
        SwordExtension.transform.localRotation = Quaternion.RotateTowards(SwordExtension.transform.localRotation, targetRotation, 1000 * Time.deltaTime);
    }

    public override void Retract()
    {
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, 0);
        SwordExtension.transform.localRotation = Quaternion.RotateTowards(SwordExtension.transform.localRotation, targetRotation, 1000 * Time.deltaTime);
    }
}

