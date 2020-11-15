﻿using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class FlowFieldManager : MonoBehaviour {

    public static FlowFieldManager Instance;

    public float CellSize;
    public Vector2Int Dimensions;
    
    private FlowField visibleFlowField;
    private readonly Dictionary<Vector2Int, FlowField> flowFieldCache = new Dictionary<Vector2Int, FlowField>();
    
    private void Awake() {
        if (Application.isPlaying) {
            if (Instance == null) {
                Instance = this;
            } else if (Instance != this) {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnValidate() {
        flowFieldCache.Clear();
        visibleFlowField = null;
    }

    private void OnDrawGizmos() {
        if (visibleFlowField != null) {
            visibleFlowField.DisplayFlowField();
        }
    }

    public void DisplayFlowField(Vector2 worldPosition) {
        visibleFlowField = GetFlowField(worldPosition);
        
    }
    
    public FlowField GetFlowField(Vector2 worldPosition) {
        var flowFieldIndex = GetFlowFieldIndex(worldPosition);
        if (flowFieldCache.ContainsKey(flowFieldIndex)) {
            return flowFieldCache[flowFieldIndex];
        } else {
            var flowField = new FlowField(CellSize, Dimensions);
            flowField.CalculateFlowField(worldPosition);
            flowFieldCache.Add(flowFieldIndex, flowField);
            return flowField;
        }
    }

    private Vector2Int GetFlowFieldIndex(Vector2 worldPosition) {
        return new Vector2Int(
            (int) Mathf.Floor(worldPosition.x / CellSize + Dimensions.x/2f), 
            (int) Mathf.Floor(-worldPosition.y / CellSize + Dimensions.y/2f)
        );
    }
}
