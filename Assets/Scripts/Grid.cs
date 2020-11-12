using System.Collections;
using System.Linq;
using UnityEngine;

public class Grid: IEnumerable {
    public class GridCell {
        public int Cost;
        public readonly Vector2Int GridIndex;
        public readonly Vector2 WorldPosition;
        public GridCell(int cost, Vector2Int gridIndex, Vector2 worldPosition) {
            Cost = cost;
            GridIndex = gridIndex;
            WorldPosition = worldPosition;
        }
    }
    
    public readonly float CellSize;
    public readonly Vector2Int Dimensions;
    private readonly GridCell[,] grid;

    public Grid(Vector2Int dimensions, float cellSize) {
        CellSize = cellSize;
        Dimensions = dimensions;
        grid = new GridCell[dimensions.x, dimensions.y];
        foreach (var x in Enumerable.Range(0, dimensions.x)) {
            foreach (var y in Enumerable.Range(0, dimensions.y)) {
                var cellIndex = new Vector2Int(x, y);
                grid[x, y] = new GridCell(int.MaxValue, cellIndex, GetWorldPosition(cellIndex));
            }
        }
    }

    private Vector2 GetWorldPosition(Vector2Int cellIndex) {
        return new Vector2(
            cellIndex.x * CellSize - Dimensions.x * CellSize / 2 + CellSize / 2, 
            Dimensions.y * CellSize - (cellIndex.y * CellSize + Dimensions.y * CellSize / 2 + CellSize / 2)
        );
    }
    
    private Vector2Int GetCellIndex(Vector2 worldPosition) {
        return new Vector2Int(
            (int) Mathf.Floor(worldPosition.x / CellSize + Dimensions.x/2f) , 
            (int) Mathf.Floor(-worldPosition.y / CellSize + Dimensions.y/2f)
        );
    }

    public GridCell GetCellAtWorldPosition(Vector2 worldPosition) {
        var cellIndex = GetCellIndex(worldPosition);
        if (cellIndex.x >= 0 && cellIndex.x < Dimensions.x && cellIndex.y >= 0 && cellIndex.y < Dimensions.y) {
            return grid[cellIndex.x, cellIndex.y];
        } else {
            return null;
        }
    }
    
    public IEnumerator GetEnumerator() {
        return grid.Cast<GridCell>().GetEnumerator();
    }
}
