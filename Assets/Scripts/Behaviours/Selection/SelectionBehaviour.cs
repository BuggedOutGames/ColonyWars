using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Events;
using UnityEngine;
using Utils;

namespace Behaviours.Selection {
    public class SelectionBehaviour : MonoBehaviour {
        
        private readonly List<UnitBehaviour> selection = new List<UnitBehaviour>();
        private readonly List<UnitBehaviour> visibleUnits = new List<UnitBehaviour>();
        
        private bool isSelecting;
        private float mouseDownStart;
        private Vector2 mousePositionStart;
        private Camera mainCamera;

        private void Start() {
            mainCamera = Camera.main;
            EventManager.Instance.UnitEnteredVisionEvent += HandleUnitEnteredVisionEvent;
            EventManager.Instance.UnitExitedVisionEvent += HandleUnitExitedVisionEvent;
        }

        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        private void Update() {
            if (Input.GetMouseButtonDown(0)) {
                mouseDownStart = Time.time;
                mousePositionStart = Input.mousePosition;
                DeSelectUnits();
                SelectUnitAtMousePosition();
            } else if (Input.GetMouseButton(0) && Time.time - mouseDownStart > 0.11) {
                var selectionBoxWorld = ProjectionUtil.GetWorldRect(mainCamera, mousePositionStart, Input.mousePosition);
                var selectionBoxScreen = ScreenUtil.GetScreenRect(mousePositionStart, Input.mousePosition);
                SelectUnitsInBounds(selectionBoxWorld);
                EventManager.Instance.OnStartMouseSelectionBoxEvent(selectionBoxScreen);
            } else if (Input.GetMouseButtonUp(0)) {
                EventManager.Instance.OnStopMouseSelectionBoxEvent();
            } else if (Input.GetMouseButtonDown(1)) {
                selection.ForEach(selectedUnit => EventManager.Instance.OnMoveCommand(selectedUnit,
                    ProjectionUtil.GetPositionInWorld(mainCamera, Input.mousePosition)));
            }
        }

        private void HandleUnitEnteredVisionEvent(object sender, UnitBehaviour unit) {
            visibleUnits.Add(unit);
        }

        private void HandleUnitExitedVisionEvent(object sender, UnitBehaviour unit) {
            visibleUnits.Remove(unit);
        }

        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        private void SelectUnitsInBounds(Rect bounds) {
            foreach (var unit in visibleUnits) {
                if (bounds.Contains(unit.gameObject.transform.position)) {
                    SelectUnit(unit);
                } else {
                    DeSelectUnit(unit);
                }
            }
        }

        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeNullComparison")]
        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        private void SelectUnitAtMousePosition() {
            var hit = ProjectionUtil.CastRayFromScreenToWorld(mainCamera, Input.mousePosition);
            if (hit.collider != null) {
                var unit = hit.transform.gameObject.GetComponent<UnitBehaviour>();
                if (unit != null) {
                    SelectUnit(unit);
                }
            }
        }

        private void SelectUnit(UnitBehaviour unit) {
            if (!selection.Contains(unit)) {
                selection.Add(unit);
                EventManager.Instance.OnSelectUnit(unit);
            }
        }

        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        private void DeSelectUnit(UnitBehaviour unit) {
            if (selection.Contains(unit)) {
                selection.Remove(unit);
                EventManager.Instance.OnDeSelectUnit(unit);
            }
        }

        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        private void DeSelectUnits() {
            foreach (var unit in selection) {
                EventManager.Instance.OnDeSelectUnit(unit);
            }
            selection.Clear();
        }
    }
}