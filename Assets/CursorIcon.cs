using UnityEngine;

public class CursorIcon : MonoBehaviour
{
    public Vector3 mousePos;

    public RectTransform customCursor;  // Reference to the UI element for the custom cursor
    float followSpeed = 10f;      // Speed at which the custom cursor catches up to the real cursor

    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        mousePos = Input.mousePosition;
        customCursor.anchoredPosition = Vector3.Lerp(customCursor.anchoredPosition, new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0), followSpeed * Time.deltaTime);
    }
}