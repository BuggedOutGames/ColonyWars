using System.Diagnostics.CodeAnalysis;
using Events;
using UnityEngine;

namespace Behaviours.FlowField {
    public class FlowFieldUI : MonoBehaviour {
        private void Start() {
            EventManager.Instance.MoveCommandEvent += HandleMoveCommandEvent;
        }

        [SuppressMessage("ReSharper", "Unity.PerformanceCriticalCodeInvocation")]
        private static void HandleMoveCommandEvent(object sender, MoveCommand move) {
            FlowFieldManager.Instance.DisplayFlowField(move.Destination);
        }
    }
}