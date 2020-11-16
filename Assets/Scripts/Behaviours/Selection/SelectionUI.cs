using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

using BuggedGames.ColonyWars.Utils;
using BuggedGames.ColonyWars.Events;
using BuggedGames.ColonyWars.Units;

namespace BuggedGames.ColonyWars.Selection {
    public class SelectionUI : MonoBehaviour {
        
        private const string SelectionIndicatorTag = "SelectionIndicator";
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
                GraphicsUtil.DrawRectBorder(selectionBoxBounds.Value, SelectionBoxColor, 2);
            }
        }

        private void HandleSelectUnitEvent(object sender, UnitBehaviour unit) {
            var instantiatedSelectionIndicator =
                Instantiate(selectionIndicator, unit.transform.position, Quaternion.identity);
            instantiatedSelectionIndicator.tag = SelectionIndicatorTag;
            instantiatedSelectionIndicator.transform.parent = unit.transform;
        }

        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        private void HandleDeselectUnitEvent(object sender, UnitBehaviour unit) {
            var instantiatedSelectionIndicator = unit.gameObject.GetComponentsInChildren<Transform>()
                .Where(t => t.CompareTag(SelectionIndicatorTag))
                .Select(t => t.gameObject).First();
            Destroy(instantiatedSelectionIndicator);
        }

        private void HandleStartMouseSelectionBoxEvent(object sender, Rect selectionBox) {
            selectionBoxBounds = selectionBox;
        }

        private void HandleStopMouseSelectionBoxEvent(object sender, EventArgs args) {
            selectionBoxBounds = null;
        }
    }
}