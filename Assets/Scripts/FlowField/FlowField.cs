using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FlowField {

    private readonly Vector2 goal;
    private readonly Grid grid;

    public FlowField(float cellSize, Vector2Int dimensions) {
        grid = new Grid(dimensions, cellSize);
        CalculateTerrainCostField();
    }
    
    public Vector2 GetFlowDirection(Vector2 worldPosition) {
        var cell = grid.GetCellAtWorldPosition(worldPosition);
        return cell?.DirectionVector ?? Vector2.zero;
    }
    
    public void CalculateFlowField(Vector2 worldPosition) {
        CalculateIntegrationCostField(worldPosition);
        CalculateFlowField();
    }
    
    public void DisplayFlowField() {
        foreach (Grid.GridCell cell in grid) {
            var cellRect = new Rect(
                new Vector2(cell.WorldPosition.x - grid.CellSize / 2, cell.WorldPosition.y - grid.CellSize / 2), 
                new Vector2(grid.CellSize, grid.CellSize)
            );
            var green = new Color(0, 255, 0, 0.1f);
            var red = new Color(255, 0, 0, 0.1f);
            var cellColor = Color.Lerp(green, red, cell.IntegrationCost * 0.00004f);

            Handles.color = Color.white;
            Handles.DrawSolidRectangleWithOutline(cellRect, cellColor, cellColor);
            
            if (cell.DirectionVector.HasValue) {
                Handles.color = Color.cyan;
                Handles.ArrowHandleCap(
                    0, cell.WorldPosition, 
                    Quaternion.LookRotation(cell.DirectionVector.Value), grid.CellSize / 3, 
                    EventType.Repaint
                );
            }
        }
    }
    
    private void CalculateTerrainCostField() {
        foreach (Grid.GridCell cell in grid) {
            int layerMask = LayerMask.GetMask("Terrain");
            RaycastHit2D raycastHit = Physics2D.Raycast(new Vector3(cell.WorldPosition.x, cell.WorldPosition.y), Vector3.forward, 10f, layerMask);
            if (raycastHit.collider != null) {
                var flowFieldBehaviour = raycastHit.transform.gameObject.GetComponent<FlowFieldBehaviour>();
                if (flowFieldBehaviour != null) {
                    cell.TerrainCost = flowFieldBehaviour.Cost;
                    continue;
                }
            }
            cell.TerrainCost = 255;
        }
    }

    private void CalculateIntegrationCostField(Vector2 worldPosition) {
        var goalCell = grid.GetCellAtWorldPosition(worldPosition);
        if (goalCell != null && goalCell.TerrainCost != 255) {
            goalCell.IntegrationCost = 0;
            foreach (Grid.GridCell cell in grid) {
                if (cell != goalCell) {
                    cell.IntegrationCost = int.MaxValue;
                }
            }
            var frontierCells = new Queue<Grid.GridCell>();
            frontierCells.Enqueue(goalCell);
            while (frontierCells.Count > 0) {
                var currentCell = frontierCells.Dequeue();
                var neighborCells = grid.GetNeighborCells(currentCell.GridIndex);
                foreach (var neighborCell in neighborCells) {
                    if (neighborCell.TerrainCost != 255) {
                        var directionCost = neighborCell.GridIndex.x != currentCell.GridIndex.x && neighborCell.GridIndex.y != currentCell.GridIndex.y ? 14 : 10;
                        var newIntegrationCost = currentCell.IntegrationCost + neighborCell.TerrainCost + directionCost;
                        if (newIntegrationCost < neighborCell.IntegrationCost) {
                            neighborCell.IntegrationCost = newIntegrationCost;
                            if (!frontierCells.Contains(neighborCell)) {
                                frontierCells.Enqueue(neighborCell);
                            }
                        }
                    }
                }
            }
        }
    }

    private void CalculateFlowField() {
        foreach (Grid.GridCell cell in grid) {
            if (cell.IntegrationCost == 0 || cell.IntegrationCost == int.MaxValue) {
                cell.DirectionVector = null;
            } else {
                var shortestPath = grid
                    .GetNeighborCells(cell.GridIndex)
                    .Aggregate((shortestPathCell, nextNeighbor) => {
                        if (shortestPathCell == null) {
                            return nextNeighbor;
                        } else {
                            return shortestPathCell.IntegrationCost < nextNeighbor.IntegrationCost
                                ? shortestPathCell
                                : nextNeighbor;
                        }
                    });
                cell.DirectionVector = (shortestPath.WorldPosition - cell.WorldPosition).normalized;
            } 
        }
    }
}