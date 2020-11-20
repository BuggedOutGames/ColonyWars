using UnityEngine;
using BuggedOutGames.ColonyWars.Events;

namespace BuggedOutGames.ColonyWars.Units {
    public class UnitBehaviour : MonoBehaviour {
        private void OnBecameInvisible() {
            EventManager.Instance.OnUnitExitedVision(this);
        }

        private void OnBecameVisible() {
            EventManager.Instance.OnUnitEnteredVision(this);
        }
    }
}