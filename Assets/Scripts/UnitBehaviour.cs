using UnityEngine;

public class UnitBehaviour : MonoBehaviour {

    private void OnBecameVisible() => EventManager.Instance.OnUnitEnteredVision(this);
    private void OnBecameInvisible() => EventManager.Instance.OnUnitExitedVision(this);
}