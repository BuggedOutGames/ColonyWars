using UnityEngine;

public static class GraphicsUtil {
    
    public static void DrawRect(Rect rect, Color color) {
        GUI.color = color;
        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();    
        GUI.DrawTexture(rect, texture);
        GUI.color = Color.white;
    }

    public static void DrawRectBorder(Rect rect, Color color, float thickness) {
        // Top
        DrawRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Left
        DrawRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Right
        DrawRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Bottom
        DrawRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }
}
