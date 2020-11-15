using UnityEngine;

public class FlowFieldUI : MonoBehaviour {
    
    private void Start() {
        EventManager.Instance.MoveCommandEvent += HandleMoveCommandEvent;
    }

    private void HandleMoveCommandEvent(object sender, MoveCommand move) {
        FlowFieldManager.Instance.DisplayFlowField(move.Destination);
    }
}
