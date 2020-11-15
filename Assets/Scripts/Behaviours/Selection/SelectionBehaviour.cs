using System.Collections.Generic;
using UnityEngine;

public class SelectionBehaviour : MonoBehaviour {

    private readonly List<UnitBehaviour> visibleUnits = new List<UnitBehaviour>();
    private readonly List<UnitBehaviour> selection = new List<UnitBehaviour>();
    private bool isSelecting;
    private float mouseDownStart;
    private Vector2 mousePositionStart;
    private Camera mainCamera;
    
    private void Start() {
        mainCamera = Camera.main;
        EventManager.Instance.UnitEnteredVisionEvent += HandleUnitEnteredVisionEvent;
        EventManager.Instance.UnitExitedVisionEvent += HandleUnitExitedVisionEvent;
    }

    private void HandleUnitEnteredVisionEvent(object sender, UnitBehaviour unit) => visibleUnits.Add(unit);
    private void HandleUnitExitedVisionEvent(object sender, UnitBehaviour unit) => visibleUnits.Remove(unit);

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            mouseDownStart = Time.time;
            mousePositionStart = Input.mousePosition;
            DeSelectUnits();
            SelectUnitAtMousePosition();
        } else if (Input.GetMouseButton(0) && (Time.time - mouseDownStart > 0.11)) {
            Rect selectionBoxWorld = ProjectionUtil.GetWorldRect(mainCamera, mousePositionStart, Input.mousePosition);
            Rect selectionBoxScreen = ScreenUtil.GetScreenRect(mousePositionStart, Input.mousePosition);
            SelectUnitsInBounds(selectionBoxWorld);
            EventManager.Instance.OnStartMouseSelectionBoxEvent(selectionBoxScreen);
        } else if (Input.GetMouseButtonUp(0)) {
            EventManager.Instance.OnStopMouseSelectionBoxEvent();
        } else if (Input.GetMouseButtonDown(1)) {
            selection.ForEach(selectedUnit => EventManager.Instance.OnMoveCommand(selectedUnit, ProjectionUtil.GetPositionInWorld(mainCamera, Input.mousePosition)));
        }
    }

    private void SelectUnitsInBounds(Rect bounds) {
        foreach (UnitBehaviour unit in visibleUnits) {
            if (bounds.Contains(unit.gameObject.transform.position)) {
                SelectUnit(unit);
            } else {
                DeSelectUnit(unit);
            }
        }
    }

    private void SelectUnitAtMousePosition() {
        RaycastHit2D hit = ProjectionUtil.CastRayFromScreenToWorld(mainCamera, Input.mousePosition);
        if (hit.collider != null) {
            UnitBehaviour unit = hit.transform.gameObject.GetComponent<UnitBehaviour>();
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

    private void DeSelectUnit(UnitBehaviour unit) {
        if (selection.Contains(unit)) {
            selection.Remove(unit);
            EventManager.Instance.OnDeSelectUnit(unit);
        }
    }
    
    private void DeSelectUnits() {
        foreach (UnitBehaviour unit in selection) {
            EventManager.Instance.OnDeSelectUnit(unit);
        }
        selection.Clear();
    }
}
