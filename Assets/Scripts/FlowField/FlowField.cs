using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FlowField {
    [ExecuteInEditMode]
    public class FlowField : MonoBehaviour, IEnumerable {

        public float CellSize;
        public Vector2Int Dimensions;
        public bool displayFlowField;

        private Grid grid;

        private void Update() {
            if (Application.isEditor) {
                grid = new Grid(Dimensions, CellSize);
                CalculateTerrainCostField();
            }
        }

        private void OnDrawGizmos() {
            if (grid != null && displayFlowField) {
                DisplayFlowField();
            }
        }

        private void CalculateTerrainCostField() {
            foreach (Grid.GridCell cell in grid) {
                int layerMask = LayerMask.GetMask("Terrain");
                RaycastHit2D raycastHit = Physics2D.Raycast(cell.WorldPosition, Vector2.zero, layerMask);
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

        public void CalculateIntegrationCostField(Vector2 worldPosition) {
            var goalCell = GetGridCellAtWorldPosition(worldPosition);
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
                            var newIntegrationCost = currentCell.IntegrationCost + neighborCell.TerrainCost;
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

        private void DisplayFlowField() {
            foreach (Grid.GridCell cell in grid) {
                var wireCubePosition = new Vector3(cell.WorldPosition.x, cell.WorldPosition.y, -1);
                var cellRect = new Rect(
                new Vector2(cell.WorldPosition.x - grid.CellSize / 2, cell.WorldPosition.y - grid.CellSize / 2), 
                new Vector2(grid.CellSize, grid.CellSize)
                );
                var terrainCostPosition = new Vector2(
                cell.WorldPosition.x - grid.CellSize / 2 + 0.1f,
                cell.WorldPosition.y + grid.CellSize / 2 - 0.1f
                );
                var integrationCostPosition = new Vector2(
                    cell.WorldPosition.x - grid.CellSize / 2 + 0.1f,
                    cell.WorldPosition.y + grid.CellSize / 2 - 0.4f
                );

                var textStyle = new GUIStyle {
                    normal = {textColor = Color.white},
                };
                var green = new Color(0, 255, 0, 0.1f);
                var red = new Color(255, 0, 0, 0.1f);
                var cellColor = Color.Lerp(green, red, cell.IntegrationCost * 0.0002f);

                Handles.DrawSolidRectangleWithOutline(cellRect, cellColor, cellColor);
                Handles.DrawWireCube(wireCubePosition, new Vector3(grid.CellSize, grid.CellSize, 0));
                Handles.Label(terrainCostPosition, $"{cell.TerrainCost}", textStyle);
                Handles.Label(integrationCostPosition, $"{(cell.IntegrationCost == int.MaxValue ? "∞" : cell.IntegrationCost.ToString())}", textStyle);
            }
        }

        public Grid.GridCell GetGridCellAtWorldPosition(Vector2 worldPosition) {
            return grid.GetCellAtWorldPosition(worldPosition);
        }

        public IEnumerator GetEnumerator() {
            return grid.GetEnumerator();
        }
    }
}