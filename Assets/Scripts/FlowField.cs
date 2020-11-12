using System.Collections;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class FlowField : MonoBehaviour, IEnumerable {

    public bool displayFlowField;
    public float CellSize;
    public Vector2Int Dimensions;
    
    private Grid grid = new Grid(new Vector2Int(0, 0), 0);

    private void Update() {
        if (Application.isEditor) {
            grid = new Grid(Dimensions, CellSize);
            UpdateCellCosts();
        }
    }

    private void OnDrawGizmos() {
        if (displayFlowField) {
            DisplayFlowField();
        }
    }
    
    private void DisplayFlowField() {
        foreach (Grid.GridCell cell in grid) {
            var wireCubePosition = new Vector3(cell.WorldPosition.x, cell.WorldPosition.y, -1);
            var cellRect = new Rect(new Vector2(cell.WorldPosition.x - grid.CellSize / 2, cell.WorldPosition.y - grid.CellSize / 2), new Vector2(grid.CellSize, grid.CellSize));
            var textPosition = new Vector2(cell.WorldPosition.x - grid.CellSize / 2 + 0.05f,
                cell.WorldPosition.y + grid.CellSize / 2 - 0.05f);
            var textStyle = new GUIStyle {
                normal = {textColor = Color.white},
            };
            var costColorFactor = cell.Cost * 0.01f;
            Color green = new Color(0, 255, 0, 0.1f);
            Color red = new Color(255, 0, 0, 0.1f);
            Color cellColor = Color.Lerp(green, red, costColorFactor);
            
            Handles.DrawSolidRectangleWithOutline(cellRect, cellColor, cellColor);
            Handles.DrawWireCube(wireCubePosition, new Vector3(grid.CellSize, grid.CellSize, 0));
            Handles.Label(textPosition, $"Cost: {cell.Cost}", textStyle);
        }
    }

    private void UpdateCellCosts() {
        foreach (Grid.GridCell cell in grid) {
            int layerMask = LayerMask.GetMask("Terrain");
            RaycastHit2D raycastHit = Physics2D.Raycast(new Vector3(cell.WorldPosition.x, cell.WorldPosition.y, -10), Vector3.forward, 10f, layerMask);
            if (raycastHit.collider != null) {
                FlowFieldBehaviour flowFieldBehaviour = raycastHit.transform.gameObject.GetComponent<FlowFieldBehaviour>();
                if (flowFieldBehaviour != null) {
                    cell.Cost = flowFieldBehaviour.Cost;
                    continue;
                }
            }
            cell.Cost = 255;
        }
    }
    
    public Grid.GridCell GetGridCellAtWorldPosition(Vector2 worldPosition) {
        return grid.GetCellAtWorldPosition(worldPosition);
    }

    public IEnumerator GetEnumerator() {
        return grid.GetEnumerator();
    }
}
