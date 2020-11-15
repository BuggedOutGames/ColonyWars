using Events;
using UnityEngine;

namespace Behaviours {
    public class UnitBehaviour : MonoBehaviour {
        private void OnBecameInvisible() {
            EventManager.Instance.OnUnitExitedVision(this);
        }

        private void OnBecameVisible() {
            EventManager.Instance.OnUnitEnteredVision(this);
        }
    }
}