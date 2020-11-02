using UnityEngine;

public class Utils : MonoBehaviour {
    
    public static Utils instance;
    private Texture2D whiteTexture;


    void Awake() {
        if (instance == null) {
            instance = InitInstance();
        } else if (instance != this) {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private Utils InitInstance() {
        this.whiteTexture = new Texture2D(1, 1);
        this.whiteTexture.SetPixel(0, 0, Color.white);
        this.whiteTexture.Apply();    
        return this;
    }

    public RaycastHit2D CastRayAtMousePosition() {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return Physics2D.Raycast(mousePosition, Vector2.zero);
    }

    public Vector2 GetMousePositionInWorld() {
        RaycastHit2D mouseRayCast = CastRayAtMousePosition();
        return mouseRayCast.point;
    }

    public void DrawRect(Rect rect, Color color) {
        GUI.color = color;
        GUI.DrawTexture(rect, whiteTexture);
        GUI.color = Color.white;
    }

    public void DrawRectBorder(Rect rect, Color color, float thickness) {
        // Top
        DrawRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Left
        DrawRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Right
        DrawRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Bottom
        DrawRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    public Rect GetScreenRect(Vector2 screenPositionOne, Vector2 screenPositionTwo) {
        // Screen space has origin at bottom left, whereas Rect has origin in top left
        Vector2 rectPositionOne = new Vector2(screenPositionOne.x, Screen.height - screenPositionOne.y);
        Vector2 rectPositionTwo = new Vector2(screenPositionTwo.x, Screen.height - screenPositionTwo.y);
        // Calculate corners
        Vector2 topLeft = Vector2.Min(rectPositionOne, rectPositionTwo);
        Vector2 bottomRight = Vector2.Max(rectPositionOne, rectPositionTwo);
        // Create Rect
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    public Rect GetWorldRect(Vector2 screenPositionOne, Vector2 screenPositionTwo) {
        Vector2 worldPositionOne = Camera.main.ScreenToWorldPoint(screenPositionOne);
        Vector2 worldPositionTwo = Camera.main.ScreenToWorldPoint(screenPositionTwo);

        Vector2 topLeft = Vector2.Min(worldPositionOne, worldPositionTwo);
        Vector2 bottomRight = Vector2.Max(worldPositionOne, worldPositionTwo);

        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }
}