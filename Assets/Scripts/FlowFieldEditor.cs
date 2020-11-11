using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FlowField))]
public class FlowFieldEditor : Editor {

    private static readonly int ControlId = "FlowField.Editor".GetHashCode();
    
    private void OnSceneGUI() {
        FlowField flowField = target as FlowField;

        if (flowField != null) {
            DisplayFlowField(flowField);
            int id = GUIUtility.GetControlID(ControlId, FocusType.Passive);
            switch (Event.current.GetTypeForControl(id)) {
                case EventType.Layout:
                    HandleUtility.AddDefaultControl(id);
                    break;
                case EventType.MouseDown:
                    if (Event.current.button == 0) {
                        Vector2 worldPosition = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
                        var gridCell = flowField.GetGridCellAtWorldPosition(worldPosition);
                        if (gridCell.HasValue) {
                            Debug.Log($"COST: {gridCell.Value.Cost}");
                        } 
                    }
                    break;
            }
        }
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("CellSize"), GUILayout.ExpandWidth(false));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Dimensions"));
        serializedObject.ApplyModifiedProperties();
        SceneView.RepaintAll();
    }

    private void DisplayFlowField(FlowField flowField) {
        Handles.color = Color.cyan;
        foreach (Grid.GridCell cell in flowField) {
            var wireCubePosition = new Vector3(cell.WorldPosition.x, cell.WorldPosition.y, -1);
            var textPosition = new Vector2(cell.WorldPosition.x - flowField.CellSize / 2 + 0.05f,
                cell.WorldPosition.y + flowField.CellSize / 2 - 0.05f);
            var textStyle = new GUIStyle {
                normal = {textColor = Color.white},
            };
            Handles.DrawWireCube(wireCubePosition, new Vector3(flowField.CellSize, flowField.CellSize, 0));
            Handles.Label(textPosition, $"Cost: {cell.Cost}", textStyle);
        }
    }
}