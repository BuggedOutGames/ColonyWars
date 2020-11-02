using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionBehaviour : MonoBehaviour {

    private static Color SELECTION_BOX_COLOR = new Color(0.8f, 0.8f, 0.95f, 0.25f);
    private List<UnitBehaviour> selection = new List<UnitBehaviour>();
    private bool isSelecting = false;
    private float mouseDownStart;
    private Vector2 mousePositionStart;

    private PlayerController playerController;

    void Start() {
        playerController = GetComponent<PlayerController>();
    }

    void OnGUI() {
        if (isSelecting) {
            Rect selectionBox = Utils.instance.GetScreenRect(mousePositionStart, Input.mousePosition);
            Utils.instance.DrawRect(selectionBox, SELECTION_BOX_COLOR);
            Utils.instance.DrawRectBorder(selectionBox,  SELECTION_BOX_COLOR, 2);

            Rect selectionBoxWorld = Utils.instance.GetWorldRect(mousePositionStart, Input.mousePosition);
            SelectUnitsInBounds(selectionBoxWorld);
        }
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            mouseDownStart = Time.time;
            mousePositionStart = Input.mousePosition;
            DeselectUnits();
            SelectUnitAtMousePosition();
        } else if (Input.GetMouseButton(0) && (Time.time - mouseDownStart > 0.1)) {
            isSelecting = true; 
        } else if (Input.GetMouseButtonUp(0)) {
            isSelecting = false;
        }
    }

    private void SelectUnitsInBounds(Rect bounds) {
        foreach (UnitBehaviour unit in playerController.GetVisibleUnits()) {
            if (bounds.Contains(unit.gameObject.transform.position)) {
                unit.SetSelected(true);
                selection.Add(unit);
            } else {
                unit.SetSelected(false);
                selection.Remove(unit);
            }
        }
    }

    private void SelectUnitAtMousePosition() {
        RaycastHit2D hit = Utils.instance.CastRayAtMousePosition();
        if (hit.collider != null) {
            UnitBehaviour unit = hit.transform.gameObject.GetComponent<UnitBehaviour>();
            if (unit != null) {
                SelectUnit(unit);
            }
        }    
    }

    private void SelectUnit(UnitBehaviour unit) {
        unit.SetSelected(true);
        selection.Add(unit);
    }

    private void DeselectUnits() {
        foreach (UnitBehaviour unit in selection) {
            unit.SetSelected(false);
        }
        selection.Clear();
    }
}
