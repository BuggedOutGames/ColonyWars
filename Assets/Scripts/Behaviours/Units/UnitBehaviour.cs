using UnityEngine;

using BuggedGames.ColonyWars.Events;

namespace BuggedGames.ColonyWars.Units {
    public class UnitBehaviour : MonoBehaviour {
        private void OnBecameInvisible() {
            EventManager.Instance.OnUnitExitedVision(this);
        }

        private void OnBecameVisible() {
            EventManager.Instance.OnUnitEnteredVision(this);
        }
    }
}