using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class FlowField : MonoBehaviour, IEnumerable {

    public float CellSize;
    public Vector2Int Dimensions;
    
    private Grid _grid = new Grid(new Vector2Int(0, 0), 0);

    private void Update() {
        if (Application.isEditor) {
            if (Dimensions != _grid.Dimensions || !CellSize.Equals(_grid.CellSize)) {
                _grid = new Grid(Dimensions, CellSize);
            }
        }
    }

    public Grid.GridCell? GetGridCellAtWorldPosition(Vector2 worldPosition) {
        return _grid.GetCellAtWorldPosition(worldPosition);
    }

    public IEnumerator GetEnumerator() {
        return _grid.GetEnumerator();
    }
}
