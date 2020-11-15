using System;
using System.Linq;
using UnityEngine;

public class SelectionUI : MonoBehaviour {

    private static readonly string SelectionIndicatorTag = "SelectionIndicator";
    private static readonly Color SelectionBoxColor = new Color(0.8f, 0.8f, 0.95f, 0.25f);
    
    private Rect? selectionBoxBounds;
    
    public GameObject selectionIndicator;

    private void Start() {
        EventManager.Instance.SelectUnitEvent += HandleSelectUnitEvent;
        EventManager.Instance.DeSelectUnitEvent += HandleDeselectUnitEvent;
        EventManager.Instance.StartMouseSelectionBoxEvent += HandleStartMouseSelectionBoxEvent;
        EventManager.Instance.StopMouseSelectionBoxEvent += HandleStopMouseSelectionBoxEvent;
    }
    
    private void OnGUI() {
        if (selectionBoxBounds.HasValue) {
            GraphicsUtil.DrawRect(selectionBoxBounds.Value, SelectionBoxColor);
            GraphicsUtil.DrawRectBorder(selectionBoxBounds.Value,  SelectionBoxColor, 2);
        }
    }
    
    private void HandleSelectUnitEvent(object sender, UnitBehaviour unit) {
        GameObject instantiatedSelectionIndicator = Instantiate(selectionIndicator, unit.transform.position, Quaternion.identity);
        instantiatedSelectionIndicator.tag = SelectionIndicatorTag;
        instantiatedSelectionIndicator.transform.parent = unit.transform;
    }

    private void HandleDeselectUnitEvent(object sender, UnitBehaviour unit) {
        GameObject instantiatedSelectionIndicator = unit.gameObject.GetComponentsInChildren<Transform>()
            .Where(t => t.CompareTag(SelectionIndicatorTag))
            .Select(t => t.gameObject).First();
        Destroy(instantiatedSelectionIndicator);
    }

    private void HandleStartMouseSelectionBoxEvent(object sender, Rect selectionBox) => selectionBoxBounds = selectionBox;
    private void HandleStopMouseSelectionBoxEvent(object sender, EventArgs args) => selectionBoxBounds = null;
}
