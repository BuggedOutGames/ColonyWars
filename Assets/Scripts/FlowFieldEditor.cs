using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FlowField))]
public class FlowFieldEditor : Editor {

    private static readonly int ControlId = "FlowField.Editor".GetHashCode();
    
    private void OnSceneGUI() {
        FlowField flowField = target as FlowField;

        if (flowField != null) {
            int id = GUIUtility.GetControlID(ControlId, FocusType.Passive);
            switch (Event.current.GetTypeForControl(id)) {
                case EventType.Layout:
                    HandleUtility.AddDefaultControl(id);
                    break;
                case EventType.MouseDown:
                    if (Event.current.button == 0) {
                        Vector2 worldPosition = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
                        var gridCell = flowField.GetGridCellAtWorldPosition(worldPosition);
                        if (gridCell != null) {
                            Debug.Log($"COST: {gridCell.Cost}");
                            Debug.Log($"INDEx: {gridCell.GridIndex}");
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
        EditorGUILayout.PropertyField(serializedObject.FindProperty("displayFlowField"));
        serializedObject.ApplyModifiedProperties();
        SceneView.RepaintAll();
    }
}