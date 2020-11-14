using UnityEditor;
using UnityEngine;

namespace FlowField {
    [CustomEditor(typeof(FlowField))]
    public class FlowFieldEditor : Editor {

        private static readonly int ControlId = "FlowField.Editor".GetHashCode();
    
        private void OnSceneGUI() {
            var flowField = target as FlowField;
            if (flowField != null) {
                int id = GUIUtility.GetControlID(ControlId, FocusType.Passive);
                switch (Event.current.GetTypeForControl(id)) {
                    case EventType.Layout:
                        HandleUtility.AddDefaultControl(id);
                        break;
                    case EventType.MouseDown:
                        if (Event.current.button == 1) {
                            Vector2 worldPosition = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
                            flowField.CalculateFlowField(worldPosition);
                            Event.current.Use();
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
}