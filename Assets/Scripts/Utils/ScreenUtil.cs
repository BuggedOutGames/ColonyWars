using UnityEngine;

public static class ScreenUtil {

    public static Rect GetScreenRect(Vector2 screenPositionOne, Vector2 screenPositionTwo) {
        // Screen space has origin at bottom left, whereas Rect has origin in top left
        Vector2 rectPositionOne = new Vector2(screenPositionOne.x, Screen.height - screenPositionOne.y);
        Vector2 rectPositionTwo = new Vector2(screenPositionTwo.x, Screen.height - screenPositionTwo.y);
        // Calculate corners
        Vector2 topLeft = Vector2.Min(rectPositionOne, rectPositionTwo);
        Vector2 bottomRight = Vector2.Max(rectPositionOne, rectPositionTwo);
        // Create Rect
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }
}
