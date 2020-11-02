using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private List<UnitBehaviour> visibleUnits = new List<UnitBehaviour>();

    public List<UnitBehaviour> GetVisibleUnits() {
        return visibleUnits;
    }

    public void AddVisibleUnit(UnitBehaviour unit) {
        visibleUnits.Add(unit);
    }

    public void RemoveVisibleUnit(UnitBehaviour unit) {
        visibleUnits.Remove(unit);
    }
}