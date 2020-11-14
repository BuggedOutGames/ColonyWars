using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FlowField {
    public class Grid: IEnumerable {
        public class GridCell {
            public int TerrainCost;
            public int IntegrationCost;
            public Vector2? DirectionVector;
            public readonly Vector2Int GridIndex;
            public readonly Vector2 WorldPosition;
            public GridCell(Vector2Int gridIndex, Vector2 worldPosition) {
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
                    grid[x, y] = new GridCell(cellIndex, GetWorldPosition(cellIndex));
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

        private GridCell GetCellAtIndex(Vector2Int cellIndex) {
            if (cellIndex.x >= 0 && cellIndex.x < Dimensions.x && cellIndex.y >= 0 && cellIndex.y < Dimensions.y) {
                return grid[cellIndex.x, cellIndex.y];
            } else {
                return null;
            }

        }

        public GridCell GetCellAtWorldPosition(Vector2 worldPosition) {
            var cellIndex = GetCellIndex(worldPosition);
            return GetCellAtIndex(cellIndex);
        }

        public List<GridCell> GetNeighborCells(Vector2Int cellIndex) {
            var originCell = GetCellAtIndex(cellIndex);
            var neighborCells = new List<GridCell>();
            if (originCell != null) {
                var northCell = GetCellAtIndex(new Vector2Int(cellIndex.x, cellIndex.y - 1));
                var northEastCell = GetCellAtIndex(new Vector2Int(cellIndex.x + 1, cellIndex.y - 1));
                var northWestCell = GetCellAtIndex(new Vector2Int(cellIndex.x - 1, cellIndex.y - 1));
                var southCell = GetCellAtIndex(new Vector2Int(cellIndex.x, cellIndex.y + 1));
                var southEastCell = GetCellAtIndex(new Vector2Int(cellIndex.x + 1, cellIndex.y + 1));
                var southWestCell = GetCellAtIndex(new Vector2Int(cellIndex.x - 1, cellIndex.y + 1));
                var eastCell = GetCellAtIndex(new Vector2Int(cellIndex.x + 1, cellIndex.y));
                var westCell = GetCellAtIndex(new Vector2Int(cellIndex.x - 1, cellIndex.y));
                
                if (northCell != null) neighborCells.Add(northCell);
                if (northEastCell != null) neighborCells.Add(northEastCell);
                if (northWestCell != null) neighborCells.Add(northWestCell);
                if (southCell != null) neighborCells.Add(southCell);
                if (southEastCell != null) neighborCells.Add(southEastCell);
                if (southWestCell != null) neighborCells.Add(southWestCell);
                if (eastCell != null) neighborCells.Add(eastCell);
                if (westCell != null) neighborCells.Add(westCell);
            }
            return neighborCells;
        }
        
        public IEnumerator GetEnumerator() {
            return grid.Cast<GridCell>().GetEnumerator();
        }
    }
}
