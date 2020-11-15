using UnityEngine;

namespace Utils {
    public static class ProjectionUtil {
        public static RaycastHit2D CastRayFromScreenToWorld(Camera camera, Vector2 screenPosition) {
            var worldPosition = camera.ScreenToWorldPoint(screenPosition);
            return Physics2D.Raycast(worldPosition, Vector2.zero);
        }

        public static Vector2 GetPositionInWorld(Camera camera, Vector2 screenPosition) {
            var worldRayCast = CastRayFromScreenToWorld(camera, screenPosition);
            return worldRayCast.point;
        }

        public static Rect GetWorldRect(Camera camera, Vector2 screenPositionOne, Vector2 screenPositionTwo) {
            Vector2 worldPositionOne = camera.ScreenToWorldPoint(screenPositionOne);
            Vector2 worldPositionTwo = camera.ScreenToWorldPoint(screenPositionTwo);

            var topLeft = Vector2.Min(worldPositionOne, worldPositionTwo);
            var bottomRight = Vector2.Max(worldPositionOne, worldPositionTwo);

            return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
        }
    }
}