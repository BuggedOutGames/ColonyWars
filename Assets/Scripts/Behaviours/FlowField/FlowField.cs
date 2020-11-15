using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Behaviours.Terrain;
using UnityEditor;
using UnityEngine;

namespace Behaviours.FlowField {
    public class FlowField {
        
        private readonly FlowFieldGrid flowFieldGrid;
        private readonly Vector2 goal;

        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        public FlowField(float cellSize, Vector2Int dimensions) {
            flowFieldGrid = new FlowFieldGrid(dimensions, cellSize);
            CalculateTerrainCostField();
        }

        public Vector2 GetFlowDirection(Vector2 worldPosition) {
            var cell = flowFieldGrid.GetCellAtWorldPosition(worldPosition);
            return cell?.DirectionVector ?? Vector2.zero;
        }

        public void CalculateFlowField(Vector2 worldPosition) {
            CalculateIntegrationCostField(worldPosition);
            CalculateFlowField();
        }

        public void DisplayFlowField() {
            foreach (FlowFieldGrid.FlowFieldCell cell in flowFieldGrid) {
                var cellRect = new Rect(
                    new Vector2(cell.WorldPosition.x - flowFieldGrid.CellSize / 2,
                        cell.WorldPosition.y - flowFieldGrid.CellSize / 2),
                    new Vector2(flowFieldGrid.CellSize, flowFieldGrid.CellSize)
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
                        Quaternion.LookRotation(cell.DirectionVector.Value), flowFieldGrid.CellSize / 3,
                        EventType.Repaint
                    );
                }
            }
        }

        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeNullComparison")]
        private void CalculateTerrainCostField() {
            foreach (FlowFieldGrid.FlowFieldCell cell in flowFieldGrid) {
                var layerMask = LayerMask.GetMask("Terrain");
                var raycastHit = Physics2D.Raycast(
                    new Vector3(cell.WorldPosition.x, cell.WorldPosition.y),
                    Vector3.forward, 10f, layerMask
                );
                if (raycastHit.collider != null) {
                    var terrainBehaviour = raycastHit.transform.gameObject.GetComponent<TerrainBehaviour>();
                    if (terrainBehaviour != null) {
                        cell.TerrainCost = terrainBehaviour.MovementPenalty;
                        continue;
                    }
                }

                cell.TerrainCost = 255;
            }
        }

        private void CalculateIntegrationCostField(Vector2 worldPosition) {
            var goalCell = flowFieldGrid.GetCellAtWorldPosition(worldPosition);
            if (goalCell != null && goalCell.TerrainCost != 255) {
                goalCell.IntegrationCost = 0;
                foreach (FlowFieldGrid.FlowFieldCell cell in flowFieldGrid) {
                    if (cell != goalCell) {
                        cell.IntegrationCost = int.MaxValue;
                    }
                }

                var frontierCells = new Queue<FlowFieldGrid.FlowFieldCell>();
                frontierCells.Enqueue(goalCell);
                while (frontierCells.Count > 0) {
                    var currentCell = frontierCells.Dequeue();
                    var neighborCells = flowFieldGrid.GetNeighborCells(currentCell.GridIndex);
                    foreach (var neighborCell in neighborCells) {
                        if (neighborCell.TerrainCost != 255) {
                            var directionCost =
                                neighborCell.GridIndex.x != currentCell.GridIndex.x
                                && neighborCell.GridIndex.y != currentCell.GridIndex.y
                                    ? 14
                                    : 10;
                            var newIntegrationCost =
                                currentCell.IntegrationCost + neighborCell.TerrainCost + directionCost;
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
            foreach (FlowFieldGrid.FlowFieldCell cell in flowFieldGrid) {
                if (cell.IntegrationCost == 0 || cell.IntegrationCost == int.MaxValue) {
                    cell.DirectionVector = null;
                } else {
                    var shortestPath = flowFieldGrid
                        .GetNeighborCells(cell.GridIndex)
                        .Aggregate((shortestPathCell, nextNeighbor) => {
                            if (shortestPathCell == null) {
                                return nextNeighbor;
                            }

                            return shortestPathCell.IntegrationCost < nextNeighbor.IntegrationCost
                                ? shortestPathCell
                                : nextNeighbor;
                        });
                    cell.DirectionVector = (shortestPath.WorldPosition - cell.WorldPosition).normalized;
                }
            }
        }
    }
}